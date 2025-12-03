using Dswi_Proyecto_Topico.Models.Entities;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.EntityFrameworkCore; // Necesario si no se pone en CompraService

namespace Dswi_Proyecto_Topico.Services.Interfaces
{
    public interface ICompraService
    {
        // 🟢 MÉTODOS DE OBTENCIÓN FALTANTES EN LA IMPLEMENTACIÓN INICIAL
        Task<List<Compra>> ObtenerTodasAsync();
        Task<List<Compra>> ObtenerPorFechasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<Compra> ObtenerPorIdAsync(int compraId);

        Task<bool> CrearAsync(CompraViewModel model, int usuarioRegistroId);
        Task<decimal> ObtenerMontoTotalMesAsync();
    }
}