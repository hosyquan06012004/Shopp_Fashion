using BTL_Mongodb.Models;
using BTL_Mongodb.Models.Atributes;
using BTL_Mongodb.Models.BusinessModels;
using BTL_Mongodb.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BTL_Mongodb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepositoryProduct _productRepository;
        private const int PageSize = 8; // Số sản phẩm mỗi trang
        private readonly IRepositoryBrand _brandRepository;
        private readonly IRepositoryCategory _categoryRepository;
        public HomeController(IRepositoryProduct productRepository, IRepositoryBrand brandRepository, IRepositoryCategory categoryRepository)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
        }



        public IActionResult Index(string keyword = "", int page = 1)
        {
            var searchModel = new ProductSearchModel { Name = keyword };
            var products = _productRepository.Search(searchModel, page, PageSize);

            ViewBag.Keyword = keyword;
            return View(products);
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID sản phẩm không hợp lệ.");
            }

            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            // Lấy thông tin category và brand
            var category = _categoryRepository.GetById(product.CategoryId);
            var brand = _brandRepository.GetById(product.BrandId);

            // Chuyển đổi sang ViewModel
            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = category?.Name ?? "Không xác định",
                BrandName = brand?.Name ?? "Không xác định",
                Image = product.Image,
                Images = product.Images ?? new List<string>(),
                CreatedAt = product.CreatedAt
            };

            return View(viewModel);
        }






        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}
