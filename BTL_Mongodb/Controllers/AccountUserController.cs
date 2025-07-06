using BTL_Mongodb.Models;
using BTL_Mongodb.Models.BusinessModels.ImplementModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BTL_Mongodb.Models.ViewModel;
using Microsoft.AspNetCore.Authentication.Google;

namespace BTL_Mongodb.Controllers
{
    public class AccountUserController : Controller
    {
        private readonly AuthService _authService;

        public AccountUserController(AuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _authService.Register(model.Username, model.Email, model.Password))
            {
                TempData["success"] = "Đăng ký thành công!";
                return RedirectToAction("Login");
            }
    

            ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại.");
            return View(model);
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model); // Giữ lại lỗi hiển thị

            var result = await _authService.Login(model.Username, model.Password);
            if (result.user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.user.Id),
            new Claim(ClaimTypes.Name, result.user.Username),
            new Claim(ClaimTypes.Email, result.user.Email ?? "")
        };

                foreach (var role in result.roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                HttpContext.Session.SetString("UserId", result.user.Id);
                TempData["success"] = "Đăng nhập thành công!";

                return RedirectToAction("Index", "Home");
            }
            TempData["error"] = "Đăng nhập thất bại!";

            ModelState.AddModelError("", "Đăng nhập thất bại. Sai thông tin!");
            return View(model);
        }



        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("UserId");
            await HttpContext.SignOutAsync();
            //HttpContext.Session.Clear();
            TempData["success"] = "Đăng xuất thành công!";
            return RedirectToAction("Login");
        }

        //public async Task LoginByGoogle()
        //{
        //    await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
        //        new AuthenticationProperties
        //        {
        //            RedirectUri = Url.Action("GoogleReponse")
        //        });
        //}

        public async Task LoginByGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleReponse")
            };

            properties.Items["prompt"] = "select_account"; // Đây là phần quan trọng

            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
        }

        public async Task<IActionResult> GoogleReponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return RedirectToAction("Login", "AccountUser");
            }

            var principal = result.Principal;

            // Lấy email và tên từ claim
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var name = principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                TempData["error"] = "Không lấy được email từ Google.";
                return RedirectToAction("Login", "AccountUser");
            }

            // Kiểm tra trong database
            var user = await _authService.GetUserByEmailAsync(email);
            if (user == null)
            {
                // Nếu chưa có -> tạo mới user với password mặc định
                var registerSuccess = await _authService.Register(name, email, "123456");
                if (!registerSuccess)
                {
                    TempData["error"] = "Đăng ký tài khoản từ Google thất bại!";
                    return RedirectToAction("Login");
                }

                user = await _authService.GetUserByEmailAsync(email);
            }

            // Lấy vai trò
            var roles = await _authService.GetRolesForUserAsync(user.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("LoginProvider", "Google") // Đánh dấu là đăng nhập qua Google
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var newPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, newPrincipal);

            HttpContext.Session.SetString("UserId", user.Id);
            TempData["success"] = "Đăng nhập Google thành công!";

            return RedirectToAction("Index", "Home");
            //return Json(claims);
        }

    }
}
