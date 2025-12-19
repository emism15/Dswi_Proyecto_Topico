using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class MedicamentoModel
    {
        public int MedicamentoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Stock { get; set; }
        public bool Estado { get; set; }
        
    }
}
