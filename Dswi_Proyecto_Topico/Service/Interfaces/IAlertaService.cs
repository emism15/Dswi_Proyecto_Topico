using System.Collections.Generic;
using System.Threading.Tasks;
using Dswi_Proyecto_Topico.Models;
using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Dswi_Proyecto_Topico.Service.Interfaces
{

    public interface IAlertaService
    {
        Task<List<Alerta>> ObtenerPorRolAsync(string rol);
        Task<List<Alerta>> ObtenerPorUsuarioAsync(int usuarioId);
        Task GenerarAlertasStockAsync();
        Task GenerarAlertasCitasAsync();
        Task MarcarComoLeidaAsync(int alertaId);
    }
}