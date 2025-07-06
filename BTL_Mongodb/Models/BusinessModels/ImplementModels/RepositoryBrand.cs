using BTL_Mongodb.Models.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using X.PagedList;
using X.PagedList.Extensions;

namespace BTL_Mongodb.Models.BusinessModels.ImplementModels
{
    
    public class RepositoryBrand : IRepositoryBrand
    {
        private readonly IMongoCollection<Brand> _brands;

        public RepositoryBrand(MongoDbContext context)
        {
            _brands = context.Brands;
        }
        public bool Delete(string id)
        {
            var results = _brands.DeleteOne(b => b.Id == id);
            return results.DeletedCount > 0;
        }

        public List<Brand> GetAll()
        {
            return _brands.AsQueryable().ToList();
        }

        public Brand GetById(string id)
        {
           return _brands.Find( b => b.Id == id).FirstOrDefault();
        }

        public IPagedList<Brand> GetPage(int page, int pageSize)
        {
            return _brands.AsQueryable().ToPagedList(page, pageSize);
        }

        public bool Insert(Brand entity)
        {
            try
            {
                _brands.InsertOne(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IPagedList<Brand> Search(string bname, int pageNumber, int pageSize)
        {
            var query = _brands.AsQueryable();
            if (!string.IsNullOrEmpty(bname)) { 
                query = query.Where(b => b.Name.ToLower().Contains(bname.ToLower()));
            }
            return query.ToPagedList(pageNumber, pageSize);
        }

        public bool Update(Brand entity)
        {
            var result = _brands.ReplaceOne(b => b.Id == entity.Id, entity);
            return result.ModifiedCount > 0;
        }
    }
}
