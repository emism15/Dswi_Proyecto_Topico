using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class LoginAlumnoModel
    {
        [Required(ErrorMessage = "Ingrese el usuario")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Ingrese la contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
