using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dswi_Proyecto_Topico.Models.Entitties
{
    public class Compra
    {
        [Key]
        public int CompraId { get; set; }

        [Required]
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El número de comprobante es obligatorio")]
        [StringLength(50)]
        public string NumeroComprobante { get; set; }

        [Required]
        [StringLength(30)]
        public string TipoComprobante { get; set; } // 'Factura', 'Boleta', 'Guía'

        public DateTime FechaCompra { get; set; } = DateTime.Now;

        // 🟢 CORRECCIÓN: Propiedad faltante para registro
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        public int UsuarioRegistroId { get; set; }

        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(12,2)")]
        public decimal MontoTotal { get; set; } = 0;

        [StringLength(500)]
        public string Observaciones { get; set; }

        [StringLength(20)]
        public string Estado { get; set; } = "Completada"; // 'Pendiente', 'Completada', 'Anulada'

        // Navegación
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }

        [ForeignKey("UsuarioRegistroId")]
        public virtual Usuario UsuarioRegistro { get; set; }

        public virtual ICollection<DetalleCompra> DetallesCompra { get; set; }
    }
}
