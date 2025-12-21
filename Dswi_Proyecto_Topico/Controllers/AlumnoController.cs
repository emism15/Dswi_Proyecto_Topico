using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Dswi_Proyecto_Topico.Controllers
{
    public class AlumnoController : Controller
    {
        private readonly CitaRepository citaRepo;
        private readonly AlumnoRepository alumnoRepo;

        public AlumnoController(CitaRepository citaRepo, AlumnoRepository alumnoRepo)
        {
            this.citaRepo = citaRepo;
            this.alumnoRepo = alumnoRepo;
        }

        // GET: Dashboard Alumno
        public async Task<IActionResult> Index()
        {
         // Usuario logueado
            int usuarioId = HttpContext.Session.GetInt32("UsuarioId").Value;

            //  Obtener AlumnoId DESDE BD
            int alumnoId = await alumnoRepo.ObtenerAlumnoIdPorUsuarioAsync(usuarioId);

            // Obtener próxima cita
            var proximaCita = await citaRepo.ObtenerProximaCitaAsync(alumnoId);

            // Armar ViewModel
            var vm = new DashboardAlumnoViewModel
            {
                CitasPendientes = await citaRepo.ContarCitasPorEstadoAsync(alumnoId, "Pendiente"),
                CitasAtendidas = await citaRepo.ContarCitasPorEstadoAsync(alumnoId, "Atendida"),
                ProximaCita = proximaCita,
                DiasParaProximaCita = proximaCita != null
                    ? (proximaCita.FechaCita.Date - DateTime.Now.Date).Days
                    : 0
            };

            return View(vm);
        }


        // GET: citas de alumno con filtro
        public async Task<IActionResult> Citas(string estado)
        {
            int usuarioId = HttpContext.Session.GetInt32("UsuarioId").Value;
            int alumnoId = await alumnoRepo.ObtenerAlumnoIdPorUsuarioAsync(usuarioId);

            var citas = await citaRepo.ListarCitasPorAlumnoAsync(alumnoId);

            // Aplicar filtro si existe
            if (!string.IsNullOrEmpty(estado))
            {
                citas = citas
                    .Where(c => c.EstadoCita.Equals(estado, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.EstadoFiltro = estado;

            var model = new CitasAlumnoViewModel
            {
                Pendientes = citas.Count(c => c.EstadoCita == "Pendiente"),
                Atendidas = citas.Count(c => c.EstadoCita == "Atendida"),
                Canceladas = citas.Count(c => c.EstadoCita == "Cancelada"),
                Citas = citas.OrderByDescending(c => c.FechaCita).ToList()
            };

            return View(model);
        }

       


    }
}
