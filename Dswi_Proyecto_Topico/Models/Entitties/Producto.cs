using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dswi_Proyecto_Topico.Models.Entitties
{
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El código de producto es obligatorio")]
        [StringLength(50)]
        public string CodigoProducto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100, MinimumLength = 3)]
        public string NombreProducto { get; set; }

        [StringLength(300)]
        public string Descripcion { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoProducto { get; set; } // 'Medicamento' o 'Implemento'

        [StringLength(30)]
        public string UnidadMedida { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int StockActual { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int StockMinimo { get; set; } = 10;

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal? PrecioUnitario { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaVencimiento { get; set; }

        [StringLength(100)]
        public string Laboratorio { get; set; }

        [StringLength(50)]
        public string Lote { get; set; }

        public bool RequiereReceta { get; set; } = false;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;

        // Navegación
        [ForeignKey("CategoriaId")]
        public virtual CategoriaProducto Categoria { get; set; }

        public virtual ICollection<DetalleCompra> DetallesCompra { get; set; }
        public virtual ICollection<DetalleReceta> DetallesReceta { get; set; }
    }
}
