using System.Collections.Generic;
using System.Threading.Tasks;
using Dswi_Proyecto_Topico.Models;
using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Dswi_Proyecto_Topico.Service.Interfaces
{


    public interface IProductoService
    {
        Task<List<Producto>> ObtenerTodosAsync();
        Task<List<Producto>> ObtenerActivosAsync();
        Task<Producto> ObtenerPorIdAsync(int productoId);
        Task<bool> CrearAsync(ProductoViewModel model);
        Task<bool> ActualizarAsync(ProductoViewModel model);
        Task<bool> EliminarAsync(int productoId);
        Task<List<Producto>> ObtenerConStockBajoAsync();
        Task<List<Producto>> ObtenerPorVencerAsync(int diasAnticipacion);
        Task<bool> ActualizarStockAsync(int productoId, int cantidad);
    }
}