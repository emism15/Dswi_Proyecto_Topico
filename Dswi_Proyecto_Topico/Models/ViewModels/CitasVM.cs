using Dswi_Proyecto_Topico.Models.Entitties;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class CitasVM
    {

        public List<Cita> Citas { get; set; }

        public int Total { get; set; }
        public int Pendientes { get; set; }
        public int Atendidas { get; set; }
        public int Canceladas { get; set; }

        public string EstadoSeleccionado { get; set; }
    }
}
