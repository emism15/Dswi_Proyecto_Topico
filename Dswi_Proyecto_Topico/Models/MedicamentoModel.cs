using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models
{
    public class MedicamentoModel
    {
		public int IdMedicamento { get; set; }

		[Required(ErrorMessage = "El nombre del medicamento es obligatorio.")]
		public string Nombre { get; set; }

		public string Categoria { get; set; }
		public string Presentacion { get; set; }


		[Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
		public int Stock { get; set; }

		[DataType(DataType.Date)]
		public DateTime FechaVencimiento { get; set; }

		public bool Activo { get; set; } = true;






	}
}
