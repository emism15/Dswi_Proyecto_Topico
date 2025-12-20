using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TopicoMedico.Data;
using TopicoMedico.Models.Entities;
using TopicoMedico.Models.ViewModels;
using TopicoMedico.Services.Interfaces;

namespace TopicoMedico.Services
{
    public class ProductoService : IProductoService
    {
        private readonly TopicoDbContext _context;

        public ProductoService(TopicoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            return await _context.Productos
                .Where(p => p.Activo)
                .Include(p => p.Categoria)
                .OrderBy(p => p.NombreProducto)
                .ToListAsync();
        }


        public async Task<List<Producto>> ObtenerActivosAsync()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo)
                .OrderBy(p => p.NombreProducto)
                .ToListAsync();
        }

        public async Task<Producto> ObtenerPorIdAsync(int productoId)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.ProductoId == productoId);
        }

        public async Task<bool> CrearAsync(ProductoViewModel model)
        {
            // VALIDACIÓN: Formato LETRAS-NÚMEROS (ejemplo: MED-123)
            if (!Regex.IsMatch(model.CodigoProducto, @"^[A-Z]+-\d+$"))
            {
                throw new ArgumentException("El código debe seguir el formato LETRAS-NÚMEROS (ejemplo: MED-123)");
            }

            // VALIDACIÓN: Código único
            if (await _context.Productos.AnyAsync(p => p.CodigoProducto == model.CodigoProducto))
            {
                throw new ArgumentException("El código de producto ya existe");
            }

            var producto = new Producto
            {
                CategoriaId = model.CategoriaId,
                CodigoProducto = model.CodigoProducto.ToUpper(),
                NombreProducto = model.NombreProducto,
                Descripcion = model.Descripcion,
                TipoProducto = model.TipoProducto,
                UnidadMedida = model.UnidadMedida,
                StockActual = model.StockActual,
                StockMinimo = model.StockMinimo,
                PrecioUnitario = (decimal)model.PrecioUnitario,
                Laboratorio = model.Laboratorio,
                RequiereReceta = model.RequiereReceta
            };

            PrepararDatosAutomaticos(producto);

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActualizarAsync(ProductoViewModel model)
        {
            var producto = await _context.Productos.FindAsync(model.ProductoId);
            if (producto == null) return false;

            // No permitir cambiar el código una vez creado
            producto.NombreProducto = model.NombreProducto;
            producto.Descripcion = model.Descripcion;
            producto.StockMinimo = model.StockMinimo;
            producto.PrecioUnitario = (decimal)model.PrecioUnitario;
            producto.Laboratorio = model.Laboratorio;
            producto.RequiereReceta = model.RequiereReceta;
            producto.Activo = model.Activo;

            // Solo recalcular vencimiento si cambia el tipo
            if (producto.TipoProducto != model.TipoProducto)
            {
                producto.TipoProducto = model.TipoProducto;

                if (model.TipoProducto == "Medicamento")
                    producto.FechaVencimiento = DateTime.Now.AddYears(2);
                else
                    producto.FechaVencimiento = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int productoId)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return false;

            producto.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Producto>> ObtenerConStockBajoAsync()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.StockActual <= p.StockMinimo && p.Activo)
                .OrderBy(p => p.StockActual)
                .ToListAsync();
        }

        public async Task<List<Producto>> ObtenerPorVencerAsync(int diasAnticipacion)
        {
            var fechaLimite = DateTime.Now.AddDays(diasAnticipacion);
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.FechaVencimiento != null &&
                           p.FechaVencimiento <= fechaLimite &&
                           p.Activo)
                .OrderBy(p => p.FechaVencimiento)
                .ToListAsync();
        }

        public async Task<bool> ActualizarStockAsync(int productoId, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return false;

            producto.StockActual += cantidad;
            await _context.SaveChangesAsync();
            return true;
        }
        private void PrepararDatosAutomaticos(Producto producto)
        {
            // Lote automático: CODIGO-YYYYMMDD
            producto.Lote = $"{producto.CodigoProducto}-{DateTime.Now:yyyyMMdd}";

            // Fecha de vencimiento SOLO para medicamentos
            if (producto.TipoProducto == "Medicamento")
            {
                producto.FechaVencimiento = DateTime.Now.AddYears(2);
            }
            else
            {
                producto.FechaVencimiento = null;
            }

            producto.FechaRegistro = DateTime.Now;
            producto.Activo = true;
        }

    }
}