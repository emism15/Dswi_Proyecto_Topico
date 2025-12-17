using Dswi_Proyecto_Topico.Models.Entitties;
using Microsoft.Data.SqlClient;

namespace Dswi_Proyecto_Topico.Data
{
    public class CitaRepository
    {

      
        private readonly string _connectionString;

        public CitaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        //
        public List<Cita> ObtenerCitas(string estado = null)
        {
            var lista = new List<Cita>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                string sql = @"
        SELECT 
            c.CitaId, c.FechaCita, c.MotivoConsulta, c.EstadoCita,
            u.UsuarioId, u.NombreCompleto, u.DNI
        FROM Citas c
        INNER JOIN Usuarios u ON c.PacienteId = u.UsuarioId
        WHERE (@estado IS NULL OR c.EstadoCita = @estado)
        ORDER BY c.FechaCita DESC";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@estado", (object)estado ?? DBNull.Value);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Cita
                    {
                        CitaId = dr.GetInt32(0),
                        FechaCita = dr.GetDateTime(1),
                        MotivoConsulta = dr.GetString(2),
                        EstadoCita = dr.GetString(3),

                        Paciente = new Usuario
                        {
                            UsuarioId = dr.GetInt32(4),
                            NombreCompleto = dr.GetString(5),
                            DNI = dr.GetString(6)
                        }
                    });
                }
            }

            return lista;
        }





        //Listar citas con filtro
        public List<Cita> Listar(string estado)
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

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Cita
                    {
                        CitaId = (int)dr["CitaId"],
                        PacienteId = (int)dr["PacienteId"],
                        EnfermeraId = dr["EnfermeraId"] as int?,
                        FechaCita = (DateTime)dr["FechaCita"],
                        MotivoConsulta = dr["MotivoConsulta"].ToString(),
                        EstadoCita = dr["EstadoCita"].ToString()
                    });
                }
            }
            return lista;
        }


        //contadores
        public Dictionary<string, int> Contar()
        {
            var dic = new Dictionary<string, int>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT EstadoCita, COUNT(*) Total FROM Citas GROUP BY EstadoCita", cn);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                    dic.Add(dr["EstadoCita"].ToString(), (int)dr["Total"]);
            }

            return dic;
        }


        //Registrar nueva cita
        public void Registrar(Cita c)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"INSERT INTO Citas
            (PacienteId, FechaCita, MotivoConsulta, Observaciones, EstadoCita)
            VALUES (@paciente, @fecha, @motivo, @obs, 'Pendiente')", cn);

                cmd.Parameters.AddWithValue("@paciente", c.PacienteId);
                cmd.Parameters.AddWithValue("@fecha", c.FechaCita);
                cmd.Parameters.AddWithValue("@motivo", c.MotivoConsulta);
                cmd.Parameters.AddWithValue("@obs", c.Observaciones ?? "");

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //Cambiar estado de cita
        public void CambiarEstado(int id, string estado)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Citas SET EstadoCita=@estado WHERE CitaId=@id", cn);

                cmd.Parameters.AddWithValue("@estado", estado);
                cmd.Parameters.AddWithValue("@id", id);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }






    }
}
