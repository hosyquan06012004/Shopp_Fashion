using BTL_Mongodb.Models.ViewModel;
using X.PagedList;

namespace BTL_Mongodb.Models.BusinessModels
{
    public interface IRepositoryOrder : IRepositoryGeneric<Order, string>
    {
        IPagedList<Order> GetPage(int page, int pageSize);
        IPagedList<OrderViewModel> Search(OrderSearchModel searchModel, int pageNumber, int pageSize);
        IPagedList<OrderViewModel> GetFull(int page, int pageSize);
        OrderViewModel ConvertToViewModel(Order order);
    }
}
