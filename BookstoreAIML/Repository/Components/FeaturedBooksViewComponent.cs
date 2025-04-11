using BookstoreAIML.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAIML.Repository.Components
{
    public class FeaturedBooksViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public FeaturedBooksViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var featuredBooks = await _context.Products
                .OrderByDescending(p => p.AverageRating)
                .Take(6)
                .Select(p => new FeaturedBookVM
                {
                    BookID = (int)p.BookID,
                    Title = p.Title,
                    CoverImageURL = p.CoverImageURL,
                    Price = p.Price,
                    Discount = p.Discount,
                    AverageRating = (double)p.AverageRating
                })
                .ToListAsync();

            return View(featuredBooks);
        }
    }

}
