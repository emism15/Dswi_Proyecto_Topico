using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Helpers;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Dswi_Proyecto_Topico.Controllers
{
    public class EnfermeraController : Controller
    {

        private readonly EnfermeraRepository _repo;

        public EnfermeraController(EnfermeraRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Seguridad básica
            if (!HttpContext.Session.IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            // Validar rol
            if (HttpContext.Session.GetNombreRol() != "Enfermera")
                return RedirectToAction("Login", "Auth");

            DashboardEnfermeraViewModel vm = await _repo.ObtenerDashboardAsync();

            return View(vm);
        }

    }
}

