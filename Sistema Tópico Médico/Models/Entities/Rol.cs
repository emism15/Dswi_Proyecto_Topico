using System.ComponentModel.DataAnnotations;

namespace TopicoMedico.Models.Entities
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(50)]
        public string NombreRol { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }


}
