using BTL_Mongodb.Models.BusinessModels.ImplementModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BTL_Mongodb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        // GET: /Admin/Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Admin/Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var (user, roles) = await _authService.Login(username, password);

            if (user != null && roles != null) // chỉ cho admin đăng nhập vào admin area
            {
                // Tạo claim để lưu thông tin đăng nhập
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? "")
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                HttpContext.Session.SetString("UserId", user.Id);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                TempData["success"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            TempData["error"] = "Tên đăng nhập hoặc mật khẩu không đúng hoặc bạn không có quyền truy cập.";
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng hoặc bạn không có quyền truy cập.");
            return View();
        }







        // GET: /Admin/Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("UserId");
            //await HttpContext.SignOutAsync();

            HttpContext.Session.Clear();
            TempData["success"] = "Đăng xuất thành công!";
            return RedirectToAction("Login");
        }

        // Nếu muốn có trang AccessDenied cho admin
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
