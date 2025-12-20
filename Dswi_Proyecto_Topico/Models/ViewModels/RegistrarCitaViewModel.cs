using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class RegistrarCitaViewModel
    {
        public int CitaId { get; set; }
        public int AlumnoId { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [RegularExpression(@"^i\d{9}$", ErrorMessage = "Formato invalido.  Ej: i200000000")]
        public string Codigo { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public bool AlumnoEncontrado { get; set; } = false;
        public DateTime Fecha { get; set; } = DateTime.Now;

        public TimeSpan Hora { get; set; } = DateTime.Now.TimeOfDay;
        public string MotivoConsulta { get; set; } = string.Empty;

        public string TipoCita { get; set; } = string.Empty;

        public string EstadoCita { get; set; } = "Pendiente"; 
        public DateTime FechaRegistro { get; set; } = DateTime.Now; 




    }



}
