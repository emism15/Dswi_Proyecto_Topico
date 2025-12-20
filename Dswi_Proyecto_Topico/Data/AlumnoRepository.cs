using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.Data.SqlClient;

namespace Dswi_Proyecto_Topico.Data
{
    public class AlumnoRepository
    {
        private readonly string _connectionString;

        public AlumnoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> ExisteCodigoAsync(string codAlumno)
        {
            var sql = "SELECT COUNT(*) FROM Alumno WHERE CodAlumno = @CodAlumno";
            using( var conn = new SqlConnection(_connectionString))
                using ( var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CodAlumno", codAlumno);

                await conn.OpenAsync();
                int count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;

            }
        }

        public async Task<AtencionModel?> BuscarAlumnoPorCodigoAsync (string codigo)
        {
            var sql = @"SELECT AlumnoId, CodAlumno, NombreCompleto, DNI, Telefono, Correo 
                       FROM Alumno 
                       WHERE LOWER(CodAlumno) = LOWER(@CodAlumno)";
            using( var conn = new SqlConnection(_connectionString))
                using( var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CodAlumno", codigo.Trim());
                await conn.OpenAsync();
                using( var reader = await cmd.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        return new AtencionModel
                        {
                            AlumnoId = reader.GetInt32(0),
                            Codigo = reader.GetString(1).Trim(),
                            NombreCompleto = reader.GetString(2).Trim(),
                            DNI = reader.GetString(3).Trim(),
                            Telefono = reader.GetString(4).Trim() ?? "",
                            Correo = reader.GetString(5).Trim() ?? "",
                            AlumnoEncontrado = true
                        };
                    }
                    return null;
                }
            }
        }

        public async Task AgregarAlumnoAsync(AlumnoModel alumnoModel)
        {
            var sql = @"INSERT INTO Alumno(CodAlumno, NombreCompleto, FechaNacimiento, DNI, Telefono, Correo)
                       VALUES (@CodAlumno, @NombreCompleto, @FechaNacimiento,@DNI, @Telefono, @Correo)";
            using( var conn = new SqlConnection(_connectionString))
                using ( var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CodAlumno", alumnoModel.Codigo);
                cmd.Parameters.AddWithValue("@NombreCompleto", alumnoModel.NombreCompleto);
                cmd.Parameters.AddWithValue("@FechaNacimiento", alumnoModel.FechaNacimiento);
                cmd.Parameters.AddWithValue("@DNI", alumnoModel.DNI);
                cmd.Parameters.AddWithValue("@Telefono", (object)alumnoModel.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Correo", (object)alumnoModel.Correo ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
     

    }
}
