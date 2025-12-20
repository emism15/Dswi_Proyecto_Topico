using System.Collections.Generic;
using System.Threading.Tasks;
using TopicoMedico.Models;
using TopicoMedico.Models.Entities;
using TopicoMedico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace TopicoMedico.Services.Interfaces
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