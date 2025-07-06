using BTL_Mongodb.Models.Atributes;
using BTL_Mongodb.Models.ViewModel;
using X.PagedList;

namespace BTL_Mongodb.Models.BusinessModels
{
    public interface IRepositoryProduct : IRepositoryGeneric<Product, string>
    {
        IPagedList<Product> GetPage(int page, int pageSize);
        IPagedList<ProductViewModel> Search(ProductSearchModel searchModel, int pageNumber, int pageSize);
        IPagedList<ProductViewModel> GetFull(int page, int pageSize);
        ProductViewModel ConvertToViewModel(Product product, string categoryName, string brandName);
		IPagedList<ProductViewModel> SearchProductOFBrandCategory(ProductSearchBrandCategoryModel searchModel, int page, int pageSize);

	}
}
