using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models
{
    public class UsuarioModel
    {
        public int IdUsuario { get; set; }

		[Required(ErrorMessage = "El nombre de usuario es olbigatorio")]
		public string NombreUsuario { get; set; }

		[Required(ErrorMessage = "El correo es olbigatorio")]
		[EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
		public string Correo { get; set; }

		[Required(ErrorMessage = "La contraseña es obligatoria")]
		[DataType(DataType.Password)]
		[StringLength(10, MinimumLength = 4, ErrorMessage = "La contraseña debe tener entre 4 y 10 caracteres")]
		public string Contrasena { get; set; }

		[Required(ErrorMessage = "El rol es obligatorio")]
		public string Rol { get; set; } //Admin,Enfermera,Alumno

		[Display(Name = "Usuario Activo")]
		public bool Activo { get; set; } = true;

		[Display(Name = "Debe Cambiar Contraseña")]
		public bool CambiarContra{ get; set; } = true;

	}
}
