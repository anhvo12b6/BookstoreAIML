using BookstoreAIML.Models;
using BookstoreAIML.Service;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreAIML.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost]
        public IActionResult CreatePaymentUrl(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url); // Chuyển hướng đến trang thanh toán VNPAY
        }

        [HttpGet]
        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.VnPayResponseCode == "00")
            {
                // Thanh toán thành công
                TempData["PaymentMessage"] = "Thanh toán thành công!";
                return RedirectToAction("Success", "Checkout");
            }
            else
            {
                // Thanh toán thất bại
                TempData["PaymentMessage"] = $"Thanh toán thất bại. Mã lỗi: {response.VnPayResponseCode}";
                return RedirectToAction("Failure", "Checkout");
            }
        }
    }
}
