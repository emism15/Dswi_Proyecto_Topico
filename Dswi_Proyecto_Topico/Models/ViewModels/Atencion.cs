using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class Atencion
    {
        public int AtencionId { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [RegularExpression(@"^i\d{9}$", ErrorMessage = "El código debe iniciar con 'i' seguido de 8 números")]
        public int AlumnoId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La hora es obligatoria")]
        [DataType(DataType.Time)]
        public TimeSpan Hora { get; set; }

        [Required(ErrorMessage = "Los detalles de la atención son obligatorios")]
        public string Detalles { get; set; }
        public string Diagnostico { get; set; }

        public List<MedicamentoAtencion> Medicamentos { get; set; }
    }
}

