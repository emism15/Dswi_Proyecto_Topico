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
        public IActionResult Index(string estado)
        {
            var vm = new CitasVM();

            vm.Citas = repo.Listar(estado);

            var cont = repo.Contar();
            vm.Total = vm.Citas.Count;
            vm.Pendientes = cont.ContainsKey("Pendiente") ? cont["Pendiente"] : 0;
            vm.Atendidas = cont.ContainsKey("Atendida") ? cont["Atendida"] : 0;
            vm.Canceladas = cont.ContainsKey("Cancelada") ? cont["Cancelada"] : 0;

            vm.EstadoSeleccionado = estado;

            return View(vm);
        }
        // GET: Citas/NuevaCita
        public IActionResult NuevaCita()
        {
            return View();
        }

        // POST: Citas/NuevaCita
        [HttpPost]
        public IActionResult NuevaCita(Cita c)
        {
            repo.Registrar(c);
            return RedirectToAction("Index");
        }


        // POST: Citas/CambiarEstado
        public IActionResult CambiarEstado(int id, string estado)
        {
            repo.CambiarEstado(id, estado);
            return RedirectToAction("Index");
        }

        //
        public IActionResult Citas(string estado)
        {
            ViewBag.EstadoFiltro = estado;

            var citas = repo.ObtenerCitas(estado);

            return View(citas);
        }





    }
}
