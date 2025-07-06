using BTL_Mongodb.Models.BusinessModels.ImplementModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BTL_Mongodb.Models.Atributes
{
    public class AuthorizePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _module;
        private readonly string _action;

        public AuthorizePermissionAttribute(string module, string action)
        {
            _module = module;
            _action = action;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var userId = httpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                // Redirect về login trong Admin area
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "Admin" });
                return;
            }

            var authService = context.HttpContext.RequestServices.GetRequiredService<AuthService>();
            bool hasPermission = await authService.HasPermission(userId, _module, _action);

            if (!hasPermission)
            {
                // Redirect về trang AccessDenied trong Admin area
                context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "Admin" });
            }
        }

    }
}
