using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.Data.SqlClient;

namespace Dswi_Proyecto_Topico.Data
{
    public class AtencionRepository
    {
        private readonly string _connectionString;

        public AtencionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /*
        public async Task<int> RegistrarAtencionAsync(AtencionModel atencion)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1) INSERTAR LA ATENCIÓN
                        var sqlAtencion = @"
                    INSERT INTO Atencion (AlumnoId, Fecha, Hora, Detalles, Diagnostico)
                    VALUES (@AlumnoId, @Fecha, @Hora, @Detalles, @Diagnostico);
                    SELECT SCOPE_IDENTITY();
                ";

                        int atencionId;

                        using (var cmd = new SqlCommand(sqlAtencion, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AlumnoId", atencion.AlumnoId);
                            cmd.Parameters.AddWithValue("@Fecha", atencion.Fecha);
                            cmd.Parameters.AddWithValue("@Hora", atencion.Hora);
                            cmd.Parameters.AddWithValue("@Detalles", atencion.Detalles);
                            cmd.Parameters.AddWithValue("@Diagnostico",
                                (object)atencion.Diagnostico ?? DBNull.Value);

                            atencionId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        }

                        // 2) PROCESAR CADA MEDICAMENTO ADMINISTRADO
                        foreach (var med in atencion.Medicamentos)
                        {
                            // 2.1 Verificar stock disponible
                            var sqlCheckStock = @"SELECT Stock FROM Medicamento WHERE MedicamentoId = @MedicamentoId";

                            int stockActual;

                            using (var cmdStock = new SqlCommand(sqlCheckStock, conn, transaction))
                            {
                                cmdStock.Parameters.AddWithValue("@MedicamentoId", med.MedicamentoId);

                                object result = await cmdStock.ExecuteScalarAsync();
                                stockActual = Convert.ToInt32(result);
                            }

                            if (stockActual < med.CantidadAdministrada)
                            {
                                throw new Exception($"El medicamento ID {med.MedicamentoId} no tiene stock suficiente. Disponible: {stockActual}");
                            }

                            // 2.2 Insertar detalle de la atención
                            var sqlInsertMed = @"
                        INSERT INTO AtencionMedicamento (AtencionId, MedicamentoId, CantidadAdministrada)
                        VALUES (@AtencionId, @MedicamentoId, @CantidadAdministrada); ";

                            using (var cmdInsert = new SqlCommand(sqlInsertMed, conn, transaction))
                            {
                                cmdInsert.Parameters.AddWithValue("@AtencionId", atencionId);
                                cmdInsert.Parameters.AddWithValue("@MedicamentoId", med.MedicamentoId);
                                cmdInsert.Parameters.AddWithValue("@CantidadAdministrada", med.CantidadAdministrada);

                                await cmdInsert.ExecuteNonQueryAsync();
                            }

                            // 2.3 Descontar stock
                            var sqlUpdateStock = @"
                        UPDATE Medicamento
                        SET Stock = Stock - @Cantidad
                        WHERE MedicamentoId = @MedicamentoId;";

                            using (var cmdUpdate = new SqlCommand(sqlUpdateStock, conn, transaction))
                            {
                                cmdUpdate.Parameters.AddWithValue("@Cantidad", med.CantidadAdministrada);
                                cmdUpdate.Parameters.AddWithValue("@MedicamentoId", med.MedicamentoId);

                                await cmdUpdate.ExecuteNonQueryAsync();
                            }
                        }

                        // 3) CONFIRMAR TODO
                        await transaction.CommitAsync();
                        return atencionId;
                    }
                    catch
                    {
                        // Si algo sale mal, deshacemos todo
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }
        */
       
    }
}
