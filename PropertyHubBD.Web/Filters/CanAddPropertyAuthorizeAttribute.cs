using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PropertyHubBD.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace PropertyHubBD.Web.Filters
{
    /// <summary>
    /// Authorization attribute that allows Seller, Admin, and SuperAdmin to add properties.
    /// </summary>
    public class CanAddPropertyAuthorizeAttribute : Attribute, IAuthorizationFilter
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
            
            // Allow Seller, Admin, and SuperAdmin to add properties
            if (currentUser == null || (currentUser.UserType != "Admin" && currentUser.UserType != "Seller"))
            {
                var tempData = context.HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory>();
                if (tempData != null)
                {
                    var tempDataDict = tempData.GetTempData(context.HttpContext);
                    tempDataDict["Error"] = "Access denied. You don't have permission to add properties.";
                }
                
                context.Result = new RedirectToActionResult("Index", "Dashboard", new { area = "Admin" });
                return;
            }
        }
    }
}
