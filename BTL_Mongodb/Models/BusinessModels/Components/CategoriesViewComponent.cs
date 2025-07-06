using BTL_Mongodb.Models.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace BTL_Mongodb.Models.BusinessModels.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly MongoDbContext _mongoDbContext;

        public CategoriesViewComponent(MongoDbContext context)
        {
            _mongoDbContext = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentCategoryId = HttpContext.Request.Query["categoryId"].FirstOrDefault();
            var currentBrandId = HttpContext.Request.Query["brandId"].FirstOrDefault();
            var keyword = HttpContext.Request.Query["keyword"].FirstOrDefault();

            var categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();

            // Lấy số lượng sản phẩm cho mỗi category
            var categoryCounts = await _mongoDbContext.Products
                .Aggregate()
                .Group(p => p.CategoryId,
                       g => new { CategoryId = g.Key, Count = g.Count() })
                .ToListAsync();

            var countsDict = categoryCounts.ToDictionary(
                x => x.CategoryId.ToString(),
                x => x.Count);

            // Tạo model mới để truyền dữ liệu
            var model = new CategoryViewComponentModel
            {
                Categories = categories,
                Counts = countsDict,
                CurrentCategoryId = currentCategoryId,
                CurrentBrandId = currentBrandId,
                Keyword = keyword
            };

            return View(model);
        }
    }

    public class CategoryViewComponentModel
    {
        public List<Category> Categories { get; set; }
        public Dictionary<string, int> Counts { get; set; }
        public string CurrentCategoryId { get; set; }
        public string CurrentBrandId { get; set; }
        public string Keyword { get; set; }
    }
}