
using BookstoreAIML.Models.ViewModel;
using BookstoreAIML.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAIML.Repository.Components;

public class TopSellingBooksViewComponent : ViewComponent
{
    private readonly DataContext _context;

    public TopSellingBooksViewComponent(DataContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(int count = 5)
    {
        var topBooks = await _context.OrderDetails
            .GroupBy(od => od.ProductId)
            .Select(group => new
            {
                ProductId = group.Key,
                TotalSold = group.Sum(od => od.Quantity)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(count*2) 
            .Join(_context.Products,
                  x => x.ProductId,
                  p => p.BookID,
                  (x, p) => new ProductViewModel
                  {
                      BookID = (int)p.BookID,
                      Title = p.Title,
                      CoverImageURL = p.CoverImageURL,
                      Price = p.Price,
                      Discount = (float)p.Discount,
                      AverageRating = (double)p.AverageRating
                  })
            .Where(p => p.AverageRating >= 4.0)
            .Take(count)
            .ToListAsync();

        return View(topBooks);
    }
}
