using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dswi_Proyecto_Topico.Models.Entitties
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }


        [Required(ErrorMessage = "El rol es obligatorio")]
        public int RolId { get; set; }


        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string NombreCompleto { get; set; }


        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(20)]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 dígitos")]
        public string DNI { get; set; }


        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100)]
        public string Email { get; set; }


        [Phone(ErrorMessage = "Teléfono inválido")]
        [StringLength(20)]
        public string Telefono { get; set; }


        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "El usuario debe tener entre 4 y 50 caracteres")]
        public string NombreUsuario { get; set; }


        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }


        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }


        [StringLength(200)]
        public string Direccion { get; set; }


        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public DateTime? UltimoAcceso { get; set; }
        public bool Activo { get; set; } = true;
        [Column("DebecambiarContraseña")]
        public bool DebecambiarContraseña { get; set; } = true;

        // Navegación
        [ForeignKey("RolId")]
        public virtual Rol Rol { get; set; }

        public virtual ICollection<Cita> CitasComoPaciente { get; set; }
        public virtual ICollection<Cita> CitasComoEnfermera { get; set; }
        public virtual ICollection<Receta> RecetasComoPaciente { get; set; }
        public virtual ICollection<Receta> RecetasComoEnfermera { get; set; }
        public string PasswordHash { get; internal set; }
    }
}
