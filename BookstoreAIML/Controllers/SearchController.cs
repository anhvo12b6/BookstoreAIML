using BookstoreAIML.Models.ViewModel;
using BookstoreAIML.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BookstoreAIML.Controllers
{
    public class SearchController : Controller
    {
        private readonly DataContext _context;

        public SearchController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index(string query)
        {
            // Nếu không nhập từ khóa tìm kiếm, trả về danh sách trống
            if (string.IsNullOrWhiteSpace(query))
            {
                ViewData["Query"] = string.Empty;
                return View(new List<ProductSearchViewModel>());
            }

            // Truy vấn kết quả kèm include Genre để tránh lỗi null và lỗi LINQ to Entities
            var results = _context.Products
                .Include(p => p.Genre)
                .Where(p => EF.Functions.Like(p.Title, $"%{query}%"))
                .Select(p => new ProductSearchViewModel
                {
                    Id = (int)p.BookID,
                    Title = p.Title,
                    Price = p.Price,
                    CoverImageURL = p.CoverImageURL,
                    GenreName = p.Genre != null ? p.Genre.GenreName : "Không rõ"
                })
                .ToList();

            ViewData["Query"] = query;
            return View(results);
        }
    }
}
