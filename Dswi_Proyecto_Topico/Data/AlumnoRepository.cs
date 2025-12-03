using Microsoft.Data.SqlClient;
using Dswi_Proyecto_Topico.Models.ViewModels;

namespace Dswi_Proyecto_Topico.Data
{
    public class AlumnoRepository
    {
        private readonly string _connectionString;

        public AlumnoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task AgregarAlumnoAsync(Alumno alumno)
        {
            var sql = @"INSERT INTO Alumno (Codigo, NombreCompleto, FechaNacimiento, Edad, DNI, Telefono, Correo)
                        VALUES (@Codigo, @NombreCompleto, @FechaNacimiento, @Edad, @DNI, @Telefono, @Correo)";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Codigo", alumno.Codigo);
                cmd.Parameters.AddWithValue("@NombreCompleto", alumno.NombreCompleto);
                cmd.Parameters.AddWithValue("@FechaNacimiento", alumno.FechaNacimiento);
                cmd.Parameters.AddWithValue("@Edad", alumno.Edad);
                cmd.Parameters.AddWithValue("@DNI", alumno.DNI);
                cmd.Parameters.AddWithValue("@Telefono", (object)alumno.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Correo", (object)alumno.Correo ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    
public async Task<Alumno> BuscarAlumnoPorCodigoAsync(string codigo)
        {
            Alumno alumno = null;
            var sql = "SELECT * FROM Alumno WHERE Codigo = @Codigo";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Codigo", codigo);
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        alumno = new Alumno
                        {
                            AlumnoId = reader.GetInt32(0),
                            Codigo = reader.GetString(1),
                            NombreCompleto = reader.GetString(2),
                            FechaNacimiento = reader.GetDateTime(3),
                            Edad = reader.GetInt32(4),
                            DNI = reader.GetString(5),
                            Telefono = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Correo = reader.IsDBNull(7) ? null : reader.GetString(7)
                        };
                    }
                }
            }

            return alumno;
}

}
}
