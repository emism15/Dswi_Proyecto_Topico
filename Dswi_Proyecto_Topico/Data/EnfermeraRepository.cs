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
            SELECT COUNT(*)  FROM Citas 
            WHERE EstadoCita = 'Atendida'
            AND CAST(FechaCita AS DATE) = CAST(GETDATE() AS DATE)", cn);
            vm.PacientesAtendidosHoy = (int)await cmdAtendidos.ExecuteScalarAsync();

            //Citas programas para hoy
            SqlCommand cmdCitasHoy = new SqlCommand(@"
            SELECT c.CitaId, c.FechaCita, a.NombreCompleto
             FROM Citas c
             INNER JOIN Alumno a ON c.AlumnoId = a.AlumnoId
             WHERE CAST(c.FechaCita AS DATE) = CAST(GETDATE() AS DATE)
              ORDER BY c.FechaCita", cn);


            using var dr = await cmdCitasHoy.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                vm.CitasHoy.Add(new Cita
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