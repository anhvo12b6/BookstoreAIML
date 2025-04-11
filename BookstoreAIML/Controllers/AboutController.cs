using Microsoft.AspNetCore.Mvc;

namespace BookstoreAIML.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
