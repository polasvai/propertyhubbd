using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PropertyHubBD.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace PropertyHubBD.Web.Filters
{
    /// <summary>
    /// Authorization attribute that restricts access to SuperAdmin only.
    /// SuperAdmin is defined as a user with UserType = "Admin" and email = "superadmin@propertyhubbd.com"
    /// </summary>
    public class SuperAdminAuthorizeAttribute : Attribute, IAuthorizationFilter
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
            
            // Check if user is SuperAdmin
            if (currentUser == null || !currentUser.IsSuperAdmin)
            {
                // Set TempData for error message
                var tempData = context.HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory>();
                if (tempData != null)
                {
                    var tempDataDict = tempData.GetTempData(context.HttpContext);
                    tempDataDict["Error"] = "Access denied. This action requires SuperAdmin privileges.";
                }
                
                context.Result = new RedirectToActionResult("Index", "Dashboard", new { area = "Admin" });
                return;
            }
        }
    }
}
