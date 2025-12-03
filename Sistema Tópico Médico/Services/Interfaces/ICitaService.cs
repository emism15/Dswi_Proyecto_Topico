using System.Collections.Generic;
using System.Threading.Tasks;
using TopicoMedico.Models;
using TopicoMedico.Models.Entities;
using TopicoMedico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace TopicoMedico.Services.Interfaces
{

    public interface ICitaService
    {
        Task<List<Cita>> ObtenerTodasAsync();
        Task<List<Cita>> ObtenerPorPacienteAsync(int pacienteId, string estado = null);
        Task<List<Cita>> ObtenerPorEnfermeraAsync(int enfermeraId);
        Task<List<Cita>> ObtenerProximasAsync(int diasAnticipacion);
        Task<Cita> ObtenerPorIdAsync(int citaId);
        Task<bool> CrearAsync(CitaViewModel model, int usuarioRegistroId);
        Task<bool> AtenderAsync(AtenderCitaViewModel model, int enfermeraId);
        Task<bool> CancelarAsync(int citaId);
    }
}