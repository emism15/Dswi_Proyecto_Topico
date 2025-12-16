using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Dswi_Proyecto_Topico.Controllers
{
    public class EnfermeraController : Controller
    {
        private readonly AlumnoRepository alumnoRepository;

        public EnfermeraController(AlumnoRepository alumnoRepository)
        {
            this.alumnoRepository = alumnoRepository;
        }

        [HttpGet]
        public IActionResult RegistrarAlumno()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarAlumno(AlumnoModel alumno)
        {
            if(!ModelState.IsValid)
            {
                return View(alumno);
            }
            if(await alumnoRepository.ExisteCodigoAsync(alumno.Codigo))
            {
                ModelState.AddModelError(nameof(alumno.Codigo), "El código ya está registrado");
                return View(alumno);
            }

            await alumnoRepository.AgregarAlumnoAsync(alumno);
            TempData["Mensaje"] = "Alumno registrado correctamente";
            return RedirectToAction(nameof(RegistrarAtencion));
        }

        public IActionResult RegistrarAtencion()
        {
            return View();
        }
    }
}
