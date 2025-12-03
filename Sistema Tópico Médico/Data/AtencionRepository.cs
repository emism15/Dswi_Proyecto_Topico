using Microsoft.Data.SqlClient;
using TopicoMedico.Models.ViewModels;

namespace TopicoMedico.Data
{
    public class AtencionRepository
    {
        private readonly string _connectionString;
        public AtencionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        // Obtener lista de medicamentos
        public async Task<List<Medicamento>> ObtenerMedicamentosAsync()
        {
            var lista = new List<Medicamento>();
            var sql = "SELECT MedicamentoId, Nombre, Stock FROM Medicamento";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Medicamento
                        {
                            MedicamentoId = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Stock = reader.GetInt32(2)
                        });
                    }
                }
            }
            return lista;
        }
        // Registrar atención
        public async Task<int> AgregarAtencionAsync(Atencion atencion)
        {
            int atencionId;
            var sql = @"INSERT INTO Atencion (AlumnoId, Fecha, Hora, Detalles, Diagnostico)
                        VALUES (@AlumnoId, @Fecha, @Hora, @Detalles, @Diagnostico);
                        SELECT SCOPE_IDENTITY();";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@AlumnoId", atencion.AlumnoId);
                cmd.Parameters.AddWithValue("@Fecha", atencion.Fecha);
                cmd.Parameters.AddWithValue("@Hora", atencion.Hora);
                cmd.Parameters.AddWithValue("@Detalles", atencion.Detalles);
                cmd.Parameters.AddWithValue("@Diagnostico", (object)atencion.Diagnostico ?? DBNull.Value);

                await conn.OpenAsync();
                atencionId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            // Registrar medicamentos y descontar stock
            if (atencion.Medicamentos != null)
            {
                foreach (var med in atencion.Medicamentos)
                {
                    await RegistrarMedicamentoAtencionAsync(atencionId, med.MedicamentoId, med.Cantidad);
                    await ActualizarStockMedicamentoAsync(med.MedicamentoId, med.Cantidad);
                }
            }

            return atencionId;
        }

        // Registrar Medicamento usado en atención
        private async Task RegistrarMedicamentoAtencionAsync(int atencionId, int medicamentoId, int cantidad)
        {
            var sql = @"INSERT INTO AtencionMedicamento (AtencionId, MedicamentoId, Cantidad)
                        VALUES (@AtencionId, @MedicamentoId, @Cantidad)";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@AtencionId", atencionId);
                cmd.Parameters.AddWithValue("@MedicamentoId", medicamentoId);
                cmd.Parameters.AddWithValue("@Cantidad", cantidad);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // Descontar stock de medicamento
        private async Task ActualizarStockMedicamentoAsync(int medicamentoId, int cantidadUsada)
        {
            var sql = @"UPDATE Medicamento SET Stock = Stock - @Cantidad WHERE MedicamentoId = @MedicamentoId";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Cantidad", cantidadUsada);
                cmd.Parameters.AddWithValue("@MedicamentoId", medicamentoId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

    }
}
