using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dswi_Proyecto_Topico.Models.Entitties
{
    public class Receta
    {
        [Key]
        public int RecetaId { get; set; }

        [Required]
        public int CitaId { get; set; }

        [Required]
        public int PacienteId { get; set; }

        [Required]
        public int EnfermeraId { get; set; }

        public DateTime FechaEmision { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string Indicaciones { get; set; }

        [StringLength(300)]
        public string Observaciones { get; set; }

        [StringLength(20)]
        public string Estado { get; set; } = "Vigente"; // 'Vigente', 'Cumplida', 'Anulada'

        // Navegación
        [ForeignKey("CitaId")]
        public virtual Cita Cita { get; set; }

        [ForeignKey("PacienteId")]
        public virtual Usuario Paciente { get; set; }

        [ForeignKey("EnfermeraId")]
        public virtual Usuario Enfermera { get; set; }

        public virtual ICollection<DetalleReceta> DetallesReceta { get; set; }
    }
}
