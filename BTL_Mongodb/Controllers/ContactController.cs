using Microsoft.AspNetCore.Mvc;

namespace BTL_Mongodb.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
