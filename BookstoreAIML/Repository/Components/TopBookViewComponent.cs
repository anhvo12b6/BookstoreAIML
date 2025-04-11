using BookstoreAIML.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAIML.Repository.Components
{
    public class TopBookViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        public TopBookViewComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var topBooks = await _context.Products
                .OrderByDescending(p => p.AverageRating)
                .Take(8)
                .Select(p => new FeaturedBookVM
                {
                    BookID = (int)p.BookID,
                    Title = p.Title,
                    CoverImageURL = p.CoverImageURL,
                    Price = p.Price,
                    AverageRating = (double)p.AverageRating
                })
                .ToListAsync();
            return View(topBooks);
        }
    }
}
