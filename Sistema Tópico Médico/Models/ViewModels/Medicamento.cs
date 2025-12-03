using System.ComponentModel.DataAnnotations;
namespace TopicoMedico.Models.ViewModels
{
    public class Medicamento
    {
        public int MedicamentoId { get; set; }

        [Required(ErrorMessage = "El nombre del medicamento es obligatorio")]
        public string Nombre { get; set; }
        public int Stock { get; set; }
    }
}
