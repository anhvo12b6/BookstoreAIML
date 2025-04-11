using BookstoreAIML.Models.ViewModel;
using BookstoreAIML.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreAIML.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly DataContext _context;
        public AdminController(DataContext context)
        {
            _context = context;
        }
        public IActionResult RevenueStatistics()
        {
            // Lấy tất cả đơn hàng đã thanh toán
            var completedOrders = _context.Orders
                .Where(o => o.Status == 1)
                .ToList();

            // Gom nhóm theo ngày và tính doanh thu
            var stats = completedOrders
                .GroupBy(o => o.CreatedDate.Date)
                .Select(group => new RevenueStatViewModel
                {
                    Date = group.Key,
                    Revenue = _context.OrderDetails
                        .Where(d => group.Select(o => o.OrderCode).Contains(d.OrderCode))
                        .Sum(d => d.Price * d.Quantity),
                    OrderCount = group.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            return View(stats);
        }
    }
}
