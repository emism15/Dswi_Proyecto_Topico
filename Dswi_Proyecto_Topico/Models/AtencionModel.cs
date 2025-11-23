using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dswi_Proyecto_Topico.Models
{
    public class AtencionModel
    {
		public int IdAtencion { get; set; }
		[Required]
		public int IdPaciente { get; set; }
		[ForeignKey("IdPaciente")]
		public PacienteModel Paciente { get; set; }

		[Required]
		public int IdEnfermera { get; set; }
		[ForeignKey("IdEnfermera")]
		public UsuarioModel Enfermera { get; set; }

		public DateTime Fecha { get; set; } = DateTime.Now;


		[Required(ErrorMessage = "El motivo de la atención es obligatorio.")]
		public string Motivo { get; set; }
		public string Diagnostico { get; set; }

		public string Estado { get; set; } = "PENDIENTE";
		// Estados posibles: PENDIENTE , EN SEGUIMIENTO , FINALIZADA
	}
}
