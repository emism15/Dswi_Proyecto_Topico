using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Dswi_Proyecto_Topico.Controllers
{
    public class EnfermeraController : Controller
    {
        private readonly AlumnoRepository alumnoRepository;
         private readonly AtencionRepository atencionRepository;

        public EnfermeraController(AlumnoRepository alumnoRepository, AtencionRepository atencionRepository)
        {
            this.alumnoRepository = alumnoRepository;
            this.atencionRepository = atencionRepository;
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

        [HttpGet]
        public IActionResult RegistrarAtencion()
        {
            var model = new AtencionModel
            {
                FechaAtencion = DateTime.Now.Date,
                HoraAtencion = DateTime.Now.TimeOfDay
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarAlumno(AtencionModel model)
        {
            if(!ModelState.IsValid)
                return View("RegistrarAtencion", model);

            model.Codigo = model.Codigo.Trim();

            var alumno = await alumnoRepository.BuscarAlumnoPorCodigoAsync(model.Codigo);
                
                if(alumno == null)
                {
                    model.AlumnoEncontrado = false;
                    ModelState.AddModelError("", "Alumno no registrado");
                    return View("RegistrarAtencion", model);
                }

            ModelState.Clear();

            return View("RegistrarAtencion", alumno);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> RegistrarAtencion(AtencionModel model)
        {
            if (!model.AlumnoEncontrado)
            {
                ModelState.AddModelError("", "Debe buscar y confirmar un alumno antes de registrar la atención.");
                return View("RegistrarAtencion", model);
            }
            // Validaciones
            if (string.IsNullOrWhiteSpace(model.DetallesClinicos))
            {
                ModelState.AddModelError("DetallesClinicos", "Los detalles clínicos son obligatorios");
            }
            else if (model.DetallesClinicos.Length > 500)
            {
                ModelState.AddModelError("DetallesClinicos", "Detalles clínicos no pueden superar 500 caracteres");
            }

            if (string.IsNullOrWhiteSpace(model.DiagnosticoPreliminar))
            {
                ModelState.AddModelError("DiagnosticoPreliminar", "El diagnóstico preliminar es obligatorio");
            }
            else if (model.DiagnosticoPreliminar.Length > 250)
            {
                ModelState.AddModelError("DiagnosticoPreliminar", "El diagnóstico preliminar no puede superar 250 caracteres");
            }

            if (!ModelState.IsValid)
            {
                return View("RegistrarAtencion", model);

            }

            await atencionRepository.RegistrarAtencionAsync(model);

            TempData["MensajeExito"] = "Atención registrada correctamente.";

            return RedirectToAction("RegistrarAtencion");
        }
        
    }
}
