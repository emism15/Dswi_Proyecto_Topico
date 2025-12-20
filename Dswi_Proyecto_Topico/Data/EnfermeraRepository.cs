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

        public async Task<DashboardEnfermeraViewModel> ObtenerDashboardAsync()
        {
            var vm = new DashboardEnfermeraViewModel();

            using SqlConnection cn = new SqlConnection(_connectionString);
            await cn.OpenAsync();

            // Citas pendientes
            SqlCommand cmdPendientes = new SqlCommand(
                "SELECT COUNT(*) FROM Citas WHERE EstadoCita = 'Pendiente'", cn);
            vm.CitasPendientes = (int)await cmdPendientes.ExecuteScalarAsync();

            // Pacientes atendidos hoy
            SqlCommand cmdAtendidos = new SqlCommand(@"
        SELECT COUNT(*) 
        FROM Citas 
        WHERE EstadoCita = 'Atendida'
        AND CAST(FechaCita AS DATE) = CAST(GETDATE() AS DATE)", cn);
            vm.PacientesAtendidosHoy = (int)await cmdAtendidos.ExecuteScalarAsync();

            SqlCommand cmdCitas = new SqlCommand(@"
             SELECT TOP 5 
             c.CitaId, 
             c.FechaCita, 
             a.NombreCompleto
              FROM Citas c
              INNER JOIN Alumno a ON c.AlumnoId = a.AlumnoId
             WHERE c.FechaCita >= GETDATE()
               ORDER BY c.FechaCita", cn);

            using var dr = await cmdCitas.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                vm.ProximasCitas.Add(new Cita
                {
                    CitaId = dr.GetInt32(0),
                    FechaCita = dr.GetDateTime(1),
                    Alumno = new Alumno
                    {
                        NombreCompleto = dr.GetString(2)
                    }
                });
            }

            return vm;
        }



    }
}