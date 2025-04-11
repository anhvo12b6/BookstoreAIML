using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookstoreAIML.Repository;
using BookstoreAIML.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList.Extensions;



namespace BookstoreAIML.ProductControllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        // GET: Product
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var products = _context.Products
                .Include(p => p.Genre)
                .OrderByDescending(p => p.BookID)
                .ToPagedList(pageNumber, pageSize); 

            return View(products);
        }


        // GET: Product/Create
        public IActionResult Create()
        {
            PopulateGenres();
            return View(new ProductViewModel());
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateGenres(product.GenreID);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            PopulateGenres(product.GenreID);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ProductViewModel product)
        {
            if (id != product.BookID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    product.UpdatedAt = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.BookID == product.BookID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateGenres(product.GenreID);
            return View(product);
        }

        private void PopulateGenres(long? selectedGenreId = null)
        {
            var genres = _context.Genres.ToList();
            ViewBag.GenreID = new SelectList(genres, "GenreID", "GenreName", selectedGenreId);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Genre)
                .FirstOrDefaultAsync(m => m.BookID == id);

            if (product == null) return NotFound();

            return View(product);
        }
        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
