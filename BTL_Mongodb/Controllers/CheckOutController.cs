using BTL_Mongodb.Models.BusinessModels;
using BTL_Mongodb.Models.Data;
using BTL_Mongodb.Models.ViewModel;
using BTL_Mongodb.Models;
using Microsoft.AspNetCore.Mvc;
using BTL_Mongodb.Models.Atributes.Services;
using Microsoft.AspNetCore.Authorization;

namespace BTL_Mongodb.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly MongoDbContext _dbContext;
        private readonly IEmailSender _emailSender;
        private readonly IVnPayService _vnPayService;

        public CheckOutController(MongoDbContext dbContext, IEmailSender emailSender, IVnPayService vnPayService)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
            _vnPayService = vnPayService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            if (cart.Count == 0)
            {
                TempData["error"] = "Giỏ hàng đang trống!";
                return RedirectToAction("Index", "Cart");
            }

            return View(new CheckoutViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel model, string payment = "COD")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (!User.Identity.IsAuthenticated)
            {
                TempData["error"] = "Vui lòng đăng nhập trước khi thanh toán.";
                return RedirectToAction("Login", "AccountUser");
            }


            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            if (cart.Count == 0)
            {
                TempData["error"] = "Không có sản phẩm trong giỏ hàng.";
                return RedirectToAction("Index", "Cart");
            }
            if(payment == "Thanh Toán VnPay")
            {

                var Cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                HttpContext.Session.SetJson("CheckoutInfo", model);
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = Cart.Sum(p => p.Price),
                    CreatedDate = DateTime.Now,
                    Description = $"{model.FirstName} {model.LastName} {model.Phone}",
                    FullName = $"{model.FirstName} {model.LastName}",
                    OrderId = new Random().Next(1000, 10000),
                };
                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
            }
            var order = new Order
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Country = model.Country,
                Address = model.Address,
                TownCity = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                Phone = model.Phone,
                Email = model.Email,
                Note = model.Note,
                TotalAmount = cart.Sum(c => c.Quantity * c.Price),
                OrderDetails = cart.Select(c => new OrderDetail
                {
                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    Quantity = c.Quantity,
                    Price = c.Price
                }).ToList()
            };

            await _dbContext.Orders.InsertOneAsync(order);
            string subject = "Xác nhận đơn hàng thành công";
            string message = $"<h3>Xin chào {order.FirstName} {order.LastName},</h3>" +
                             $"<p>Cảm ơn bạn đã đặt hàng tại cửa hàng của chúng tôi.</p>" +
                             $"<p>Tổng số tiền: <strong>{order.TotalAmount:N0} VND</strong></p>" +
                             "<p>Chúng tôi sẽ xử lý đơn hàng của bạn trong thời gian sớm nhất.</p>" +
                             "<p>Trân trọng,</p><p>Đội ngũ hỗ trợ</p>";

            await _emailSender.SendEmailAsync(order.Email, subject, message);
            // Xoá giỏ hàng sau khi đặt hàng
            HttpContext.Session.Remove("Cart");

            TempData["success"] = "Đặt hàng thành công!";
            return RedirectToAction("Index", "Cart");
        }



        //[Authorize]
        //public IActionResult PaymentCallBack()
        //{
        //    var response = _vnPayService.PaymentExcute(Request.Query);
        //    if (response == null || response.VnPayResponseCode != "00")
        //    {
        //        TempData["error"] = $"Lỗi thanh toán VnPay: {response.VnPayResponseCode}";
        //        return RedirectToAction("Index", "CheckOut");
        //    }

        //    //lưu vào database


        //    TempData["success"] = $"Thanh toán VnPay thành công: {response.VnPayResponseCode}";
        //    return RedirectToAction("Index", "Home");
        //}
        [Authorize]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnPayService.PaymentExcute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["error"] = $"Lỗi thanh toán VnPay: {response?.VnPayResponseCode ?? "Không có phản hồi"}";
                return RedirectToAction("Index", "CheckOut");
            }

            // Lấy thông tin giỏ hàng từ session
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Lấy thông tin từ form checkout (cần lưu trữ tạm trong session)
            var checkoutInfo = HttpContext.Session.GetJson<CheckoutViewModel>("CheckoutInfo") ?? new CheckoutViewModel();

            // Tạo đơn hàng mới
            var order = new Order
            {
                FirstName = checkoutInfo.FirstName,
                LastName = checkoutInfo.LastName,
                Country = checkoutInfo.Country,
                Address = checkoutInfo.Address,
                TownCity = checkoutInfo.City,
                State = checkoutInfo.State,
                ZipCode = checkoutInfo.ZipCode,
                Phone = checkoutInfo.Phone,
                Email = checkoutInfo.Email,
                Note = checkoutInfo.Note,
                Status = "Paid",
                TotalAmount = cart.Sum(c => c.Quantity * c.Price), // Lấy tổng giá trị thực từ giỏ hàng
                OrderDetails = cart.Select(c => new OrderDetail
                {
                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    Quantity = c.Quantity,
                    Price = c.Price
                }).ToList(),
           
                OrderDate = DateTime.UtcNow
            };

            // Lưu vào database
            await _dbContext.Orders.InsertOneAsync(order);

            //Gửi email xác nhận
            string subject = "Xác nhận đơn hàng thành công";
            string message = $"<h3>Xin chào {order.FirstName} {order.LastName},</h3>" +
                            $"<p>Cảm ơn bạn đã thanh toán qua VNPay.</p>" +

                            $"<p>Tổng số tiền: <strong>{order.TotalAmount:N0} VND</strong></p>" +
                            "<p>Chúng tôi sẽ xử lý đơn hàng của bạn trong thời gian sớm nhất.</p>" +
                            "<p>Trân trọng,</p><p>Đội ngũ hỗ trợ</p>";

            await _emailSender.SendEmailAsync(order.Email, subject, message);

            // Xóa giỏ hàng và thông tin checkout sau khi xử lý
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("CheckoutInfo");

            TempData["success"] = $"Thanh toán VnPay thành công. Mã giao dịch: {response.TransactionId}";
            return RedirectToAction("Index", "Home");
        }
    }
}
