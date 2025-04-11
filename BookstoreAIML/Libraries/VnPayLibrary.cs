using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using BookstoreAIML.Models;
using Microsoft.AspNetCore.Http;

public class VnPayLibrary
{
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
    private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

    // Lấy IP Client
    public string GetIpAddress(HttpContext context)
    {
        try
        {
            var remoteIpAddress = context.Connection.RemoteIpAddress;
            if (remoteIpAddress != null)
            {
                if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                        .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                }

                return remoteIpAddress?.ToString() ?? "127.0.0.1";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return "127.0.0.1";
    }

    // Thêm dữ liệu gửi request
    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
        {
            _requestData[key] = value;
        }
    }

    // Thêm dữ liệu nhận về
    public void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
        {
            _responseData[key] = value;
        }
    }

    public string GetResponseData(string key)
    {
        return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
    }

    // Tạo URL thanh toán
    public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
    {
        var data = new StringBuilder();
        foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
        {
            data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
        }

        var querystring = data.ToString();
        if (querystring.EndsWith("&"))
        {
            querystring = querystring[..^1]; // Xoá dấu & cuối
        }

        var signData = querystring;
        var vnpSecureHash = HmacSha512(vnpHashSecret, signData);
        return $"{baseUrl}?{querystring}&vnp_SecureHash={vnpSecureHash}";
    }

    // Lấy dữ liệu raw từ response để xác minh
    private string GetRawResponseData()
    {
        var data = new StringBuilder();
        _responseData.Remove("vnp_SecureHashType");
        _responseData.Remove("vnp_SecureHash");

        foreach (var (key, value) in _responseData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
        {
            data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
        }

        if (data.Length > 0)
        {
            data.Remove(data.Length - 1, 1); // Xoá dấu & cuối
        }

        return data.ToString();
    }

    // Validate chữ ký
    public bool ValidateSignature(string inputHash, string secretKey)
    {
        var rawData = GetRawResponseData();
        var myChecksum = HmacSha512(secretKey, rawData);
        return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
    }

    // HMAC SHA512
    private string HmacSha512(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);

        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(inputBytes);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    // Phân tích dữ liệu phản hồi từ VNPAY
    public PaymentResponseModel GetFullResponseData(IQueryCollection collection, string hashSecret)
    {
        var vnPay = new VnPayLibrary();

        foreach (var (key, value) in collection)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                vnPay.AddResponseData(key, value);
            }
        }

        var vnpSecureHash = collection["vnp_SecureHash"];
        var orderId = vnPay.GetResponseData("vnp_TxnRef");
        var tranId = vnPay.GetResponseData("vnp_TransactionNo");
        var responseCode = vnPay.GetResponseData("vnp_ResponseCode");
        var orderInfo = vnPay.GetResponseData("vnp_OrderInfo");

        var isValid = vnPay.ValidateSignature(vnpSecureHash, hashSecret);

        return new PaymentResponseModel
        {
            Success = isValid && responseCode == "00",
            PaymentMethod = "VnPay",
            OrderDescription = orderInfo,
            OrderId = orderId,
            PaymentId = tranId,
            TransactionId = tranId,
            Token = vnpSecureHash,
            VnPayResponseCode = responseCode
        };
    }
}

// So sánh key theo chuẩn VNPAY
public class VnPayCompare : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var comparer = CompareInfo.GetCompareInfo("en-US");
        return comparer.Compare(x, y, CompareOptions.Ordinal);
    }
}
