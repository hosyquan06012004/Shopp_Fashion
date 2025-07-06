using BTL_Mongodb.Helpers;
using BTL_Mongodb.Models.ViewModel;
using System.Security.Policy;

namespace BTL_Mongodb.Models.BusinessModels.ImplementModels
{
    public class VnPayService : IVnPayService
    {
        public readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); 
    
         
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);

            vnpay.AddRequestData("vnp_OrderInfo", "thanh toán cho đơn hàng:" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackUrl"]);

            vnpay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu của giao dịch tại hệ 


            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);

            return paymentUrl;
        }

        //public VnPaymentResponseModel PaymentExcute(IQueryCollection collections)
        //{
        //    var vnpay = new VnPayLibrary();
        //    foreach(var(key, value) in collections)
        //    {
        //        if(!string.IsNullOrEmpty(key) && key.StartsWith("vnp_")){
        //            vnpay.AddRequestData(key, value.ToString());
        //        }


        //    }

        //    var vnp_OrderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
        //    var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
        //    var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
        //    var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        //    var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");


        //    bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
        //    if (!checkSignature)
        //    {
        //        return new VnPaymentResponseModel
        //        {
        //            Success = false
        //        };

        //    }


        //    return new VnPaymentResponseModel { 
        //        Success = true,
        //        PaymentMethod = "VnPay",
        //        OrderDescription = vnp_OrderInfo,
        //        OrderId = vnp_OrderId.ToString(),
        //        TransactionId = vnp_TransactionId.ToString(),
        //        Token = vnp_SecureHash,
        //        VnPayResponseCode = vnp_ResponseCode
        //    };

        //}


        public VnPaymentResponseModel PaymentExcute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();

            // Sửa: Dùng AddResponseData thay vì AddRequestData
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            // Kiểm tra và xử lý vnp_TransactionNo an toàn
            var vnp_TransactionNo = vnpay.GetResponseData("vnp_TransactionNo");
            if (!long.TryParse(vnp_TransactionNo, out var vnp_TransactionId))
            {
                // Log lỗi và xử lý phù hợp
                Console.WriteLine($"Invalid vnp_TransactionNo: {vnp_TransactionNo}");
                return new VnPaymentResponseModel { Success = false };
            }

            // Xử lý vnp_TxnRef an toàn
            var vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");
            if (!long.TryParse(vnp_TxnRef, out var vnp_OrderId))
            {
                Console.WriteLine($"Invalid vnp_TxnRef: {vnp_TxnRef}");
                return new VnPaymentResponseModel { Success = false };
            }

            var vnp_SecureHash = collections["vnp_SecureHash"];
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            // Validate chữ ký
            if (!vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]))
            {
                Console.WriteLine("Invalid signature");
                return new VnPaymentResponseModel { Success = false };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_OrderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }
    }
}
