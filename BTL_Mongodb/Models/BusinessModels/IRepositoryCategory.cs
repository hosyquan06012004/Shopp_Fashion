using X.PagedList;

namespace BTL_Mongodb.Models.BusinessModels
{
    public interface IRepositoryCategory : IRepositoryGeneric<Category, string>
    {
        IPagedList<Category> GetPage(int page, int pageSize);
        IPagedList<Category> Search(string cname, int pageNumber, int pageSize);
    }
}
