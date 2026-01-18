using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PropertyHubBD.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace PropertyHubBD.Web.Filters
{
    /// <summary>
    /// Authorization attribute that allows Admin and SuperAdmin to edit properties.
    /// Sellers cannot edit properties.
    /// </summary>
    public class CanEditPropertyAuthorizeAttribute : Attribute, IAuthorizationFilter
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
            
            // Only Admin and SuperAdmin can edit properties
            if (currentUser == null || currentUser.UserType != "Admin")
            {
                var tempData = context.HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory>();
                if (tempData != null)
                {
                    var tempDataDict = tempData.GetTempData(context.HttpContext);
                    tempDataDict["Error"] = "Access denied. Only Admins can edit properties.";
                }
                
                context.Result = new RedirectToActionResult("Index", "Dashboard", new { area = "Admin" });
                return;
            }
        }
    }
}
