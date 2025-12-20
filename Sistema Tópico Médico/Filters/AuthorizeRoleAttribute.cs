using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TopicoMedico.Helpers;

namespace TopicoMedico.Filters
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            if (!session.IsAuthenticated())
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            var rolUsuario = session.GetNombreRol();
            if (_roles != null && _roles.Length > 0 && !_roles.Contains(rolUsuario))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}