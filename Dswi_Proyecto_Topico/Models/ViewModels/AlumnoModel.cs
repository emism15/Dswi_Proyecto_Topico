using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class AlumnoModel
    {
        public int AlumnoId { get; set; }

        [Required(ErrorMessage = "El código del alumno es obligatorio")]
        [RegularExpression(@"^i\d{9}$", ErrorMessage = "El código debe iniciar con 'i' seguido de 9 números")]
        public string CodigoAlumno { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "La edad es obligatoria")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        public string DNI { get; set; }

        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string Correo { get; set; }
    }
}
