using System.ComponentModel.DataAnnotations;
using TopicoMedico.Models.Entities;

namespace TopicoMedico.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        [Display(Name = "Rol")]
        public int RolId { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100, MinimumLength = 3)]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe contener solo números")]
        [Display(Name = "DNI")]
        public string DNI { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Teléfono inválido")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 4)]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [StringLength(200)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Para crear usuario nuevo
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contraseña { get; set; }

        // Lista de roles disponibles
        public List<Rol> RolesDisponibles { get; set; }
    }

}
