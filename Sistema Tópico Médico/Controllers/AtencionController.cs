using Microsoft.AspNetCore.Mvc;
using TopicoMedico.Data;
using TopicoMedico.Filters;
using TopicoMedico.Models.ViewModels;

namespace TopicoMedico.Controllers
{
    public class AtencionController : Controller
    {
        private readonly AtencionRepository _atencionRepo;
        private readonly AlumnoRepository _alumnoRepo;

        public AtencionController(AtencionRepository atencionRepo, AlumnoRepository alumnoRepo)
        {
            _atencionRepo = atencionRepo;
            _alumnoRepo = alumnoRepo;
        }

        /*[HttpGet]
        public async Task<IActionResult> Listado()
        {
            // Podrías implementar listado de atenciones si deseas
            return View();
        }*/

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Medicamentos = await _atencionRepo.ObtenerMedicamentosAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Atencion atencion, int[] medicamentoIds, int[] cantidades)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Medicamentos = await _atencionRepo.ObtenerMedicamentosAsync();
                return View(atencion);
            }

            // Asociar medicamentos seleccionados
            if (medicamentoIds != null && cantidades != null)
            {
                atencion.Medicamentos = medicamentoIds
                    .Select((id, index) => new MedicamentoAtencion
                    {
                        MedicamentoId = id,
                        Cantidad = cantidades[index]
                    }).ToList();
            }

            await _atencionRepo.AgregarAtencionAsync(atencion);
            TempData["Mensaje"] = "Atención registrada correctamente";
            return RedirectToAction("Index");
        }

        public class BuscarAlumnoRequest
        {
            public string Codigo { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> BuscarAlumno([FromBody] BuscarAlumnoRequest request)
        {
            string codigo = request.Codigo;

            if (string.IsNullOrEmpty(codigo))
                return Json(new { success = false, mensaje = "Debe ingresar un código" });

            var alumno = await _alumnoRepo.BuscarAlumnoPorCodigoAsync(codigo);
            if (alumno == null)
                return Json(new
                {
                    success = false,
                    mensaje = "Alumno no registrado",
                    registrarUrl = Url.Action("Create", "Alumno")
                });

            return Json(new
            {
                success = true,
                alumnoId = alumno.AlumnoId,
                nombre = alumno.NombreCompleto
            });
        }

    }
}
