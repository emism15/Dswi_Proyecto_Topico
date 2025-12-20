using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TopicoMedico.Models.Entities;

namespace TopicoMedico.Models.Entities
{
    public class Cita
    {
        [Key]
        public int CitaId { get; set; }

        [Required]
        public int PacienteId { get; set; }

        public int? EnfermeraId { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es obligatoria")]
        [DataType(DataType.DateTime)]
        public DateTime FechaCita { get; set; }

        [Required(ErrorMessage = "El motivo de consulta es obligatorio")]
        [StringLength(300, MinimumLength = 10)]
        public string MotivoConsulta { get; set; }

        [StringLength(500)]
        public string Diagnostico { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }

        [StringLength(20)]
        public string EstadoCita { get; set; } = "Pendiente"; // 'Pendiente', 'Atendida', 'Cancelada', 'NoAsistió'

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public DateTime? FechaAtencion { get; set; }

        [StringLength(300)]
        public string SignosVitales { get; set; } // JSON: {temperatura, presion, pulso}

        // Navegación
        [ForeignKey("PacienteId")]
        public virtual Usuario Paciente { get; set; }

        [ForeignKey("EnfermeraId")]
        public virtual Usuario Enfermera { get; set; }

        public virtual ICollection<Receta> Recetas { get; set; }
    }

}
