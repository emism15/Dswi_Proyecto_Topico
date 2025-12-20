using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TopicoMedico.Models.Entities;

namespace TopicoMedico.Models.Entities
{
    public class Auditoria
    {
        [Key]
        public int AuditoriaId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Accion { get; set; }

        [Required]
        [StringLength(50)]
        public string Tabla { get; set; }

        public int? RegistroId { get; set; }
        public string ValoresAnteriores { get; set; }
        public string ValoresNuevos { get; set; }
        public DateTime FechaAccion { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string IPAddress { get; set; }

        // Navegación
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
    }
}

