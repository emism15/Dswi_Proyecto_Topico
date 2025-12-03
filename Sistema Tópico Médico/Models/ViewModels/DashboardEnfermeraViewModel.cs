using System;
using System.Collections.Generic;
using TopicoMedico.Models.Entities;

namespace TopicoMedico.Models.ViewModels
{
    public class DashboardEnfermeraViewModel
    {
        // Contadores principales (tarjetas)
        public int CitasHoy { get; set; }
        public int CitasPendientes { get; set; }

        // Pacientes atendidos hoy (tarjeta)
        public int PacientesAtendidosHoy { get; set; }

        // Alternativa por si en otras vistas se usa 'CitasAtendidasHoy'
        public int CitasAtendidasHoy { get; set; }

        // Próximas citas (lista para mostrar tablas / listados)
        public List<Cita> ProximasCitas { get; set; } = new List<Cita>();

        // Otra propiedad nombrada diferente (algunas vistas usaban CitasProximas)
        // la dejamos por compatibilidad con las vistas que te entregué
        public List<Cita> CitasProximas
        {
            get => ProximasCitas;
            set => ProximasCitas = value;
        }

        // Alertas relevantes para la enfermera
        public List<Alerta> Alertas { get; set; } = new List<Alerta>();

        // Para compatibilidad con el nombre usado anteriormente (AlertasCitas)
        public List<Alerta> AlertasCitas
        {
            get => Alertas;
            set => Alertas = value;
        }
    }
}
