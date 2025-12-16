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
                cmd.Parameters.AddWithValue("@Edad", alumnoModel.Edad);
                cmd.Parameters.AddWithValue("@DNI", alumnoModel.DNI);
                cmd.Parameters.AddWithValue("@Telefono", (object)alumnoModel.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Correo", (object)alumnoModel.Correo ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }


        /*
        public async Task<AlumnoModel?> BuscarAlumnoPorCodigoAsync( string codigoAlumno)
        {
            var sql = "SELECT AlumnoId, CodigoAlumno, NombreCompleto, FechaNacimiento, Edad, DNI, Telefono, Correo FROM Alumno WHERE CodigoAlumno = @CodigoAlumno";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CodigoAlumno", codigoAlumno);
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        return new AlumnoModel
                        {
                            AlumnoId= reader.GetInt32(0),
                            CodigoAlumno = reader.GetString(1),
                            NombreCompleto = reader.GetString(2),
                            FechaNacimiento = reader.GetDateTime(3),
                            Edad = reader.GetInt32(4),
                            DNI = reader.GetString(5),
                            Telefono = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Correo = reader.IsDBNull(7) ? null : reader.GetString(7)
                        };
                    }
                    return null;
                }
            }

        }
        
        */

}
}
