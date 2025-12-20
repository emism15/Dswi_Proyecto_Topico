using Microsoft.AspNetCore.Mvc;
using TopicoMedico.Filters;
using TopicoMedico.Helpers;
using TopicoMedico.Models.Entities;
using TopicoMedico.Models.ViewModels;
using TopicoMedico.Services.Interfaces;

namespace TopicoMedico.Controllers
{
    // ========== PacienteController - Panel del Paciente ==========
    [AuthorizeRole("Paciente")]
    public class PacienteController : Controller
    {
        private readonly ICitaService _citaService;
        private readonly IRecetaService _recetaService;
        private readonly IAlertaService _alertaService;

        public PacienteController(
            ICitaService citaService,
            IRecetaService recetaService,
            IAlertaService alertaService)
        {
            _citaService = citaService;
            _recetaService = recetaService;
            _alertaService = alertaService;
        }

        // GET: Paciente/Index - Dashboard
        public async Task<IActionResult> Index()
        {
            var pacienteId = HttpContext.Session.GetUsuarioId().Value;
            var citasPendientes = await _citaService.ObtenerPorPacienteAsync(pacienteId, "Pendiente");
            var citasAtendidas = await _citaService.ObtenerPorPacienteAsync(pacienteId, "Atendida");
            var recetas = await _recetaService.ObtenerPorPacienteAsync(pacienteId);

            var viewModel = new DashboardPacienteViewModel
            {
                CitasPendientes = citasPendientes.Count,
                CitasAtendidas = citasAtendidas.Count,
                RecetasVigentes = recetas.Count(r => r.Estado == "Vigente"),
                ProximaCita = citasPendientes.OrderBy(c => c.FechaCita).FirstOrDefault(),
                HistorialCitas = citasAtendidas.Take(5).ToList(),
                RecetasRecientes = recetas.Take(5).ToList()
            };

            return View(viewModel);
        }

        // GET: Paciente/MisCitas
        public async Task<IActionResult> MisCitas()
        {
            var pacienteId = HttpContext.Session.GetUsuarioId().Value;
            var citas = await _citaService.ObtenerPorPacienteAsync(pacienteId);
            return View(citas);
        }

        // GET: Paciente/DetalleCita/5
        public async Task<IActionResult> DetalleCita(int id)
        {
            var cita = await _citaService.ObtenerPorIdAsync(id);

            // Verificar que la cita pertenezca al paciente actual
            var pacienteId = HttpContext.Session.GetUsuarioId().Value;
            if (cita == null || cita.PacienteId != pacienteId)
                return NotFound();

            return View(cita);
        }

        // GET: Paciente/MisRecetas
        public async Task<IActionResult> MisRecetas()
        {
            var pacienteId = HttpContext.Session.GetUsuarioId().Value;
            var recetas = await _recetaService.ObtenerPorPacienteAsync(pacienteId);
            return View(recetas);
        }

        // GET: Paciente/DetalleReceta/5
        public async Task<IActionResult> DetalleReceta(int id)
        {
            var receta = await _recetaService.ObtenerPorIdAsync(id);

            // Verificar que la receta pertenezca al paciente actual
            var pacienteId = HttpContext.Session.GetUsuarioId().Value;
            if (receta == null || receta.PacienteId != pacienteId)
                return NotFound();

            ViewBag.Detalles = await _recetaService.ObtenerDetallesAsync(id);
            return View(receta);
        }

        // GET: Paciente/Alertas
        public async Task<IActionResult> Alertas()
        {
            var pacienteId = HttpContext.Session.GetUsuarioId().Value;
            var alertas = await _alertaService.ObtenerPorUsuarioAsync(pacienteId);
            return View(alertas);
        }

        // POST: Paciente/MarcarAlertaLeida/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarAlertaLeida(int id)
        {
            await _alertaService.MarcarComoLeidaAsync(id);
            return RedirectToAction(nameof(Alertas));
        }

        // GET: Paciente/SolicitarCita
        public IActionResult SolicitarCita()
        {
            var pacienteId = HttpContext.Session.GetUsuarioId().Value;
            var model = new CitaViewModel
            {
                PacienteId = pacienteId,
                FechaCita = DateTime.Now.AddDays(1)
            };
            return View(model);
        }

        // POST: Paciente/SolicitarCita
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarCita(CitaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pacienteId = HttpContext.Session.GetUsuarioId().Value;
                model.PacienteId = pacienteId;

                var resultado = await _citaService.CrearAsync(model, pacienteId);

                if (resultado)
                {
                    TempData["Mensaje"] = "Cita solicitada correctamente. Espere confirmación.";
                    return RedirectToAction(nameof(MisCitas));
                }
                else
                {
                    TempData["Error"] = "Error al solicitar la cita. Intente nuevamente.";
                }
            }

            return View(model);
        }

        // POST: Paciente/CancelarCita/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarCita(int id)
        {
            var cita = await _citaService.ObtenerPorIdAsync(id);
            var pacienteId = HttpContext.Session.GetUsuarioId().Value;

            if (cita == null || cita.PacienteId != pacienteId)
            {
                TempData["Error"] = "Cita no encontrada.";
                return RedirectToAction(nameof(MisCitas));
            }

            if (cita.EstadoCita != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden cancelar citas pendientes.";
                return RedirectToAction(nameof(MisCitas));
            }

            var resultado = await _citaService.CancelarAsync(id);

            if (resultado)
            {
                TempData["Mensaje"] = "Cita cancelada correctamente.";
            }
            else
            {
                TempData["Error"] = "Error al cancelar la cita.";
            }

            return RedirectToAction(nameof(MisCitas));
        }
    }
}