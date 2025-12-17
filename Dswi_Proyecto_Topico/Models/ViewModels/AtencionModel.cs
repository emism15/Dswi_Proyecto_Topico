using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class AtencionModel
    {
        public int AlumnoId { get; set; }
        [Required(ErrorMessage = "El código es obligatorio")]
        [RegularExpression(@"^i\d{9}$", ErrorMessage = "Formato invalido.  Ej: i200000000")]
        public string Codigo { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;

        public bool AlumnoEncontrado { get; set; } = false;
        public DateTime FechaAtencion { get; set; }
        public TimeSpan HoraAtencion { get; set; }

        
        public string DetallesClinicos { get; set; } = string.Empty;

        
        public string DiagnosticoPreliminar { get; set; } = string.Empty;





    }
}
