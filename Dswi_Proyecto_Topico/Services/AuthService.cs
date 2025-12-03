using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Helpers;

namespace Dswi_Proyecto_Topico.Services
{
    public class AuthService 
    {

        private readonly AuthRepository _authRepository;

        public AuthService(AuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<Usuario> AutenticarAsync(string nombreUsuario, string contraseña)
        {
            var contraseñaHash = PasswordHelper.HashPassword(contraseña);

            var usuario = await _authRepository.AutenticarAsync(nombreUsuario, contraseñaHash);

            if (usuario != null)
            {
                await _authRepository.ActualizarUltimoAccesoAsync(usuario.UsuarioId);
            }

            return usuario;
        }

        public async Task<bool> CambiarContraseñaAsync(int usuarioId, string contraseñaActual, string contraseñaNueva)
        {
            var usuario = await _authRepository.AutenticarAsync(
                nombreUsuario: null,
                contraseñaHash: PasswordHelper.HashPassword(contraseñaActual)
            );

            if (usuario == null)
                return false;

            var nuevaHash = PasswordHelper.HashPassword(contraseñaNueva);

            return await _authRepository.CambiarContraseñaAsync(usuarioId, nuevaHash);
        }
    }
}
