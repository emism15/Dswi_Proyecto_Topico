using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dswi_Proyecto_Topico.Models
{
	public class PacienteModel
	{
		public int IdPaciente { get; set; }

		[Required(ErrorMessage = "El código del alumno es obligatorio.")]
		public string CodigoAlumno { get; set; }  // EJ: i202312345


		[Required(ErrorMessage = "Los nombres son obligatorios.")]
		public string Nombres { get; set; }


		[Required(ErrorMessage = "Los apellidos son obligatorios.")]
		public string Apellidos { get; set; }


		[Required(ErrorMessage = "El número de DNI es obligatorio.")]
		public string DNI { get; set; }

		[DataType(DataType.Date)]
		public DateTime FechaNacimiento { get; set; }

		[Required(ErrorMessage = "Ingrese un numero de telefono")]
		public string Telefono { get; set; }
		public string Direccion { get; set; }

		public int IdUsuario { get; set; }
		[ForeignKey("IdUsuario")]
		public UsuarioModel Usuario { get; set; }

	}
}