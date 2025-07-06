using BTL_Mongodb.Models.Atributes;
using BTL_Mongodb.Models.Data;
using BTL_Mongodb.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Xml.Linq;
using X.PagedList;
using X.PagedList.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BTL_Mongodb.Models.BusinessModels.ImplementModels
{
    public class RepositoryProduct : IRepositoryProduct
    {
        private readonly IMongoCollection<Product> _products;
        public RepositoryProduct(MongoDbContext context)
        {
            _products = context.Products;
        }
        public bool Delete(string id)
        {
            var results = _products.DeleteOne(p => p.Id == id);
            return results.DeletedCount > 0;
        }

        public List<Product> GetAll()
        {
            return _products.AsQueryable().ToList();
        }

        public Product GetById(string id)
        {
            return _products.Find(p => p.Id == id).FirstOrDefault();
        }

        public IPagedList<ProductViewModel> GetFull(int page, int pageSize)
        {
            BsonDocument[] lookup = new BsonDocument[]
            {
            new BsonDocument
            {
                {
                    "$lookup", new BsonDocument
                    {
                        { "from", "categories" },
                        { "localField", "categoryId" },
                        { "foreignField", "_id" },
                        { "as", "categories" }
                    }
                }
            },
            new BsonDocument
            {
                {
                    "$lookup", new BsonDocument
                    {
                        { "from", "brands" },
                        { "localField", "brandId" },
                        { "foreignField", "_id" },
                        { "as", "brands" }
                    }
                }
            }
            };

            var products = _products.Aggregate<BsonDocument>(lookup).ToList();
            var data = new List<ProductViewModel>();

            foreach (var e in products)
            {
                var p = new ProductViewModel();
                p.Id = e["_id"].ToString();
                p.Name = e.Contains("name") ? e["name"].AsString : "";
                p.Description = e.Contains("description") && e["description"].BsonType == BsonType.String
                    ? e["description"].AsString
                    : "";

                p.Price = e.Contains("price") ? e["price"].ToDouble() : 0;
                p.Image = e.Contains("image") ? e["image"].AsString : "";
                p.Images = e.Contains("images") ? e["images"].AsBsonArray.Select(x => x.AsString).ToList() : new List<string>();
                p.CreatedAt = e.Contains("createdAt") ? e["createdAt"].ToUniversalTime() : DateTime.MinValue;
                p.CategoryName = e["categories"].AsBsonArray[0]["name"].ToString();
                p.BrandName = e["brands"].AsBsonArray[0]["name"].ToString();
               

                data.Add(p);
            }

            // Phân trang
            return data.ToPagedList(page, pageSize);
        }



        public IPagedList<Product> GetPage(int page, int pageSize)
        {
            return _products.AsQueryable().ToPagedList(page, pageSize);
        }

        public bool Insert(Product entity)
        {
            try
            {
                _products.InsertOne(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IPagedList<ProductViewModel> Search(ProductSearchModel searchModel, int pageNumber, int pageSize)
        {
            var pipeline = new List<BsonDocument>();

            // Bước 1: Thêm match theo name, price nếu có
            var filters = new List<BsonDocument>();

            if (!string.IsNullOrEmpty(searchModel.Name))
            {
                filters.Add(new BsonDocument("name", new BsonDocument
        {
            { "$regex", searchModel.Name },
            { "$options", "i" }
        }));
            }

            if (searchModel.Price.HasValue)
            {
                filters.Add(new BsonDocument("price", searchModel.Price.Value));
            }

            if (filters.Any())
            {
                pipeline.Add(new BsonDocument("$match", new BsonDocument("$and", new BsonArray(filters))));
            }

            // Bước 2: Lookup categories và brands
            pipeline.AddRange(new[]
            {
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "categories" },
            { "localField", "categoryId" },
            { "foreignField", "_id" },
            { "as", "categories" }
        }),
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "brands" },
            { "localField", "brandId" },
            { "foreignField", "_id" },
            { "as", "brands" }
        })
    });

            // Bước 3: Lấy toàn bộ dữ liệu về
            var products = _products.Aggregate<BsonDocument>(pipeline).ToList();
            var data = new List<ProductViewModel>();

            foreach (var e in products)
            {
                var p = new ProductViewModel();
                p.Id = e["_id"].ToString();
                p.Name = e.Contains("name") ? e["name"].AsString : "";
                p.Description = e.Contains("description") && e["description"].BsonType == BsonType.String
                 ? e["description"].AsString
                 : "";

                p.Price = e.Contains("price") ? e["price"].ToDouble() : 0;
                p.Image = e.Contains("image") ? e["image"].AsString : "";
                p.Images = e.Contains("images") ? e["images"].AsBsonArray.Select(x => x.AsString).ToList() : new List<string>();
                p.CreatedAt = e.Contains("createdAt") ? e["createdAt"].ToUniversalTime() : DateTime.MinValue;
                p.CategoryName = e.Contains("categories") && e["categories"].AsBsonArray.Any()
                    ? e["categories"].AsBsonArray[0]["name"].ToString()
                    : "";
                p.BrandName = e.Contains("brands") && e["brands"].AsBsonArray.Any()
                    ? e["brands"].AsBsonArray[0]["name"].ToString()
                    : "";

                // Bổ sung filter theo CategoryName và BrandName sau khi lookup
                bool matchCategory = string.IsNullOrEmpty(searchModel.CategoryName) || p.CategoryName.Contains(searchModel.CategoryName, StringComparison.OrdinalIgnoreCase);
                bool matchBrand = string.IsNullOrEmpty(searchModel.BrandName) || p.BrandName.Contains(searchModel.BrandName, StringComparison.OrdinalIgnoreCase);

                if (matchCategory && matchBrand)
                {
                    data.Add(p);
                }
            }

            // Bước 4: Phân trang
            return data.ToPagedList(pageNumber, pageSize);
        }


        public bool Update(Product entity)
        {
            var result = _products.ReplaceOne(p => p.Id == entity.Id, entity);
            return result.ModifiedCount > 0;
        }


        public ProductViewModel ConvertToViewModel(Product product, string categoryName, string brandName)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = categoryName,
                BrandName = brandName,
                Image = product.Image,
                Images = product.Images ?? new List<string>(),
                CreatedAt = product.CreatedAt
            };
        }

        public IPagedList<ProductViewModel> SearchProductOFBrandCategory(ProductSearchBrandCategoryModel searchModel, int page, int pageSize)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;

            // Lọc theo tên (không phân biệt hoa thường)
            if (!string.IsNullOrEmpty(searchModel.Name))
            {
                filter &= builder.Regex(x => x.Name, new BsonRegularExpression(searchModel.Name, "i"));
            }

            // Lọc theo categoryId
            if (!string.IsNullOrEmpty(searchModel.CategoryId))
            {
                filter &= builder.Eq(x => x.CategoryId, searchModel.CategoryId);
            }

            // Lọc theo brandId
            if (!string.IsNullOrEmpty(searchModel.BrandId))
            {
                filter &= builder.Eq(x => x.BrandId, searchModel.BrandId);
            }

            // Tổng số sản phẩm thỏa điều kiện
            var totalItems = (int)_products.CountDocuments(filter);

            // Tìm các sản phẩm
            var items = _products.Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            // Kết nối để lấy thêm thông tin tên Category và Brand
            var categoryCollection = _products.Database.GetCollection<BsonDocument>("categories");
            var brandCollection = _products.Database.GetCollection<BsonDocument>("brands");

            var categoryMap = categoryCollection.Find(Builders<BsonDocument>.Filter.Empty)
                .ToList()
                .ToDictionary(x => x["_id"].ToString(), x => x["name"].AsString);

            var brandMap = brandCollection.Find(Builders<BsonDocument>.Filter.Empty)
                .ToList()
                .ToDictionary(x => x["_id"].ToString(), x => x["name"].AsString);

            // Chuyển sang ViewModel
            var result = items.Select(product =>
            {
                categoryMap.TryGetValue(product.CategoryId, out string categoryName);
                brandMap.TryGetValue(product.BrandId, out string brandName);

                return new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Image = product.Image,
                    Images = product.Images ?? new List<string>(),
                    CreatedAt = product.CreatedAt,
                    CategoryName = categoryName ?? "",
                    BrandName = brandName ?? ""
                };
            }).ToList();

            return new StaticPagedList<ProductViewModel>(result, page, pageSize, totalItems);
        }

    }
}
