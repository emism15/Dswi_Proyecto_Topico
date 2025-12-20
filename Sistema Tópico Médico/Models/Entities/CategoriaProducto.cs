using System.ComponentModel.DataAnnotations;

namespace TopicoMedico.Models.Entities
{
    public class CategoriaProducto
    {
        [Key]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El nombre de categoría es obligatorio")]
        [StringLength(50)]
        public string NombreCategoria { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoCategoria { get; set; } // 'Medicamento' o 'Implemento'

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<Producto> Productos { get; set; }
    }
}
