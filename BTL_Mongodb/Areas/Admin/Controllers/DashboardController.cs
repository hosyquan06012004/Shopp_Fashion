using BTL_Mongodb.Models.Atributes;
using Microsoft.AspNetCore.Mvc;
using BTL_Mongodb.Models.Data;
using BTL_Mongodb.Models;
using MongoDB.Driver;
namespace BTL_Mongodb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizePermission("Dashboard", "View")]
    public class DashboardController : Controller
    {
        private readonly MongoDbContext _context;

        public DashboardController(MongoDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Đếm số lượng tài liệu trong mỗi collection
            long categoryCount = _context.Categories.CountDocuments(Builders<Category>.Filter.Empty);
            long productCount = _context.Products.CountDocuments(Builders<Product>.Filter.Empty);
            long brandCount = _context.Brands.CountDocuments(Builders<Brand>.Filter.Empty);
            long orderCount = _context.Orders.CountDocuments(Builders<Order>.Filter.Empty);

            // Truyền dữ liệu qua ViewBag
            ViewBag.CategoryCount = categoryCount;
            ViewBag.ProductCount = productCount;
            ViewBag.BrandCount = brandCount;
            ViewBag.OrderCount = orderCount;

            long totalAll = categoryCount + productCount + brandCount + orderCount;

            ViewBag.ProductPercent = totalAll > 0 ? (double)productCount / totalAll * 100 : 0;
            ViewBag.CategoryPercent = totalAll > 0 ? (double)categoryCount / totalAll * 100 : 0;
            ViewBag.BrandPercent = totalAll > 0 ? (double)brandCount / totalAll * 100 : 0;
            ViewBag.OrderPercent = totalAll > 0 ? (double)orderCount / totalAll * 100 : 0;

            return View();
        }
    }
}
