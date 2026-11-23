using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CrimeGameStore.Web.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isAdmin = context.HttpContext.Session.GetString("PoirotAdminLoggedIn");
            if (isAdmin != "true")
            {
                context.Result = new RedirectToActionResult("Login", "Admin", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}