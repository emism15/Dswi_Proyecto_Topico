using Dswi_Proyecto_Topico.Models.Entitties;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class DashboardEnfermeraViewModel
    {
        public int CitasHoy { get; set; }
        public int CitasPendientes { get; set; }

        public int PacientesAtendidosHoy { get; set; }

    
        public int CitasAtendidasHoy { get; set; }

       
        public List<Cita> ProximasCitas { get; set; } = new List<Cita>();

       
        public List<Cita> CitasProximas
        {
            get => ProximasCitas;
            set => ProximasCitas = value;
        }


        public List<Alerta> Alertas { get; set; } = new List<Alerta>();

 
        public List<Alerta> AlertasCitas
        {
            get => Alertas;
            set => Alertas = value;
        }
    }
}