using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace Dswi_Proyecto_Topico.Controllers
{
    [Authorize(Roles = "Alumno")]
    public class AlumnoController : Controller
    {

        private readonly HistorialReporteRepository historialReporteRepository;
        private readonly ReporteAtencionRepository reporteAtencionRepository;
        public AlumnoController(HistorialReporteRepository historialReporteRepository, ReporteAtencionRepository reporteAtencionRepository)
        {
            this.historialReporteRepository = historialReporteRepository;
            this.reporteAtencionRepository = reporteAtencionRepository;
        }
        public IActionResult Index()
        {
            
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
        [HttpGet]
        public async Task<IActionResult> HistorialReportesAlumno()
        {
            var codigoAlumno = HttpContext.Session.GetString("CodAlumno");

            if (string.IsNullOrEmpty(codigoAlumno))
            {
                return RedirectToAction("LoginAlumno", "Auth");
            }

            var lista = await historialReporteRepository.ListarHistorialReporteAsync(codigoAlumno, null, null);

            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> VerDetalleReporteAlumno(int atencionId)
        {
            var codigoAlumno = HttpContext.Session.GetString("CodAlumno");

            var reporte = await historialReporteRepository.ObtenerReporteDetalleAsync(atencionId);

            if (reporte == null || reporte.Codigo != codigoAlumno)
            {
                TempData["Error"] = "Acceso no autorizado.";
                return RedirectToAction(nameof(HistorialReportesAlumno));
            }

            // Indica que es alumno, no enfermera
            reporte.EsVistaGeneracion = false;

            return View("~/Views/Enfermera/VistaPreviaReporte.cshtml", reporte);
        }

       
        [HttpGet]
        public async Task<IActionResult> DescargarReportePdfAlumno(int atencionId)
        {
            var codigoAlumno = HttpContext.Session.GetString("CodAlumno");

            var reporte = await reporteAtencionRepository.ObtenerReporteParaPdfAsync(atencionId);

            if (reporte == null || reporte.Codigo != codigoAlumno)
            {
                TempData["Error"] = "No se puede descargar el reporte.";
                return RedirectToAction(nameof(HistorialReportesAlumno));
            }

            return new ViewAsPdf("~/Views/Enfermera/ReporteAtencionPdf.cshtml", reporte)
            {
                FileName = $"ReporteAtencion_{reporte.Codigo}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }


        [HttpGet]


        public IActionResult Notificaciones()
        {
            return View();
        }
    }
}
