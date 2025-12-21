using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dswi_Proyecto_Topico.Data
{
    public class CitaRepository
    {

      
        private readonly string _connectionString;

        public CitaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        // Listar citas con filtro 
        public async Task<List<Cita>> ObtenerCitasAsync(string estado = null)
        {
            var lista = new List<Cita>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                string sql = @"
        SELECT c.CitaId, c.FechaCita, c.MotivoConsulta,c.TipoCita, c.EstadoCita,
            a.AlumnoId, a.NombreCompleto, a.DNI, a.CodAlumno
        FROM Citas c
        INNER JOIN Alumno a ON c.AlumnoId = a.AlumnoId
        WHERE (@estado IS NULL OR c.EstadoCita = @estado)
        ORDER BY c.FechaCita DESC";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@estado", (object)estado ?? DBNull.Value);

                await cn.OpenAsync();
                var dr = await cmd.ExecuteReaderAsync();

                while (await dr.ReadAsync())
                {
                    lista.Add(new Cita
                    {
                        CitaId = dr.GetInt32(0),
                        FechaCita = dr.GetDateTime(1),
                        MotivoConsulta = dr.GetString(2),
                        TipoCita = dr.GetString(3),
                        EstadoCita = dr.GetString(4),

                        Alumno = new Alumno
                        {
                            AlumnoId = dr.GetInt32(5),
                            NombreCompleto = dr.GetString(6),
                            DNI = dr.GetString(7),
                            Codigo = dr.GetString(8)
                        }
                    });
                }
            }

            return lista;
        }


        // Listar citas 
        public async Task<List<Cita>> ListarAsync(string estado)
        {
            var lista = new List<Cita>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Citas";

                if (!string.IsNullOrEmpty(estado))
                    sql += " WHERE EstadoCita = @estado";

                SqlCommand cmd = new SqlCommand(sql, cn);

                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@estado", estado);

                await cn.OpenAsync();
                var dr = await cmd.ExecuteReaderAsync();

                while (await dr.ReadAsync())
                {
                    lista.Add(new Cita
                    {
                        CitaId = (int)dr["CitaId"],
                        AlumnoId = (int)dr["AlumnoId"],
                        EnfermeraId = dr["EnfermeraId"] as int?,
                        FechaCita = (DateTime)dr["FechaCita"],
                        MotivoConsulta = dr["MotivoConsulta"].ToString(),
                        TipoCita = dr["TipoCita"].ToString(),
                        EstadoCita = dr["EstadoCita"].ToString()
                    });
                }
            }

            return lista;
        }


        // Contar citas por estado
        public async Task<Dictionary<string, int>> ContarAsync()
        {
            var dic = new Dictionary<string, int>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT EstadoCita, COUNT(*) Total FROM Citas GROUP BY EstadoCita", cn);

                await cn.OpenAsync();
                var dr = await cmd.ExecuteReaderAsync();

                while (await dr.ReadAsync())
                {
                    dic.Add(dr["EstadoCita"].ToString(), (int)dr["Total"]);
                }
            }

            return dic;
        }


        //Registrar nueva cita
        public async Task<int> RegistrarCitaAsync(RegistrarCitaViewModel model)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        DateTime fechaCita = model.Fecha.Date + model.Hora;

                        var cmdSP = new SqlCommand("sp_RegistrarCita", conn, tran);
                        cmdSP.CommandType = CommandType.StoredProcedure;

                        cmdSP.Parameters.AddWithValue("@AlumnoId", model.AlumnoId);
                        cmdSP.Parameters.AddWithValue("@FechaCita", fechaCita);
                        cmdSP.Parameters.AddWithValue("@MotivoConsulta", model.MotivoConsulta);
                        cmdSP.Parameters.AddWithValue("@TipoCita", model.TipoCita);

                        int citaId = Convert.ToInt32(await cmdSP.ExecuteScalarAsync());

                        tran.Commit();
                        return citaId;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }


        public async Task ActualizarEstadoAsync(int citaId, string nuevoEstado)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("UPDATE Citas SET EstadoCita = @Estado WHERE CitaId = @CitaId", conn);
                cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
                cmd.Parameters.AddWithValue("@CitaId", citaId);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<Cita> ObtenerPorIdAsync(int citaId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
            SELECT c.CitaId, c.FechaCita, c.MotivoConsulta, c.TipoCita, c.EstadoCita,
                   a.AlumnoId, a.NombreCompleto, a.DNI, a.CodAlumno
            FROM Citas c
            INNER JOIN Alumno a ON c.AlumnoId = a.AlumnoId
            WHERE c.CitaId = @CitaId";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@CitaId", citaId);

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        return new Cita
                        {
                            CitaId = dr.GetInt32(0),
                            FechaCita = dr.GetDateTime(1),
                            MotivoConsulta = dr.GetString(2),
                            TipoCita = dr.GetString(3),
                            EstadoCita = dr.GetString(4),

                            Alumno = new Alumno
                            {
                                AlumnoId = dr.GetInt32(5),
                                NombreCompleto = dr.GetString(6),
                                DNI = dr.GetString(7),
                                Codigo = dr.GetString(8)
                            }
                        };
                    }
                }
            }

            return null; // si no se encuentra la cita
        }





        ////Cambiar estado de cita
        //public async Task CambiarEstadoAsync(int citaId, string nuevoEstado)
        //{
        //    string[] estadosValidos = { "Pendiente", "Atendida", "Cancelada" };

        //    if (!estadosValidos.Contains(nuevoEstado))
        //        throw new Exception("Estado inválido");

        //    using (SqlConnection cn = new SqlConnection(_connectionString))
        //    {
        //        string sql = @"
        //UPDATE Citas
        //SET EstadoCita = @estado
        //WHERE CitaId = @id";

        //        SqlCommand cmd = new SqlCommand(sql, cn);
        //        cmd.Parameters.AddWithValue("@estado", nuevoEstado);
        //        cmd.Parameters.AddWithValue("@id", citaId);

        //        await cn.OpenAsync();
        //        await cmd.ExecuteNonQueryAsync();
        //    }
        //}


        public async Task<RegistrarCitaViewModel?> BuscarAlumnoPorCodigoAsync(string codigo)
        {
            var sql = @"SELECT AlumnoId, CodAlumno, NombreCompleto, DNI
                       FROM Alumno 
                       WHERE LOWER(CodAlumno) = LOWER(@CodAlumno)";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CodAlumno", codigo.Trim().ToLower());

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new RegistrarCitaViewModel
                        {
                            AlumnoId = reader.GetInt32(0),
                            Codigo = reader.GetString(1).Trim(),
                            NombreCompleto = reader.GetString(2).Trim(),
                            DNI = reader.GetString(3).Trim(),                           
                            AlumnoEncontrado = true
                        };
                    }
                    return null;
                }
            }
        }

        // Verificar cruce de horario
        public async Task<bool> ExisteCruceAsync(DateTime fechaHora)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                string sql = @" SELECT COUNT(*)  FROM Citas
                WHERE FechaCita BETWEEN DATEADD(minute,-30,@f)
                AND DATEADD(minute,30,@f)
                AND EstadoCita = 'Pendiente'";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@f", fechaHora);

                await cn.OpenAsync();
                int total = (int)await cmd.ExecuteScalarAsync();

                return total > 0;
            }
        }



    }
}
