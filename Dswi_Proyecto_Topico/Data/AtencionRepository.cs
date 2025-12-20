using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dswi_Proyecto_Topico.Data
{
    public class AtencionRepository
    {
        private readonly string _connectionString;

        public AtencionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

       
        public async Task RegistrarAtencionCompletaAsync (AtencionModel model, List<AtencionMedicamentoModel> medicamentos)
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using(var tran = conn.BeginTransaction())
                {
                    try
                    {
                        DateTime fechaValida = model.FechaAtencion < new DateTime(1755, 1, 1) ?
                            DateTime.Now.Date : model.FechaAtencion.Date;

                        TimeSpan horaValida = (model.HoraAtencion < TimeSpan.Zero || model.HoraAtencion >= TimeSpan.FromDays(1))
                            ? DateTime.Now.TimeOfDay : model.HoraAtencion;

                        var cmdAtencion = new SqlCommand(@"INSERT INTO Atencion(AlumnoId, Fecha, Hora, DetallesClinicos, DiagnosticoPreliminar)
                      OUTPUT INSERTED.AtencionId
                      VALUES(@AlumnoId, @Fecha, @Hora, @DetallesClinicos, @DiagnosticoPreliminar)",
                      conn, tran);

                        cmdAtencion.Parameters.AddWithValue("@AlumnoId", model.AlumnoId);
                        cmdAtencion.Parameters.AddWithValue("@Fecha", fechaValida);
                        cmdAtencion.Parameters.AddWithValue("@Hora", horaValida);
                        cmdAtencion.Parameters.AddWithValue("@DetallesClinicos", model.DetallesClinicos);
                        cmdAtencion.Parameters.AddWithValue("@DiagnosticoPreliminar", model.DiagnosticoPreliminar);

                        int atencionId = (int)await cmdAtencion.ExecuteScalarAsync();

                        foreach( var item in medicamentos)
                        {
                            var cmdMed = new SqlCommand(@"INSERT INTO AtencionMedicamento(AtencionId, MedicamentoId, Cantidad)
                                                          VALUES (@AtencionId, @MedicamentoId, @Cantidad)", conn, tran);

                            cmdMed.Parameters.AddWithValue("@AtencionId", atencionId);
                            cmdMed.Parameters.AddWithValue("@MedicamentoId", item.MedicamentoId);
                            cmdMed.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                            await cmdMed.ExecuteNonQueryAsync();

                        }

                        var cmdSP = new SqlCommand("sp_RegistrarAtencion", conn, tran);
                        cmdSP.CommandType = CommandType.StoredProcedure;
                        cmdSP.Parameters.AddWithValue("@AtencionId", atencionId);
                        await cmdSP.ExecuteNonQueryAsync();

                        tran.Commit();

                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
     

        public async Task<List<MedicamentoModel>> BuscarMedicamentoAsync(string filtro)
        {
            var lista = new List<MedicamentoModel>();
            var sql = @"SELECT MedicamentoId, Nombre, Stock, Estado 
                        FROM Medicamento 
                        WHERE Estado = 1 AND Nombre LIKE @Filtro";
            using( var conn = new SqlConnection(_connectionString))
                using ( var cmd= new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Filtro", "%" + filtro.Trim() + "%");

                await conn.OpenAsync();
                using( var reader = await cmd.ExecuteReaderAsync())
                {
                    while ( await reader.ReadAsync())
                    {
                        lista.Add(new MedicamentoModel
                        {
                            MedicamentoId = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Stock = reader.GetInt32(2),
                            Estado = reader.GetBoolean(3)
                        });
                    }
                }
                return lista;
            }
        }

        


    }
}
