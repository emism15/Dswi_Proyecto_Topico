using Microsoft.AspNetCore.Mvc;
using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Models.ViewModels;

namespace Dswi_Proyecto_Topico.Controllers
{
    public class AlumnoController : Controller
    {
        private readonly AlumnoRepository _alumnoRepo;

        public AlumnoController(AlumnoRepository alumnoRepo)
        {
            _alumnoRepo = alumnoRepo;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Alumno alumno)
        {
            if (!ModelState.IsValid)
                return View(alumno);

            await _alumnoRepo.AgregarAlumnoAsync(alumno);
            TempData["Mensaje"] = "Alumno registrado correctamente";
            return RedirectToAction("Create", "Atencion"); // Volver a registrar atención
        }


}
}
