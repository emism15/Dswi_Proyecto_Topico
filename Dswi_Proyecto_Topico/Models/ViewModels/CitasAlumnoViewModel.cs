using Dswi_Proyecto_Topico.Models.Entitties;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class CitasAlumnoViewModel
    {
        public int Pendientes { get; set; }
        public int Atendidas { get; set; }
        public int Canceladas { get; set; }

        public List<Cita> Citas { get; set; }
    }
}
