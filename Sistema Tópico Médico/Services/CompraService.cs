using TopicoMedico.Data;
using TopicoMedico.Models.Entities;
using TopicoMedico.Models.ViewModels;
using TopicoMedico.Services.Interfaces;
using Microsoft.EntityFrameworkCore; // 🟢 CORRECCIÓN: Necesario para usar SumAsync, ToListAsync, FindAsync

namespace TopicoMedico.Services
{
    public class CompraService : ICompraService
    {
        private readonly TopicoDbContext _context;

        public CompraService(TopicoDbContext context)
        {
            _context = context;
        }

        // 🟢 Implementación de métodos de la interfaz (Obligatorios)
        public async Task<List<Compra>> ObtenerTodasAsync()
        {
            return await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.UsuarioRegistro)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
        }

        public async Task<List<Compra>> ObtenerPorFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.UsuarioRegistro)
                .Where(c => c.FechaCompra >= fechaInicio && c.FechaCompra <= fechaFin)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
        }

        public async Task<Compra> ObtenerPorIdAsync(int compraId)
        {
            return await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.UsuarioRegistro)
                .Include(c => c.DetallesCompra).ThenInclude(dc => dc.Producto)
                .FirstOrDefaultAsync(c => c.CompraId == compraId);
        }

        // -----------------------------------------------------------

        public async Task<bool> CrearAsync(CompraViewModel model, int usuarioRegistroId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var compra = new Compra
                {
                    ProveedorId = model.ProveedorId,
                    NumeroComprobante = model.NumeroComprobante,
                    TipoComprobante = model.TipoComprobante,
                    UsuarioRegistroId = usuarioRegistroId,
                    MontoTotal = model.MontoTotal, // Recibe el monto calculado por JS
                    Observaciones = model.Observaciones,
                    Estado = "Completada",
                    FechaCompra = model.FechaCompra,
                };

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                // Agregar detalles y actualizar stock
                foreach (var detalle in model.Detalles)
                {
                    var detalleCompra = new DetalleCompra
                    {
                        CompraId = compra.CompraId,
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Subtotal = detalle.Subtotal,
                        FechaVencimiento = detalle.FechaVencimiento,
                        Lote = detalle.Lote // Recibe el Lote del ViewModel
                    };
                    _context.DetallesCompras.Add(detalleCompra);

                    // Actualizar stock, precio y vencimiento del producto
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto != null)
                    {
                        producto.StockActual += detalle.Cantidad;
                        producto.PrecioUnitario = detalle.PrecioUnitario;

                        if (detalle.FechaVencimiento.HasValue)
                            producto.FechaVencimiento = detalle.FechaVencimiento;

                        if (!string.IsNullOrEmpty(detalle.Lote))
                            producto.Lote = detalle.Lote;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw; 
            }

        }

        public async Task<decimal> ObtenerMontoTotalMesAsync()
        {
            var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var finMes = inicioMes.AddMonths(1).AddDays(-1);

            // 🟢 CORRECCIÓN: El using Microsoft.EntityFrameworkCore; hace que SumAsync funcione
            return await _context.Compras
                .Where(c => c.FechaCompra >= inicioMes && c.FechaCompra <= finMes && c.Estado == "Completada")
                .SumAsync(c => c.MontoTotal);
        }
    }
}