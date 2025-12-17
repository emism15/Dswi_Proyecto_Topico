using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class AtencionDetallesModel
    {
        public int AlumnoId { get; set; }
        public DateTime FechaAtencion { get; set; }
        public TimeSpan HoraAtencion { get; set; }

        [Required(ErrorMessage = "Los detalles clínicos son obligatorios")]
        [StringLength(500)]
        public string DetallesClinicos { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los detalles clínicos son obligatorios")]
        [StringLength(250)]
        public string DiagnosticoPreliminar { get; set; } = string.Empty;
        public bool AlumnoEncontrado { get; set; } = false;

    }
}
