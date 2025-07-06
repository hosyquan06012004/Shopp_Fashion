using X.PagedList;

namespace BTL_Mongodb.Models.BusinessModels
{
    public interface IRepositoryBanner : IRepositoryGeneric<Banner, string>
    {
        IPagedList<Banner> GetPage(int page, int pageSize);
        IPagedList<Banner> Search(string keyword, int pageNumber, int pageSize);
    }
}
