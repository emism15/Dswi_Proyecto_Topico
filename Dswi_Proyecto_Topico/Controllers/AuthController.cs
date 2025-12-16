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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

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

            HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
            HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
            HttpContext.Session.SetString("NombreRol", usuario.Rol.NombreRol);

            if (usuario.DebecambiarContraseña)
                return RedirectToAction("CambiarContraseña");

            return RedirectToAction("Index", GetDashboardByRole());
        }



        [HttpGet]
        public IActionResult CambiarContraseña()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CambiarContraseña(CambioContraseñaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int usuarioId = HttpContext.Session.GetUsuarioId().Value;

            bool ok = await _authRepo.CambiarContraseñaAsync(
                usuarioId, model.ContraseñaActual, model.ContraseñaNueva);

            if (!ok)
            {
                ModelState.AddModelError("", "Contraseña actual incorrecta");
                return View(model);
            }

            return RedirectToAction("Index", GetDashboardByRole());
        }

        private string GetDashboardByRole()
        {
            var rol = HttpContext.Session.GetNombreRol();

            return rol switch
            {
                "Administrador" => "Admin",
                "Enfermera" => "Enfermera",
                "Paciente" => "Paciente",
                _ => "Home"
            };
        }


    }
}
