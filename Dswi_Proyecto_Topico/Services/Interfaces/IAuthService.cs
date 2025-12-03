using System.Collections.Generic;
using System.Threading.Tasks;
using Dswi_Proyecto_Topico.Models;
using Dswi_Proyecto_Topico.Models.Entities;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Dswi_Proyecto_Topico.Services.Interfaces
{

    public interface IAuthService
    {
        Task<Usuario> AutenticarAsync(string nombreUsuario, string contraseña);
        Task<bool> CambiarContraseñaAsync(int usuarioId, string contraseñaActual, string contraseñaNueva);
    }
}