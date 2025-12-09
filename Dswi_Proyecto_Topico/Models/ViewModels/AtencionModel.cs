using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class AtencionModel
    {
        public int AtencionId { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [RegularExpression(@"^i\d{9}$", ErrorMessage = "El código debe iniciar con 'i' seguido de 9 números")]
        public int AlumnoId { get; set; }
        public DateTime Fecha { get; set; }

        public TimeSpan Hora { get; set; }

        [Required(ErrorMessage = "Los detalles de la atención son obligatorios")]
        public string Detalles { get; set; }
        public string Diagnostico { get; set; }

        public List<AtencionMedicamentoModel> Medicamentos { get; set; } = new();
    }
}
