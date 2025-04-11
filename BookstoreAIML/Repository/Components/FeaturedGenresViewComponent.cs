// Components/FeaturedGenresViewComponent.cs
using BookstoreAIML.Models;
using BookstoreAIML.Models.ViewModel;
using BookstoreAIML.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAIML.Components
{
    public class FeaturedGenresViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public FeaturedGenresViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var topGenres = await _context.Genres
                .Include(g => g.Products)
                .Select(g => new FeaturedGenreVM
                {
                    GenreID = g.GenreID,
                    GenreName = g.GenreName,
                    BookCount = g.Products.Count,
                    CoverImageURL =g.Products.FirstOrDefault().CoverImageURL, // Assuming you want the cover image of the first book in the genre
                })
                .OrderByDescending(g => g.BookCount)
                .Take(6)
                .ToListAsync();

            return View(topGenres);
        }
    }
}
