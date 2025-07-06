using BTL_Mongodb.Models.Data;
using MongoDB.Driver;
using X.PagedList;
using X.PagedList.Extensions;

namespace BTL_Mongodb.Models.BusinessModels.ImplementModels
{
    public class RepositoryBanner : IRepositoryBanner
    {
        private readonly IMongoCollection<Banner> _banners;

        public RepositoryBanner(MongoDbContext context)
        {
            _banners = context.Baners;
        }

        public bool Delete(string id)
        {
            var result = _banners.DeleteOne(b => b.Id.ToString() == id);
            return result.DeletedCount > 0;
        }

        public List<Banner> GetAll()
        {
            return _banners.AsQueryable().ToList();
        }

        public Banner GetById(string id)
        {
            return _banners.Find(b => b.Id.ToString() == id).FirstOrDefault();
        }

        public IPagedList<Banner> GetPage(int page, int pageSize)
        {
            return _banners.AsQueryable().ToPagedList(page, pageSize);
        }

        public bool Insert(Banner entity)
        {
            try
            {
                _banners.InsertOne(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IPagedList<Banner> Search(string keyword, int pageNumber, int pageSize)
        {
            var query = _banners.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b => b.H1.ToLower().Contains(keyword.ToLower()));
            }
            return query.ToPagedList(pageNumber, pageSize);
        }

        public bool Update(Banner entity)
        {
            var result = _banners.ReplaceOne(b => b.Id == entity.Id, entity);
            return result.ModifiedCount > 0;
        }
    }
}
