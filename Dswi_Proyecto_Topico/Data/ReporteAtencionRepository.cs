using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dswi_Proyecto_Topico.Data
{
    public class ReporteAtencionRepository
    {
        private readonly string _connectionString;

        public ReporteAtencionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task<ReporteAtencionModel> ObtenerReporteAtencionAsync(int atencionId)
        {
            using(var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_ObtenerReporteAtencion", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AtencionId", atencionId);

                await conn.OpenAsync();

                var checkReporteCmd = new SqlCommand(@"SELECT ReporteGenerado FROM Atencion WHERE AtencionId = @AtencionId", conn);
                checkReporteCmd.Parameters.AddWithValue("@AtencionId", atencionId);

                var reporteGenerado = await checkReporteCmd.ExecuteScalarAsync();

                if (reporteGenerado == null)
                {
                    return null;
                }

                if (Convert.ToBoolean(reporteGenerado))
                {
                    return null;
                }

                using ( var reader = await cmd.ExecuteReaderAsync())
                {
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
                        HoraAtencion =(TimeSpan)reader["Hora"],
                        DetallesClinicos = reader["DetallesClinicos"].ToString()!,
                        DiagnosticoPreliminar = reader["DiagnosticoPreliminar"].ToString()!,
                        Medicamentos = new List<ReporteMedicamentoModel>()
                    };

                    if(await reader.NextResultAsync())
                    {
                        while(await reader.ReadAsync())
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

        public async Task MarcarReporteGeneradoAsync(int atencionId)
        {
            var sql = @"UPDATE Atencion SET ReporteGenerado = 1,
                                                  FechaGeneracionReporte = GETDATE()
                                                  WHERE AtencionId = @AtencionId";
            using(var conn = new SqlConnection(_connectionString))
                using(var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@AtencionId", atencionId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
       

        public async Task<ReporteAtencionModel> ObtenerReporteParaPdfAsync(int atencionId)
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
