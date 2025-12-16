using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.Data.SqlClient;
namespace Dswi_Proyecto_Topico.Data
{
    public class EnfermeraRepository
    {
        private readonly string _connectionString;

        public EnfermeraRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public DashboardEnfermeraViewModel ObtenerDashboard()
        {
            var vm = new DashboardEnfermeraViewModel();

            using SqlConnection cn = new SqlConnection(_connectionString);
            cn.Open();

            // 🔹 Citas pendientes
            SqlCommand cmdPendientes = new SqlCommand(
                "SELECT COUNT(*) FROM Citas WHERE EstadoCita = 'Pendiente'", cn);
            vm.CitasPendientes = (int)cmdPendientes.ExecuteScalar();

            // 🔹 Pacientes atendidos hoy
            SqlCommand cmdAtendidos = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM Citas 
                WHERE EstadoCita = 'Atendida'
                AND CAST(FechaAtencion AS DATE) = CAST(GETDATE() AS DATE)", cn);
            vm.PacientesAtendidosHoy = (int)cmdAtendidos.ExecuteScalar();

            // 🔹 Próximas citas
            SqlCommand cmdCitas = new SqlCommand(@"
                SELECT TOP 5 c.CitaId, c.FechaCita, u.NombreCompleto
                FROM Citas c
                INNER JOIN Usuarios u ON c.PacienteId = u.UsuarioId
                WHERE c.FechaCita >= GETDATE()
                ORDER BY c.FechaCita", cn);

            using var dr = cmdCitas.ExecuteReader();
            while (dr.Read())
            {
                vm.ProximasCitas.Add(new Cita
                {
                    CitaId = (int)dr["CitaId"],
                    FechaCita = (DateTime)dr["FechaCita"],
                    Paciente = new Usuario
                    {
                        NombreCompleto = dr["NombreCompleto"].ToString()
                    }
                });
            }

            return vm;
        }




    }
}