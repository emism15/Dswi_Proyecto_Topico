using TopicoMedico.Models.Entities;

namespace TopicoMedico.Models.ViewModels
{
    public class DashboardPacienteViewModel
    {
        public int CitasPendientes { get; set; }
        public int CitasAtendidas { get; set; }
        public int RecetasVigentes { get; set; }
        public Cita ProximaCita { get; set; }
        public List<Cita> HistorialCitas { get; set; }
        public List<Receta> RecetasRecientes { get; set; }
    }
}
