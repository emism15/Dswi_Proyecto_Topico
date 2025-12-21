using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Dswi_Proyecto_Topico.Service.Interfaces
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