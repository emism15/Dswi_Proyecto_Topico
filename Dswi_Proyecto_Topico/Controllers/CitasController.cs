using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Dswi_Proyecto_Topico.Controllers
{
    public class CitasController : Controller
    {

        private readonly CitaRepository citaRepo;
        private readonly AlumnoRepository alumnoRepo;

        public CitasController(AlumnoRepository alumrepo, CitaRepository repo) 
        {
            alumnoRepo = alumrepo;
            citaRepo = repo;

        }

        // GET: Citas
        public async Task<IActionResult> Index(string estado)
        {
            var vm = new CitasVM();

            vm.Citas = await citaRepo.ListarAsync(estado);

            var cont = await citaRepo.ContarAsync();
            vm.Total = vm.Citas.Count;
            vm.Pendientes = cont.ContainsKey("Pendiente") ? cont["Pendiente"] : 0;
            vm.Atendidas = cont.ContainsKey("Atendida") ? cont["Atendida"] : 0;
            vm.Canceladas = cont.ContainsKey("Cancelada") ? cont["Cancelada"] : 0;

            vm.EstadoSeleccionado = estado;

            return View(vm);
        }

        // GET: Detalle de cita
        [HttpGet]
        public async Task<IActionResult> DetalleCita(int id)
        {
            var cita = await citaRepo.ObtenerPorIdAsync(id);
            if (cita == null)
                return NotFound();

            return View(cita);
        }

        // POST: Actualizar estado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarEstado(int id, Cita cita)
        {
            await citaRepo.ActualizarEstadoAsync(id, cita.EstadoCita);
            TempData["MensajeExito"] = $"Estado de la cita #{id} actualizado a {cita.EstadoCita}.";
            return RedirectToAction(nameof(Citas));
        }





        //// POST: Citas/CambiarEstado
        //[HttpPost]
        //public async Task<IActionResult> CambiarEstado(int id, string estado)
        //{
        //    await citaRepo.CambiarEstadoAsync(id, estado);
        //    return RedirectToAction("Index");
        //}

        // GET: Citas/Citas
        public async Task<IActionResult> Citas(string estado)
        {
            ViewBag.EstadoFiltro = estado;

            var citas = await citaRepo.ObtenerCitasAsync(estado);

            return View(citas);
        }

        // GET
        [HttpGet]
        public IActionResult RegistrarCita()
        {
            var model = new RegistrarCitaViewModel
            {
                Fecha = DateTime.Now.Date,
                Hora = DateTime.Now.TimeOfDay,
                TipoCita = string.Empty,
                AlumnoEncontrado = false
            };
            return View(model);
        }

        // POST Buscar Alumno
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarAlumno(RegistrarCitaViewModel model)
        {
            if (!ModelState.IsValid)
                return View("RegistrarCita", model);

        
            var alumno = await alumnoRepo.BuscarAlumnoPorCodigoAsync(model.Codigo);

            if (alumno == null)
            {
                model.AlumnoEncontrado = false;
                ModelState.AddModelError("", "El código ingresado no existe en el sistema");
                return View("RegistrarCita", model);
            }

            model.AlumnoEncontrado = true;
            model.AlumnoId = alumno.AlumnoId;
            model.NombreCompleto = alumno.NombreCompleto;
            model.DNI = alumno.DNI;

            ModelState.Clear();
            return View("RegistrarCita", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarCita(RegistrarCitaViewModel model)
        {
            if (!model.AlumnoEncontrado || model.AlumnoId == 0)
            {
                ModelState.AddModelError("", "Debe buscar y confirmar un alumno antes de registrar la cita.");
                return View("RegistrarCita", model);
            }

            if (string.IsNullOrWhiteSpace(model.TipoCita))
            {
                ModelState.AddModelError("TipoCita", "Debe seleccionar un tipo de cita.");
            }

            if (!ModelState.IsValid)
            {
                return View("RegistrarCita", model);
            }

            try
            {
                int citaId = await citaRepo.RegistrarCitaAsync(model);

                if (citaId <= 0)
                {
                    ModelState.AddModelError("", "Ya existe una cita en ese horario.");
                    return View("RegistrarCita", model);
                }

                TempData["MensajeExito"] = $"Cita registrada correctamente (ID {citaId}).";
                return RedirectToAction(nameof(RegistrarCita)); // ← permanecer en el módulo de enfermería
            }
            catch (SqlException e)
            {
                ModelState.AddModelError("", e.Message);
                return View("RegistrarCita", model);
            }
        }


    
    }

}

