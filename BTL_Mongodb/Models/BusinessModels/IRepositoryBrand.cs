using X.PagedList;

namespace BTL_Mongodb.Models.BusinessModels
{
    public interface IRepositoryBrand : IRepositoryGeneric<Brand, string>
    {
        IPagedList<Brand> GetPage(int page, int pageSize);
        IPagedList<Brand> Search(string bname, int pageNumber, int pageSize);
    }
}
