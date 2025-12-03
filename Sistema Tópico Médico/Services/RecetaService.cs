using Microsoft.EntityFrameworkCore;
using TopicoMedico.Data;
using TopicoMedico.Models.Entities;
using TopicoMedico.Models.ViewModels;
using TopicoMedico.Services.Interfaces;

namespace TopicoMedico.Services
{
    public class RecetaService : IRecetaService
    {
        private readonly TopicoDbContext _context;

        public RecetaService(TopicoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Receta>> ObtenerPorPacienteAsync(int pacienteId)
        {
            return await _context.Recetas
                .Include(r => r.Cita)
                .Include(r => r.Enfermera)
                .Where(r => r.PacienteId == pacienteId)
                .OrderByDescending(r => r.FechaEmision)
                .ToListAsync();
        }

        public async Task<Receta> ObtenerPorIdAsync(int recetaId)
        {
            return await _context.Recetas
                .Include(r => r.Cita)
                .Include(r => r.Paciente)
                .Include(r => r.Enfermera)
                .FirstOrDefaultAsync(r => r.RecetaId == recetaId);
        }

        public async Task<bool> CrearAsync(RecetaViewModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // VALIDACIÓN: Verificar stock suficiente ANTES de crear la receta
                var productosInsuficientes = new List<string>();

                foreach (var detalle in model.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto == null)
                    {
                        throw new Exception($"Producto con ID {detalle.ProductoId} no encontrado");
                    }

                    if (producto.StockActual < detalle.Cantidad)
                    {
                        productosInsuficientes.Add($"{producto.NombreProducto} (Stock: {producto.StockActual}, Requerido: {detalle.Cantidad})");
                    }
                }

                // Si hay productos sin stock suficiente, lanzar excepción
                if (productosInsuficientes.Any())
                {
                    throw new Exception($"Stock insuficiente para: {string.Join(", ", productosInsuficientes)}");
                }

                // Crear receta
                var receta = new Receta
                {
                    CitaId = model.CitaId,
                    PacienteId = model.PacienteId,
                    EnfermeraId = model.EnfermeraId,
                    Indicaciones = model.Indicaciones,
                    Observaciones = model.Observaciones,
                    Estado = "Vigente",
                    FechaEmision = DateTime.Now
                };

                _context.Recetas.Add(receta);
                await _context.SaveChangesAsync();

                // Agregar detalles y descontar stock
                foreach (var detalle in model.Detalles)
                {
                    var detalleReceta = new DetalleReceta
                    {
                        RecetaId = receta.RecetaId,
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        Dosis = detalle.Dosis,
                        Frecuencia = detalle.Frecuencia,
                        Duracion = detalle.Duracion,
                        Indicaciones = detalle.Indicaciones
                    };
                    _context.DetallesReceta.Add(detalleReceta);

                    // DESCUENTO AUTOMÁTICO DE STOCK
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto != null)
                    {
                        producto.StockActual -= detalle.Cantidad;

                        // GENERAR ALERTA si el stock queda bajo el mínimo
                        if (producto.StockActual <= producto.StockMinimo)
                        {
                            var alerta = new Alerta
                            {
                                UsuarioDestinoId = model.EnfermeraId,
                                TipoAlerta = "Stock",
                                Mensaje = $"ALERTA: El producto '{producto.NombreProducto}' tiene stock bajo. Stock actual: {producto.StockActual}, Mínimo: {producto.StockMinimo}",
                                Leida = false,
                                FechaGeneracion = DateTime.Now
                            };
                            _context.Alertas.Add(alerta);

                            // También crear alerta para administrador
                            var adminUsers = await _context.Usuarios.Where(u => u.RolId == 1 && u.Activo).ToListAsync();
                            foreach (var admin in adminUsers)
                            {
                                var alertaAdmin = new Alerta
                                {
                                    UsuarioDestinoId = admin.UsuarioId,
                                    TipoAlerta = "Stock",
                                    Mensaje = $"STOCK CRÍTICO: '{producto.NombreProducto}' - Stock actual: {producto.StockActual}",
                                    Leida = false,
                                    FechaGeneracion = DateTime.Now
                                };
                                _context.Alertas.Add(alertaAdmin);
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al crear receta: {ex.Message}");
            }
        }

        public async Task<List<DetalleReceta>> ObtenerDetallesAsync(int recetaId)
        {
            return await _context.DetallesReceta
                .Include(d => d.Producto)
                .ThenInclude(p => p.Categoria)
                .Where(d => d.RecetaId == recetaId)
                .ToListAsync();
        }
    }
}