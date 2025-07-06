using Microsoft.AspNetCore.Mvc;

namespace BTL_Mongodb.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
