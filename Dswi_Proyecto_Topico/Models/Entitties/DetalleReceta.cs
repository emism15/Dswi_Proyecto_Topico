using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dswi_Proyecto_Topico.Models.Entitties
{
    public class DetalleReceta
    {
        [Key]
        public int DetalleRecetaId { get; set; }

        [Required]
        public int RecetaId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [StringLength(100)]
        public string Dosis { get; set; }

        [StringLength(100)]
        public string Frecuencia { get; set; }

        [StringLength(50)]
        public string Duracion { get; set; }

        [StringLength(300)]
        public string Indicaciones { get; set; }

        public DateTime? FechaEntrega { get; set; }
        public bool Entregado { get; set; } = false;

        // Navegación
        [ForeignKey("RecetaId")]
        public virtual Receta Receta { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; }
    }
}
