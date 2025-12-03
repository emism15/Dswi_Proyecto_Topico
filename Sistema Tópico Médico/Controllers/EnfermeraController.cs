using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopicoMedico.Data;
using TopicoMedico.Filters;
using TopicoMedico.Helpers;
using TopicoMedico.Models;
using TopicoMedico.Models.ViewModels;
using TopicoMedico.Services;
using TopicoMedico.Services.Interfaces;

namespace TopicoMedico.Controllers
{

    // ========== EnfermeraController - Panel de Enfermera ==========
    [AuthorizeRole("Enfermera")]
    public class EnfermeraController : Controller
    {
        private readonly ICitaService _citaService;
        private readonly IRecetaService _recetaService;
        private readonly IProductoService _productoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IAlertaService _alertaService;
        private readonly TopicoDbContext _context;

        public EnfermeraController(
            ICitaService citaService,
            IRecetaService recetaService,
            IProductoService productoService,
            IUsuarioService usuarioService,
            IAlertaService alertaService,
            TopicoDbContext context)
        {
            _citaService = citaService;
            _recetaService = recetaService;
            _productoService = productoService;
            _usuarioService = usuarioService;
            _alertaService = alertaService;
            _context = context;
        }

        // GET: Enfermera/Index - Dashboard
        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Now.Date;
            var mañana = hoy.AddDays(1);

            var vm = new DashboardEnfermeraViewModel();

            // Contadores
            vm.CitasHoy = _context.Citas.Count(c => c.FechaCita >= hoy && c.FechaCita < mañana);
            vm.CitasPendientes = _context.Citas.Count(c => c.EstadoCita == "Pendiente");
            vm.CitasAtendidasHoy = _context.Citas.Count(c => c.EstadoCita == "Atendida" && c.FechaAtencion >= hoy && c.FechaAtencion < mañana);

            // En tu caso "PacientesAtendidosHoy" y "CitasAtendidasHoy" pueden ser lo mismo,
            // pero lo dejamos separados por claridad (ajusta si necesitas otra lógica)
            vm.PacientesAtendidosHoy = vm.CitasAtendidasHoy;

            // Próximas citas (ej: próximos 5)
            vm.ProximasCitas = await _citaService.ObtenerProximasAsync(5);

            // Alertas para el rol enfermera
            vm.Alertas = await _alertaService.ObtenerPorRolAsync("Enfermera");

            // Llamada para generar alertas si corresponde (tuya ya la usabas)
            await _alertaService.GenerarAlertasCitasAsync();

            return View(vm);
        }


        // ===== GESTIÓN DE CITAS =====

        // GET: Enfermera/Citas
        public async Task<IActionResult> Citas(string estado = null)
        {
            ViewBag.EstadoFiltro = estado;
            var citas = await _citaService.ObtenerTodasAsync();

            if (!string.IsNullOrEmpty(estado))
                citas = citas.Where(c => c.EstadoCita == estado).ToList();

            return View(citas);
        }

        // GET: Enfermera/RegistrarCita
        public async Task<IActionResult> RegistrarCita()
        {
            var model = new CitaViewModel
            {
                PacientesDisponibles = await _usuarioService.ObtenerPorRolAsync(3), // Rol Paciente = 3
                FechaCita = DateTime.Now.AddHours(1)
            };
            return View(model);
        }

        // POST: Enfermera/RegistrarCita
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarCita(CitaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PacientesDisponibles = await _usuarioService.ObtenerPorRolAsync(3);
                return View(model);
            }

            var usuarioId = HttpContext.Session.GetUsuarioId().Value;
            var resultado = await _citaService.CrearAsync(model, usuarioId);

            if (resultado)
            {
                TempData["Success"] = "Cita registrada exitosamente";
                return RedirectToAction("Citas");
            }

            ModelState.AddModelError("", "Error al registrar la cita");
            model.PacientesDisponibles = await _usuarioService.ObtenerPorRolAsync(3);
            return View(model);
        }

        // GET: Enfermera/AtenderCita/5
        public async Task<IActionResult> AtenderCita(int id)
        {
            var cita = await _citaService.ObtenerPorIdAsync(id);
            if (cita == null || cita.EstadoCita != "Pendiente")
                return NotFound();

            var model = new AtenderCitaViewModel
            {
                CitaId = cita.CitaId,
                NombrePaciente = cita.Paciente.NombreCompleto,
                FechaCita = cita.FechaCita,
                MotivoConsulta = cita.MotivoConsulta
            };

            return View(model);
        }

        // POST: Enfermera/AtenderCita/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtenderCita(int id, AtenderCitaViewModel model)
        {
            if (id != model.CitaId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var enfermeraId = HttpContext.Session.GetUsuarioId().Value;
            var resultado = await _citaService.AtenderAsync(model, enfermeraId);

            if (resultado)
            {
                TempData["Success"] = "Cita atendida exitosamente";
                return RedirectToAction("CrearReceta", new { citaId = model.CitaId });
            }

            ModelState.AddModelError("", "Error al atender la cita");
            return View(model);
        }

        // ===== GESTIÓN DE RECETAS =====

        // GET: Enfermera/CrearReceta?citaId=5
        public async Task<IActionResult> CrearReceta(int citaId)
        {
            var cita = await _citaService.ObtenerPorIdAsync(citaId);
            if (cita == null || cita.EstadoCita != "Atendida")
                return NotFound();

            var model = new RecetaViewModel
            {
                CitaId = cita.CitaId,
                PacienteId = cita.PacienteId,
                NombrePaciente = cita.Paciente.NombreCompleto,
                Diagnostico = cita.Diagnostico,
                FechaCita = cita.FechaCita,
                ProductosDisponibles = await _productoService.ObtenerActivosAsync()
            };

            return View(model);
        }

        // POST: Enfermera/CrearReceta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearReceta(RecetaViewModel model)
        {
            if (!ModelState.IsValid || model.Detalles == null || !model.Detalles.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un medicamento o implemento");
                model.ProductosDisponibles = await _productoService.ObtenerActivosAsync();
                return View(model);
            }

            model.EnfermeraId = HttpContext.Session.GetUsuarioId().Value;
            var resultado = await _recetaService.CrearAsync(model);

            if (resultado)
            {
                TempData["Success"] = "Receta creada exitosamente. Stock actualizado.";
                return RedirectToAction("Citas");
            }

            ModelState.AddModelError("", "Error al crear la receta. Verifique el stock disponible.");
            model.ProductosDisponibles = await _productoService.ObtenerActivosAsync();
            return View(model);
        }

        // GET: Enfermera/Alertas
        public async Task<IActionResult> Alertas()
        {
            var alertas = await _alertaService.ObtenerPorRolAsync("Enfermera");
            return View(alertas);
        }
    }
}