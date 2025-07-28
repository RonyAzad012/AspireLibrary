using Microsoft.AspNetCore.Mvc;

namespace AspireLibrary.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
