using Microsoft.AspNetCore.Identity;

namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class ApplicationUser : IdentityUser
    {

        public bool DebeCambiarPassword { get; set; } = true;

        public string Codigo { get; set; }

    }
}
