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

        public async Task<Usuario> AutenticarAsync(string nombreUsuario, string contraseñaHash)
        {
            Usuario usuario = null;

            string query = @"
            SELECT u.UsuarioId, u.NombreUsuario, u.Contraseña, u.NombreCompleto, u.DNI,
                   u.Email, u.Telefono, u.Activo, u.DebecambiarContraseña,
                   u.FechaRegistro, u.UltimoAcceso, u.RolId,
                   r.NombreRol
            FROM Usuario u
            INNER JOIN Rol r ON u.RolId = r.RolId
            WHERE u.NombreUsuario = @NombreUsuario AND u.Contraseña = @Contraseña AND u.Activo = 1";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                cmd.Parameters.AddWithValue("@Contraseña", contraseñaHash);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        usuario = new Usuario
                        {
                            UsuarioId = reader.GetInt32(0),
                            NombreUsuario = reader.GetString(1),
                            Contraseña = reader.GetString(2),
                            NombreCompleto = reader.GetString(3),
                            DNI = reader.GetString(4),
                            Email = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Telefono = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Activo = reader.GetBoolean(7),
                            DebecambiarContraseña = reader.GetBoolean(8),
                            FechaRegistro = reader.GetDateTime(9),
                            UltimoAcceso = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                            RolId = reader.GetInt32(11),
                            Rol = new Rol
                            {
                                RolId = reader.GetInt32(11),
                                NombreRol = reader.GetString(12)
                            }
                        };
                    }
                }
            }

            return usuario;
        }

        public async Task<bool> CambiarContraseñaAsync(int usuarioId, string nuevaContraseñaHash)
        {
            string query = @"
            UPDATE Usuario
            SET Contraseña = @NuevaContraseña, DebecambiarContraseña = 0
            WHERE UsuarioId = @UsuarioId";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@NuevaContraseña", nuevaContraseñaHash);

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }


        public async Task ActualizarUltimoAccesoAsync(int usuarioId)
        {
            string query = @"
            UPDATE Usuario
            SET UltimoAcceso = GETDATE()
            WHERE UsuarioId = @UsuarioId";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}