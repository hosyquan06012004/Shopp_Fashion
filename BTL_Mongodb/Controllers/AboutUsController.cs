using Microsoft.AspNetCore.Mvc;

namespace BTL_Mongodb.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
