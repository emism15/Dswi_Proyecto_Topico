using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.Entitties
{
    public class Proveedor
    {
        [Key]
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio")]
        [StringLength(100)]
        public string NombreProveedor { get; set; }

        [Required(ErrorMessage = "El RUC es obligatorio")]
        [StringLength(20)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "El RUC debe tener 11 dígitos")]
        public string RUC { get; set; }

        [Phone]
        [StringLength(20)]
        public string Telefono { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        [StringLength(100)]
        public string ContactoNombre { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<Compra> Compras { get; set; }
    }
}
