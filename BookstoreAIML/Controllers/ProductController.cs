using BookstoreAIML.Models;
using BookstoreAIML.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAIML.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext Context)
        {
            _dataContext = Context;
        }

        public async Task<IActionResult> Index(decimal? minPrice, decimal? maxPrice)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            ViewBag.CartCount = cart.Sum(item => item.Quantity);

            // Lấy danh sách sản phẩm
            var productsQuery = _dataContext.Products.AsQueryable();

            // Lọc theo giá nếu có truyền vào
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            // Trả kết quả
            var products = await productsQuery.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int Id)
        {
            if (Id == 0)
            {
                return RedirectToAction("Index");
            }

            var bookById = await _dataContext.Products
                .Include(p => p.Genre)
                .FirstOrDefaultAsync(p => p.BookID == Id);

            if (bookById == null)
            {
                return NotFound();
            }

            return View(bookById);
        }
    }
}
