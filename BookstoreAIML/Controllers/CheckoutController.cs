using BookstoreAIML.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BookstoreAIML.Repository;
using BookstoreAIML.Models.ViewModel;

using System.Text.Json;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using BookstoreAIML.Service;

namespace BookstoreAIML.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        

        public CheckoutController(DataContext dataContext)
        {
            _dataContext = dataContext;
            

        }

        // Trang Checkout - Hiển thị thông tin giỏ hàng
        public IActionResult Index()
        {
            var cartItems = GetCartItemsFromSession();
            var grandTotal = cartItems.Sum(item => item.Price * item.Quantity);

            var model = new CartItemViewModel
            {
                CartItems = cartItems,
                GrandTotal = grandTotal
            };

            return View(model);
        }

        // Xử lý đặt hàng
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = GetCartItemsFromSession();
            if (cartItems == null || !cartItems.Any())
            {
                TempData["error"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "Cart");
            }

            string orderCode = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();

            var order = new OrderModel
            {
                OrderCode = orderCode,
                UserName = userEmail,
                Status = 1,
                CreatedDate = DateTime.Now
            };

            _dataContext.Orders.Add(order);

            var orderDetails = cartItems.Select(cart => new OrderDetails
            {
                OrderCode = orderCode,
                UserName = userEmail,
                ProductId = cart.ProductId,
                Price = cart.Price,
                Quantity = cart.Quantity
            }).ToList();

            _dataContext.OrderDetails.AddRange(orderDetails);

            await _dataContext.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");

            TempData["success"] = "Đơn hàng đã được tạo. Vui lòng chờ duyệt!";
            return RedirectToAction("Index", "Cart");
        }

        
        private List<CartItemModel> GetCartItemsFromSession()
        {
            var sessionData = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(sessionData))
                return new List<CartItemModel>();

            return System.Text.Json.JsonSerializer.Deserialize<List<CartItemModel>>(sessionData);
        }
        

    }
}
