using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dswi_Proyecto_Topico.Controllers
{
    [Authorize(Roles = "Alumno")]
    public class AlumnoController : Controller
    {
        public IActionResult Index()
        {
            // ⚠️ Luego estos valores vendrán de la BD
            var model = new DashboardAlumnoViewModel
            {
                CitasPendientes = 2,
                CitasAtendidas = 5
            };

            return View(model);
        }

        public IActionResult MisCitas()
        {
            return View();
        }

        public IActionResult HistorialAtenciones()
        {
            return View();
        }

        public IActionResult Notificaciones()
        {
            return View();
        }
    }
}
