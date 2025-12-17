using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Helpers;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Dswi_Proyecto_Topico.Controllers
{
    // ========== AuthController - Login y Autenticación ==========
    public class AuthController : Controller
    {
        private readonly AuthRepository _authRepo;

        public AuthController(AuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        //GET: Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir al dashboard correspondiente
            if (HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("Index", GetDashboardByRole());
            }
            return View();
        }

        //POST: Auth/Login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _authRepo.AutenticarAsync(
                model.NombreUsuario, model.Contraseña);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View(model);
            }

            // Guardar información en sesión
            HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
            HttpContext.Session.SetInt32("RolId", usuario.RolId);
            HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
            HttpContext.Session.SetString("NombreCompleto", usuario.NombreCompleto);
            HttpContext.Session.SetString("NombreRol", usuario.Rol.NombreRol);

            // Si debe cambiar contraseña
            if (usuario.DebecambiarContraseña)
                return RedirectToAction("CambiarContraseña");

            // Redirigir según rol
            return RedirectToAction("Index", GetDashboardByRole());
        }


        // GET: Auth/CambiarContraseña
        [HttpGet]
        public IActionResult CambiarContraseña()
        {
            if (!HttpContext.Session.IsAuthenticated())
                return RedirectToAction("Login");
            return View();
        }

        // POST: Auth/CambiarContraseña
        [HttpPost]
        public async Task<IActionResult> CambiarContraseña(CambioContraseñaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int usuarioId = HttpContext.Session.GetUsuarioId().Value;

            var resultado = await _authRepo.CambiarContraseñaAsync(
                 usuarioId,
                model.ContraseñaActual,
                model.ContraseñaNueva);

            if (!resultado)
            {
                ModelState.AddModelError("", "La contraseña actual es incorrecta");
                return View(model);
            }

            TempData["Success"] = "Contraseña actualizada correctamente";
            return RedirectToAction("Index", GetDashboardByRole());
        }

        // POST: Auth/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string GetDashboardByRole()
        {
            var rol = HttpContext.Session.GetNombreRol();

            return rol switch
            {
                "Administrador" => "Admin",
                "Enfermera" => "Enfermera",
                "Paciente" => "Paciente",
                _ => "Auth"
            };
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GenerarHash(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Content("Uso: /Auth/GenerarHash?password=tucontraseña");
            }

            var hash = PasswordHelper.HashPassword(password);
            return Content($@"
            <h2>Generador de Hash SHA256</h2>
            <p><strong>Contraseña:</strong> {password}</p>
            <p><strong>Hash SHA256:</strong></p>
            <textarea style='width:100%; height:100px; font-family:monospace;'>{hash}</textarea>
        ", "text/html");
        }

    }
}
