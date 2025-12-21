using Dswi_Proyecto_Topico.Models.Entitties;

namespace Dswi_Proyecto_Topico.Service.Interfaces
{
    public interface IAuthService
    {
        Task<Usuario> AutenticarAsync(string nombreUsuario, string contraseña);
        Task<bool> CambiarContraseñaAsync(int usuarioId, string contraseñaActual, string contraseñaNueva);
    }

}
