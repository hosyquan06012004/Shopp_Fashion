using BTL_Mongodb.Models.Data;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using X.PagedList.Extensions;
using X.PagedList;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BTL_Mongodb.Models.BusinessModels.ImplementModels
{
    public class RepositoryCategory : IRepositoryCategory
    {
        private readonly IMongoCollection<Category> _categories;

        public RepositoryCategory(MongoDbContext context)
        {
            _categories = context.Categories;
        }






        public bool Insert(Category entity)
        {
            try
            {
                _categories.InsertOne(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(Category entity)
        {
            var result = _categories.ReplaceOne(c => c.Id == entity.Id, entity);
            return result.MatchedCount > 0;

        }

        public bool Delete(string id)
        {
            var result = _categories.DeleteOne(c => c.Id == id);
            return result.DeletedCount > 0;
        }

        public IPagedList<Category> Search(string cname, int pageNumber, int pageSize)
        {
            var query = _categories.AsQueryable();

            if (!string.IsNullOrEmpty(cname))
            {
                query = query.Where(c => c.Name.ToLower().Contains(cname.ToLower()));
            }

            return query.ToPagedList(pageNumber, pageSize);
        }

        public Category GetById(string id)
        {
            return _categories.Find(c => c.Id == id).FirstOrDefault();
        }
        public List<Category> GetAll()
        {
            return _categories.AsQueryable().ToList();
        }

      

        public IPagedList<Category> GetPage(int page, int pageSize)
        {
            return _categories.AsQueryable().ToPagedList(page, pageSize);
        }

      

       
    }
}