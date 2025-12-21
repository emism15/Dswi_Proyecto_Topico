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

        public async Task<IActionResult> Index()
        {
            int usuarioId = HttpContext.Session.GetInt32("UsuarioId").Value;

            int alumnoId = await alumnoRepo.ObtenerAlumnoIdPorUsuarioAsync(usuarioId);

            var citas = await citaRepo.ListarCitasPorAlumnoAsync(alumnoId);

            var model = new DashboardAlumnoViewModel
            {
                CitasPendientes = citas.Count(c => c.EstadoCita == "Pendiente"),
                CitasAtendidas = citas.Count(c => c.EstadoCita == "Atendida"),

                ProximaCita = citas
                    .Where(c => c.EstadoCita == "Pendiente" && c.FechaCita >= DateTime.Now)
                    .OrderBy(c => c.FechaCita)
                    .FirstOrDefault(),

                HistorialCitas = citas
                    .OrderByDescending(c => c.FechaCita)
                    .Take(5)
                    .ToList()
            };

            return View(model);
        }
    }
}
