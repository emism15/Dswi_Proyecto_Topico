using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Helpers;
using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dswi_Proyecto_Topico.Service
{
    public class AuthService : IAuthService
    {

        private readonly TopicoDbContext _context;

        public AuthService(TopicoDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> AutenticarAsync(string nombreUsuario, string contraseña)
        {
            var contraseñaHash = PasswordHelper.HashPassword(contraseña);

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u =>
                    u.NombreUsuario == nombreUsuario &&
                    u.PasswordHash == contraseñaHash &&
                    u.Activo);

            if (usuario != null)
            {
                usuario.UltimoAcceso = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return usuario;
        }

        public async Task<bool> CambiarContraseñaAsync(int usuarioId, string contraseñaActual, string contraseñaNueva)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null) return false;

            var hashActual = PasswordHelper.HashPassword(contraseñaActual);
            if (usuario.PasswordHash != hashActual) return false;

            usuario.PasswordHash = PasswordHelper.HashPassword(contraseñaNueva);
            usuario.DebecambiarContraseña = false;

            await _context.SaveChangesAsync();
            return true;
        }
    }

}
