using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class MedicamentoModel
    {
        public int MedicamentoId { get; set; }

        [Required(ErrorMessage = "El nombre del medicamento es obligatorio")]
        public string Nombre { get; set; }
        public int Stock { get; set; }

        public string Unidad { get; set; }
        public string Descripcion { get; set; }
    }
}
