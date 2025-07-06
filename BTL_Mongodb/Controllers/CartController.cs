using BTL_Mongodb.Models;
using BTL_Mongodb.Models.ViewModel;
using BTL_Mongodb.Models.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BTL_Mongodb.Models.BusinessModels;
using Microsoft.AspNetCore.Authorization;
using BTL_Mongodb.Models.BusinessModels.ImplementModels;

namespace BTL_Mongodb.Controllers
{
    public class CartController : Controller
    {
        private readonly MongoDbContext _dbContext;
        private readonly IVnPayService _vnPayService;

        public CartController(MongoDbContext dbContext, IVnPayService vnPayService)
        {
            _dbContext = dbContext;
            _vnPayService = vnPayService;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var cartVM = new CartItemViewModel
            {
                CartItems = cart
            };
            return View(cartVM);
        }

        public async Task<IActionResult> Add(string id, int? quantity)
        {
            quantity ??= 1;

            var product = await _dbContext.Products.Find(p => p.Id == id).FirstOrDefaultAsync();

            if (product == null)
            {
                TempData["error"] = "Sản phẩm không tồn tại";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem == null)
            {
                cart.Add(new CartItemModel(product) { Quantity = quantity.Value });
            }
            else
            {
                cartItem.Quantity += quantity.Value;
            }

            HttpContext.Session.SetJson("Cart", cart);
            TempData["success"] = $"Đã thêm {quantity.Value} sản phẩm vào giỏ hàng.";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult Increase(string id)
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                cartItem.Quantity++;
                HttpContext.Session.SetJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Decrease(string id)
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    cart.Remove(cartItem);
                }
            }

            if (cart.Count == 0)
                HttpContext.Session.Remove("Cart");
            else
                HttpContext.Session.SetJson("Cart", cart);

            return RedirectToAction("Index");
        }

        public IActionResult Remove(string id)
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            cart.RemoveAll(x => x.ProductId == id);

            if (cart.Count == 0)
                HttpContext.Session.Remove("Cart");
            else
                HttpContext.Session.SetJson("Cart", cart);

            TempData["success"] = "Đã xoá sản phẩm khỏi giỏ hàng";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult UpdateQuantity(string id, int quantity)
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity <= 0 ? 1 : quantity;
                HttpContext.Session.SetJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }


        public IActionResult Checkout()
        {
            return View();
        }
        //[Authorize]
        //public IActionResult PaymentCallBack()
        //{
        //    var response = _vnPayService.PaymentExcute(Request.Query);
        //    if(response == null || response.VnPayResponseCode != "00")
        //    {
        //        TempData["error"] = $"Lỗi thanh toán VnPay: {response.VnPayResponseCode}";
        //        return RedirectToAction("Index", "CheckOut");
        //    }
        //    TempData["error"] = $"Thanh toán VnPay thành công: {response.VnPayResponseCode}";

        //    return View();
        //}
    }
}
