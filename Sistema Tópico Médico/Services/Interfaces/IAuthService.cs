using System.Collections.Generic;
using System.Threading.Tasks;
using TopicoMedico.Models;
using TopicoMedico.Models.Entities;
using TopicoMedico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace TopicoMedico.Services.Interfaces
{

    public interface IAuthService
    {
        Task<Usuario> AutenticarAsync(string nombreUsuario, string contraseña);
        Task<bool> CambiarContraseñaAsync(int usuarioId, string contraseñaActual, string contraseñaNueva);
    }
}