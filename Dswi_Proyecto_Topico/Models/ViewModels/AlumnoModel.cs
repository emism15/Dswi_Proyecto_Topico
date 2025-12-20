using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class AlumnoModel
    {
        public int AlumnoId { get; set; }

        [Display(Name = "Id de Estudiante")]
        [Required(ErrorMessage = "El código del alumno es obligatorio")]
        [RegularExpression(@"^i\d{9}$", ErrorMessage = "El código debe iniciar con 'i' seguido de 9 números")]
        public string Codigo { get; set; }

        [Display(Name = "Nombre Completo")]
        [Required(ErrorMessage = "Los nombres y apellidos son obligatorios")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo debe contener letras y espacios")]
        public string NombreCompleto { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        public int Edad
        {
            get
            {
                var hoy = DateTime.Today;
                int edad = hoy.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > hoy.AddYears(-edad))
                    edad--;
                return edad;
            }
        }

        [Display(Name = "DNI")]
        [Required(ErrorMessage = "El DNI es obligatorio")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe contener exactamente 8 números")]
        public string DNI { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^9\d{8}$", ErrorMessage = "El teléfono debe iniciar con 9 y contener 9 números")]
        public string Telefono { get; set; }

        [Display(Name = "Correo Personal")]
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        public string Correo { get; set; }
    }
}
