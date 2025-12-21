using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class CambiarPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string PasswordActual { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordNueva { get; set; }

        [Required]
        [Compare("PasswordNueva", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        public string ConfirmarPassword { get; set; }
    }
}
