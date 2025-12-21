using Dswi_Proyecto_Topico.Models.Entitties;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class DashboardAlumnoViewModel
    {
        public int CitasPendientes { get; set; }
        public int CitasAtendidas { get; set; }

        public Cita? ProximaCita { get; set; }
        public int DiasParaProximaCita { get; set; }
    }
}
