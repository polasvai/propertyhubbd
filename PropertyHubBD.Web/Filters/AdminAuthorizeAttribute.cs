using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PropertyHubBD.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace PropertyHubBD.Web.Filters
{
    /// <summary>
    /// Authorization attribute that allows Admin and Seller users to access the admin dashboard.
    /// </summary>
    public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new RedirectToPageResult("/Account/Login", new { area = "Identity" });
                return;
            }

            // Get the UserManager from DI
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
            if (userManager == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Get the current user
            var currentUser = userManager.GetUserAsync(user).Result;
            
            // Allow Admin and Seller users to access
            if (currentUser == null || (currentUser.UserType != "Admin" && currentUser.UserType != "Seller"))
            {
                context.Result = new RedirectToActionResult("Index", "Home", new { area = "" });
                return;
            }
        }
    }
}
