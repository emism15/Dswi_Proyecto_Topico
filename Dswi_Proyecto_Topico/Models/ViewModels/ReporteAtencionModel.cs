namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class ReporteAtencionModel
    {
        public int AtencionId { get; set; }
        public string Codigo { get; set; }
        public string NombreCompleto { get; set; }
        public string DNI { get; set; }
        public string Telefono { get; set; } 
        public string Correo { get; set; } 

        public DateTime FechaAtencion { get; set; }
        public TimeSpan HoraAtencion { get; set; }
        public string DetallesClinicos { get; set; }
        public string DiagnosticoPreliminar { get; set; }

        public bool ReporteGenerado { get; set; }

        public List<ReporteMedicamentoModel> Medicamentos { get; set; }

        public ReporteAtencionModel()
        {
            Medicamentos = new List<ReporteMedicamentoModel>();
        }

    }
}
