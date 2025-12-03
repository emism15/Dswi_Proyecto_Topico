using TopicoMedico.Models.Entities;
using TopicoMedico.Models.ViewModels;
using Microsoft.EntityFrameworkCore; // Necesario si no se pone en CompraService

namespace TopicoMedico.Services.Interfaces
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