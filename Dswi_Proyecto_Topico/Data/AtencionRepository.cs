using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.Data.SqlClient;

namespace Dswi_Proyecto_Topico.Data
{
    public class AtencionRepository
    {
        private readonly string _connectionString;

        public AtencionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        
        
      public async Task RegistrarAtencionAsync(AtencionModel model)
        {
            var sql = @"INSERT INTO Atencion(AlumnoId, Fecha, Hora, DetallesClinicos, DiagnosticoPreliminar)
                    VALUES(@AlumnoId, @Fecha, @Hora, @DetallesClinicos, @DiagnosticoPreliminar)";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@AlumnoId", model.AlumnoId);
                cmd.Parameters.AddWithValue("@Fecha", model.FechaAtencion.Date);
                cmd.Parameters.AddWithValue("@Hora", model.HoraAtencion);
                cmd.Parameters.AddWithValue("@DetallesClinicos", model.DetallesClinicos);
                cmd.Parameters.AddWithValue("@DiagnosticoPreliminar", model.DiagnosticoPreliminar);

                await conn.OpenAsync();
                await cmd.ExecuteReaderAsync();
            }
        }

        /*
        <!--Central-->

<!--
<div class="card shadow-sm mb-4">
    <div class="card-header bg-success text-white">Datos de la Atención Médica</div>
    
    <div class="card-body">
        <form asp-action="RegistrarAtencion" method="post">
        <input type="hidden" asp-for="FechaAtencion" />
            <input type="hidden" asp-for="HoraAtencion" />
        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Fecha</label>
                    <input class="form-control" value="@Model.FechaAtencion.ToString("dd/MM/yyyy")" readonly />
            </div>
            <div class="col-md-6">
                <label class="form-label">Hora</label>
                    <input class="form-control" value="@Model.HoraAtencion.ToString(@"hh\:mm")" readonly />
            </div>
            <div class="col-md-6">
                <label class="form-label">Detalles Clínicos</label>
                    <textarea asp-for="DetallesClinicos" class="form-control"></textarea>
                    <span asp-validation-for="DetallesClinicos" class="text-danger"></span>
            </div>
            <div class="col-md-6">
                <label class="form-label">Diagnóstico Preliminar</label>
                    <textarea asp-for="DiagnosticoPreliminar" class="form-control"></textarea>
                    <span asp-validation-for="DiagnosticoPreliminar" class="text-danger"></span>
            </div>
        </div>
        </form>
    </div>
</div>

-->*/

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> RegistrarAtencion(AtencionDetallesModel model)
        {
            if (!model.AlumnoEncontrado)
            {
                ModelState.AddModelError("", "Debe buscar y confirmar un alumno antes de registrar la atención.");
                return View("RegistrarAtencion", model);
            }
            if(!ModelState.IsValid)
            {
                return View("RegistrarAtencion", model);

            }

            await atencionRepository.RegistrarAtencionAsync(model);

            TempData["MensajeExito"] = "Atención registrada correctamente.";

            return RedirectToAction("RegistrarAtencion");
        }
        */



    }
}
