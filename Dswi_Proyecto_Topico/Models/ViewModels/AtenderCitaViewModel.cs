using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class AtenderCitaViewModel
    {
        public int CitaId { get; set; }

        [Display(Name = "Paciente")]
        public string NombreAlumno { get; set; }

        [Display(Name = "Fecha de Cita")]
        public DateTime FechaCita { get; set; }

        [Display(Name = "Motivo de Consulta")]
        public string MotivoConsulta { get; set; }

        [Required(ErrorMessage = "El diagnóstico es obligatorio")]
        [StringLength(500, MinimumLength = 10)]
        [Display(Name = "Diagnóstico")]
        public string Diagnostico { get; set; }

        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Display(Name = "Temperatura (°C)")]
        public string Temperatura { get; set; }

        [Display(Name = "Presión Arterial")]
        public string PresionArterial { get; set; }

        [Display(Name = "Pulso (lpm)")]
        public string Pulso { get; set; }

        [Display(Name = "Saturación O2 (%)")]
        public string SaturacionO2 { get; set; }

        [Display(Name = "Frecuencia Respiratoria")]
        public string FrecuenciaRespiratoria { get; set; }
    }
}
