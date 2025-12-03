using Dswi_Proyecto_Topico.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class CitaViewModel
    {
        public int CitaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un paciente")]
        [Display(Name = "Paciente")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es obligatoria")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha y Hora de Cita")]
        public DateTime FechaCita { get; set; }

        [Required(ErrorMessage = "El motivo de consulta es obligatorio")]
        [StringLength(300, MinimumLength = 10)]
        [Display(Name = "Motivo de Consulta")]
        public string MotivoConsulta { get; set; }

        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        // Para vista
        public string NombrePaciente { get; set; }
        public string DNIPaciente { get; set; }
        public string EstadoCita { get; set; }
        public List<Usuario> PacientesDisponibles { get; set; }
    }

}