using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dswi_Proyecto_Topico.Data
{
    public class HistorialReporteRepository
    {

        private readonly string _connectionString;

        public HistorialReporteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task<List<HistorialReporteAtencionModel>> ListarHistorialReporteAsync(string codigo, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var lista = new List<HistorialReporteAtencionModel>();

            using(var conn = new SqlConnection(_connectionString))
                using(var cmd = new SqlCommand("sp_ListarHistorialReportes", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CodAlumno", (object?)codigo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaInicio", (object?)fechaInicio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaFin", (object?)fechaFin ?? DBNull.Value);

                await conn.OpenAsync();
                using( var reader = await cmd.ExecuteReaderAsync())
                {
                    while( await reader.ReadAsync())
                    {
                        lista.Add(new HistorialReporteAtencionModel
                        {
                            AtencionId = (int)reader["AtencionId"],
                            Codigo = reader["CodAlumno"].ToString(),
                            NombreCompleto = reader["NombreCompleto"].ToString(),
                            FechaAtencion = (DateTime)reader["FechaAtencion"],
                            FechaGeneracionReporte = reader["FechaGeneracionReporte"] as DateTime?
                        });
                    }
                    return lista;
                }
            }
        }

        public async Task<ReporteAtencionModel> ObtenerReporteDetalleAsync(int atencionId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_ObtenerReporteAtencion", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AtencionId", atencionId);

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var reporte = new ReporteAtencionModel
            {
                AtencionId = atencionId,
                Codigo = reader["CodAlumno"].ToString()!,
                NombreCompleto = reader["NombreCompleto"].ToString()!,
                DNI = reader["DNI"].ToString()!,
                Telefono = reader["Telefono"].ToString()!,
                Correo = reader["Correo"].ToString()!,
                FechaAtencion = (DateTime)reader["Fecha"],
                HoraAtencion = (TimeSpan)reader["Hora"],
                DetallesClinicos = reader["DetallesClinicos"].ToString()!,
                DiagnosticoPreliminar = reader["DiagnosticoPreliminar"].ToString()!,
                Medicamentos = new List<ReporteMedicamentoModel>()
            };

            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    reporte.Medicamentos.Add(new ReporteMedicamentoModel
                    {
                        Nombre = reader["Nombre"].ToString()!,
                        Cantidad = (int)reader["Cantidad"]
                    });
                }
            }

            return reporte;
        }



    }
}
