using System.Collections.Generic;
using System.Threading.Tasks;
using Dswi_Proyecto_Topico.Models;
using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Dswi_Proyecto_Topico.Service.Interfaces
{

    public interface ICitaService
    {
        Task<List<Cita>> ObtenerTodasAsync();
        Task<List<Cita>> ObtenerPorAlumnoAsync(int alumnoId, string estado = null);
        Task<List<Cita>> ObtenerPorEnfermeraAsync(int enfermeraId);
        Task<List<Cita>> ObtenerProximasAsync(int diasAnticipacion);
        Task<Cita> ObtenerPorIdAsync(int citaId);
        Task<bool> CrearAsync(CitaViewModel model, int usuarioRegistroId);
        Task<bool> AtenderAsync(AtenderCitaViewModel model, int enfermeraId);
        Task<bool> CancelarAsync(int citaId);
    }
}