using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dswi_Proyecto_Topico.Models
{
    public class DetalleAtencionModel
    {
		public int IdDetalle { get; set; }

		public int IdAtencion { get; set; }
		[ForeignKey("IdAtencion")]
		public AtencionModel Atencion { get; set; }

		public int IdMedicamento { get; set; }
		[ForeignKey("IdMedicamento")]
		public MedicamentoModel Medicamento { get; set; }


		[Required(ErrorMessage = "Debe ingresar la cantidad.")]
		[Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
		public int Cantidad { get; set; }
	}
}
