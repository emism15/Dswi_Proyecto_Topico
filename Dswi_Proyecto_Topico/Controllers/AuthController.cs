using Dswi_Proyecto_Topico.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dswi_Proyecto_Topico.Helpers;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Identity;


namespace Dswi_Proyecto_Topico.Controllers
{
    // ========== AuthController - Login y Autenticación ==========
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthController(/*AuthService authService*/ AuthRepository authRepo, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        
        {
            _authService = authService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("Index", GetDashboardByRole());
            }
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _authService.AutenticarAsync(model.NombreUsuario, model.Contraseña);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View(model);
            }

            // Guardar datos en sesión
            HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
            HttpContext.Session.SetInt32("RolId", usuario.RolId);
            HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
            HttpContext.Session.SetString("NombreCompleto", usuario.NombreCompleto);
            HttpContext.Session.SetString("NombreRol", usuario.Rol.NombreRol);

            // Si debe cambiar contraseña
            if (usuario.DebecambiarContraseña)
            {
                TempData["Mensaje"] = "Por seguridad, debe cambiar su contraseña";
                return RedirectToAction("CambiarContraseña");
            }

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContraseña(CambioContraseñaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuarioId = HttpContext.Session.GetUsuarioId().Value;

            var resultado = await _authService.CambiarContraseñaAsync(
                usuarioId,
                model.ContraseñaActual,
                model.ContraseñaNueva
            );

            if (!resultado)
            {
                ModelState.AddModelError("", "La contraseña actual es incorrecta");
                return View(model);
            }

            TempData["Success"] = "Contraseña actualizada correctamente";
            return RedirectToAction("Index", GetDashboardByRole());
        }

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
            <textarea style='width:100%; height:80px; font-family:monospace;'>{hash}</textarea>
        ", "text/html");
        }


        [HttpGet]
        public IActionResult LoginAlumno()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAlumno(LoginAlumnoModel model)
           {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByNameAsync(model.Usuario);

            if (user == null)
            {
                ModelState.AddModelError("", "Usuario no existe");
                return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                false,
                false
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Credenciales incorrectas");
                return View(model);
            }
            HttpContext.Session.SetString("CodAlumno", user.Codigo);

            if (user.DebeCambiarPassword)
            {
                return RedirectToAction("CambiarPassword");
            }

            return RedirectToAction("Index", "Alumno");
        }

        [HttpGet]
        public IActionResult CambiarPassword()
        {
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("LoginAlumno");
            }

            var result = await userManager.ChangePasswordAsync(
                user,
                model.PasswordActual,
                model.PasswordNueva
            );

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            user.DebeCambiarPassword = false;
            await userManager.UpdateAsync(user);

            // 🔒 Cerrar sesión
            await signInManager.SignOutAsync();

            // Mensaje para el login
            TempData["Mensaje"] = "Contraseña cambiada exitosamente. Inicie sesión con su nueva contraseña.";

            // 🔁 Volver al login
            return RedirectToAction("LoginAlumno", "Auth");
        }
    }
}

