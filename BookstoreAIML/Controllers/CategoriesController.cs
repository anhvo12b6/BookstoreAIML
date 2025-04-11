using BookstoreAIML.Models;
using BookstoreAIML.Models.ViewModel;
using BookstoreAIML.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;

namespace BookstoreAIML.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoriesController(DataContext Context)
        {
            _dataContext = Context;
        }

        public async Task <IActionResult> Index(int id)
        {
            GenreModel categories = _dataContext.Genres.Where(c => c.GenreID == id).FirstOrDefault();
            if (categories == null) return RedirectToAction("Index");
            var productsByCategories = _dataContext.Products.Where(p => p.GenreID == categories.GenreID);

            return View(await productsByCategories.OrderByDescending(p=>p.GenreID).ToListAsync());
        }
        
    }
}
