using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dswi_Proyecto_Topico.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<ApplicationUser> userManager;

        public BaseController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = userManager.GetUserAsync(User).Result;

                if (user != null &&
                    user.DebeCambiarPassword &&
                    context.ActionDescriptor.RouteValues["action"] != "CambiarPassword")
                {
                    context.Result = RedirectToAction("CambiarPassword", "Account");
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
