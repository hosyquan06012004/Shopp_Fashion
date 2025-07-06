using BTL_Mongodb.Models.ViewModel;

namespace BTL_Mongodb.Models.BusinessModels
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext content, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExcute(IQueryCollection collections);
    }
}
