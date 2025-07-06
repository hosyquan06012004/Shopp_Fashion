using BTL_Mongodb.Models.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace BTL_Mongodb.Models.BusinessModels.Components
{
	public class BrandsViewComponent : ViewComponent
	{
		private readonly MongoDbContext _mongoDbContext;

		public BrandsViewComponent(MongoDbContext context)
		{
			_mongoDbContext = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var currentBrandId = HttpContext.Request.Query["brandId"].FirstOrDefault();
			var currentCategoryId = HttpContext.Request.Query["categoryId"].FirstOrDefault();
			var keyword = HttpContext.Request.Query["keyword"].FirstOrDefault();

			var brands = await _mongoDbContext.Brands.Find(_ => true).ToListAsync();

			// Lấy số lượng sản phẩm cho mỗi brand
			var brandCounts = await _mongoDbContext.Products
				.Aggregate()
				.Group(p => p.BrandId,
					   g => new { BrandId = g.Key, Count = g.Count() })
				.ToListAsync();

			var countsDict = brandCounts.ToDictionary(
				x => x.BrandId.ToString(),
				x => x.Count);

			// Tạo model mới để truyền dữ liệu
			var model = new BrandViewComponentModel
			{
				Brands = brands,
				Counts = countsDict,
				CurrentBrandId = currentBrandId,
				CurrentCategoryId = currentCategoryId,
				Keyword = keyword
			};

			return View(model);
		}
	}

	public class BrandViewComponentModel
	{
		public List<Brand> Brands { get; set; }
		public Dictionary<string, int> Counts { get; set; }
		public string CurrentBrandId { get; set; }
		public string CurrentCategoryId { get; set; }
		public string Keyword { get; set; }
	}
}