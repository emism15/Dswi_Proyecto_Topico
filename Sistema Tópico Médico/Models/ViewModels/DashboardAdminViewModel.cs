using TopicoMedico.Models.Entities;

namespace TopicoMedico.Models.ViewModels
{
    public class DashboardAdminViewModel
    {
        public int TotalUsuarios { get; set; }
        public int TotalPacientes { get; set; }
        public int TotalEnfermeras { get; set; }
        public int ProductosStockBajo { get; set; }
        public int ProductosPorVencer { get; set; }
        public decimal MontoComprasMes { get; set; }
        public List<Alerta> AlertasCriticas { get; set; }
        public List<Producto> ProductosAlerta { get; set; }
    }
}
