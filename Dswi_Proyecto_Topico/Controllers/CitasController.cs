using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Dswi_Proyecto_Topico.Controllers
{
    public class CitasController : Controller
    {

        private readonly CitaRepository repo;

        public CitasController(IConfiguration config)
        {
            repo = new CitaRepository(config);
        }

        // GET: Citas
        public async Task<IActionResult> Index(string estado)
        {
            var vm = new CitasVM();

            vm.Citas = await repo.ListarAsync(estado);

            var cont = await repo.ContarAsync();
            vm.Total = vm.Citas.Count;
            vm.Pendientes = cont.ContainsKey("Pendiente") ? cont["Pendiente"] : 0;
            vm.Atendidas = cont.ContainsKey("Atendida") ? cont["Atendida"] : 0;
            vm.Canceladas = cont.ContainsKey("Cancelada") ? cont["Cancelada"] : 0;

            vm.EstadoSeleccionado = estado;

            return View(vm);
        }


        // POST: Citas/CambiarEstado
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, string estado)
        {
            await repo.CambiarEstadoAsync(id, estado);
            return RedirectToAction("Index");
        }

        // GET: Citas/Citas
        public async Task<IActionResult> Citas(string estado)
        {
            ViewBag.EstadoFiltro = estado;

            var citas = await repo.ObtenerCitasAsync(estado);

            return View(citas);
        }

        // GET: Citas/RegistrarCita
        public IActionResult RegistrarCita()
        {
            return View();
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> RegistrarCita(RegistrarCitaViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            bool ok = await repo.RegistrarCitaAsync(vm);

            if (!ok)
            {
                ModelState.AddModelError("", "Ya existe una cita en ese horario");
                return View(vm);
            }

            return RedirectToAction("Citas");
        }


    }

}

