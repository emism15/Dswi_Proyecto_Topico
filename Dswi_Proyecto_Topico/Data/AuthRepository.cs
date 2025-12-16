using Dswi_Proyecto_Topico.Helpers;
using Dswi_Proyecto_Topico.Models.Entitties;
using Microsoft.Data.SqlClient;

namespace Dswi_Proyecto_Topico.Data
{
    public class AuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // LOGIN
        public async Task<Usuario?> AutenticarAsync(string nombreUsuario, string contraseña)
        {
            Usuario? usuario = null;
            string hash = PasswordHelper.HashPassword(contraseña);

            using SqlConnection cn = new SqlConnection(_connectionString);

            string sql = @"
                SELECT u.UsuarioId, u.RolId, u.NombreCompleto, u.NombreUsuario,
                       u.Contraseña, u.Activo, u.DebecambiarContraseña,
                       r.NombreRol
                FROM Usuarios u
                INNER JOIN Roles r ON u.RolId = r.RolId
                WHERE u.NombreUsuario = @usuario
                  AND u.Contraseña = @password
                  AND u.Activo = 1";

            SqlCommand cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@usuario", nombreUsuario);
            cmd.Parameters.AddWithValue("@password", hash);

            await cn.OpenAsync();
            using SqlDataReader dr = await cmd.ExecuteReaderAsync();

            if (await dr.ReadAsync())
            {
                usuario = new Usuario
                {
                    UsuarioId = dr.GetInt32(0),
                    RolId = dr.GetInt32(1),
                    NombreCompleto = dr.GetString(2),
                    NombreUsuario = dr.GetString(3),
                    Contraseña = dr.GetString(4),
                    Activo = dr.GetBoolean(5),
                    DebecambiarContraseña = dr.GetBoolean(6),
                    Rol = new Rol
                    {
                        NombreRol = dr.GetString(7)
                    }
                };
            }

            return usuario;
        }

        // CAMBIAR CONTRASEÑA
        public async Task<bool> CambiarContraseñaAsync(
            int usuarioId, string contraseñaActual, string contraseñaNueva)
        {
            using SqlConnection cn = new SqlConnection(_connectionString);

            string sql = @"
                UPDATE Usuarios
                SET Contraseña = @nueva,
                    DebecambiarContraseña = 0
                WHERE UsuarioId = @id
                  AND Contraseña = @actual";

            SqlCommand cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", usuarioId);
            cmd.Parameters.AddWithValue("@actual", PasswordHelper.HashPassword(contraseñaActual));
            cmd.Parameters.AddWithValue("@nueva", PasswordHelper.HashPassword(contraseñaNueva));

            await cn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}
