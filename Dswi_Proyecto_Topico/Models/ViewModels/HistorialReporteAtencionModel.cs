namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class HistorialReporteAtencionModel
    {
        public int AtencionId { get; set; }
        public string Codigo { get; set; }
        public string NombreCompleto { get; set; }

        public DateTime FechaAtencion { get; set; }

        public DateTime? FechaGeneracionReporte { get; set; }
    }
}
