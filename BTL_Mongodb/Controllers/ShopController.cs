using BTL_Mongodb.Models.Atributes;
using BTL_Mongodb.Models.BusinessModels;
using Microsoft.AspNetCore.Mvc;

namespace BTL_Mongodb.Controllers
{
    public class ShopController : Controller
    {
        private readonly IRepositoryProduct _productRepository;
        private const int PageSize = 12;
        private readonly IRepositoryBrand _brandRepository;
        private readonly IRepositoryCategory _categoryRepository;
        
        public ShopController(IRepositoryProduct productRepository, 
                           IRepositoryBrand brandRepository, 
                           IRepositoryCategory categoryRepository)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
        }

		public IActionResult Index(string keyword = "", string categoryId = null, string brandId = null, int page = 1)
		{
			var searchModel = new ProductSearchBrandCategoryModel
			{
				Name = keyword,
				CategoryId = categoryId,
				BrandId = brandId
			};

			var products = _productRepository.SearchProductOFBrandCategory(searchModel, page, PageSize);

			ViewBag.Keyword = keyword;
			ViewBag.CategoryId = categoryId;
			ViewBag.BrandId = brandId;

			return View(products);
		}
	}
}