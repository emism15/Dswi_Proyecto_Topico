using Microsoft.AspNetCore.Mvc;
using TopicoMedico.Data;
using TopicoMedico.Models.ViewModels;

namespace TopicoMedico.Controllers
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
