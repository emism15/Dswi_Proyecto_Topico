using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopicoMedico.Models.Entities
{
    public class Compra
    {
        [Key]
        public int CompraId { get; set; }

        [Required]
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El número de comprobante es obligatorio")]
        [StringLength(50)]
        public string NumeroComprobante { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string TipoComprobante { get; set; } = string.Empty;

        public DateTime FechaCompra { get; set; } = DateTime.Now;

        [Required]
        public int UsuarioRegistroId { get; set; }

        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(12,2)")]
        public decimal MontoTotal { get; set; } = 0;

        [StringLength(500)]
        public string? Observaciones { get; set; }   // ✅ NULL permitido

        [StringLength(20)]
        public string Estado { get; set; } = "Completada";

        // Navegación (OPCIONALES)
        public virtual Proveedor? Proveedor { get; set; }          // ✅
        public virtual Usuario? UsuarioRegistro { get; set; }      // ✅

        public virtual ICollection<DetalleCompra> DetallesCompra { get; set; }
            = new List<DetalleCompra>();                             // ✅
    }

}