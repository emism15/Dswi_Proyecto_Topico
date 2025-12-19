using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Helpers;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;

namespace Dswi_Proyecto_Topico.Controllers
{
    public class EnfermeraController : Controller
    {
        private readonly AlumnoRepository alumnoRepository;
         private readonly AtencionRepository atencionRepository;
       
        private readonly EnfermeraRepository _repo;

        public EnfermeraController(AlumnoRepository alumnoRepository, AtencionRepository atencionRepository, EnfermeraRepository repo)
       
        {
            this.alumnoRepository = alumnoRepository;
            this.atencionRepository = atencionRepository;
            _repo = repo;
        }

        

        [HttpGet]
        public IActionResult Index()
        {
            // Seguridad básica
            if (!HttpContext.Session.IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            // Validar rol
            if (HttpContext.Session.GetNombreRol() != "Enfermera")
                return RedirectToAction("Login", "Auth");

            DashboardEnfermeraViewModel vm = _repo.ObtenerDashboard();

            return View(vm);
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
                HoraAtencion = DateTime.Now.TimeOfDay,

                Medicamentos = HttpContext.Session.
                GetObject<List<AtencionMedicamentoModel>>("MedicamentosTemp")
                ?? new List<AtencionMedicamentoModel>()
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
            
            model.AlumnoEncontrado = true;
            model.AlumnoId = alumno.AlumnoId;
            model.NombreCompleto = alumno.NombreCompleto;
            model.DNI = alumno.DNI;
            model.Telefono = alumno.Telefono;
            model.Correo = alumno.Correo;

            
            model.Medicamentos = HttpContext.Session.GetObject<List<AtencionMedicamentoModel>>("MedicamentosTemp")
                                ?? new List<AtencionMedicamentoModel>();

            ModelState.Clear();

            return View("RegistrarAtencion", model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> RegistrarAtencion(AtencionModel model)
        {
            model.FechaAtencion = DateTime.Now.Date;
            model.HoraAtencion = DateTime.Now.TimeOfDay;

            if (!model.AlumnoEncontrado || model.AlumnoId == 0)
            {
                ModelState.AddModelError("", "Debe buscar y confirmar un alumno antes de registrar la atención.");
                return View("RegistrarAtencion", model);
            }
            

            
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

            if (model.FechaAtencion < new DateTime(1753, 1, 1))
                model.FechaAtencion = DateTime.Now.Date;
            if (model.HoraAtencion < TimeSpan.Zero || model.HoraAtencion >= TimeSpan.FromDays(1))
                model.HoraAtencion = DateTime.Now.TimeOfDay;

            var medicamentos = HttpContext.Session.GetObject<List<AtencionMedicamentoModel>>("MedicamentosTemp") ??
                new List<AtencionMedicamentoModel>();
            try
            {
                await atencionRepository.RegistrarAtencionCompletaAsync(model, medicamentos);
                HttpContext.Session.Remove("MedicamentosTemp");

                TempData["MensajeExito"] = "Atención registrada correctamente.";
                return RedirectToAction(nameof(RegistrarAtencion));
            }

            catch (SqlException e)
            {
                ModelState.AddModelError("", e.Message);
                return View("RegistrarAtencion", model);
            }

            
        }
        
        [HttpGet]
        public async Task<IActionResult> BuscarMedicamentos (string filtro)
        {
            var lista = await atencionRepository.BuscarMedicamentoAsync(filtro ?? "");
            return PartialView("_ListaMedicamentos", lista);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMedicamento(int medicamentoId, string nombre, int cantidad, int stock)
        {
            if(cantidad <=0 || cantidad > stock)
            {
                return BadRequest("Stock insuficiente");
            }
            var lista = HttpContext.Session.
                GetObject<List<AtencionMedicamentoModel>>("MedicamentosTemp")
                ?? new List<AtencionMedicamentoModel>();

            var existente = lista.FirstOrDefault(x => x.MedicamentoId == medicamentoId);
            

            if(existente != null)
            {
                if (existente.Cantidad + cantidad > stock)
                    return BadRequest("Stock insuficiente");
                existente.Cantidad += cantidad;
            }

            else
            {
                lista.Add(new AtencionMedicamentoModel
               {
                    MedicamentoId = medicamentoId,
                    Nombre = nombre,
                    Cantidad = cantidad,
                    StockDisponible = stock

                });
            }

            HttpContext.Session.SetObject("MedicamentosTemp", lista);
            return PartialView("_TablaMedicamentosSeleccionados", lista);
        }

        [HttpPost]
        public async Task<IActionResult> EditarCantidad(int medicamentoId, int nuevaCantidad)
        {
            var lista = HttpContext.Session.
               GetObject<List<AtencionMedicamentoModel>>("MedicamentosTemp");

            if (lista == null)
                return BadRequest();

            var item = lista.FirstOrDefault(x => x.MedicamentoId == medicamentoId);
            if (item == null) 

            if (nuevaCantidad <= 0 || nuevaCantidad > item.StockDisponible)
                return BadRequest("Cantidad invalida");
            item.Cantidad = nuevaCantidad;

            HttpContext.Session.SetObject("MedicamentosTemp", lista);
            return PartialView("_TablaMedicamentosSeleccionados", lista);
        }

        
        [HttpPost]
        public async Task<IActionResult> EliminarMedicamento(int medicamentoId)
        {
            var lista = HttpContext.Session.
                GetObject<List<AtencionMedicamentoModel>>("MedicamentosTemp");

            if (lista == null)
                return BadRequest();

            lista.RemoveAll(m => m.MedicamentoId == medicamentoId);

            HttpContext.Session.SetObject("MedicamentosTemp", lista);
            return PartialView("_TablaMedicamentosSeleccionados", lista);
        }


        


    }
}
