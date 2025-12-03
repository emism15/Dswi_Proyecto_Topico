using System.Collections.Generic;
using System.Threading.Tasks;
using TopicoMedico.Models;
using TopicoMedico.Models.Entities;
using TopicoMedico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace TopicoMedico.Services.Interfaces
{
    // ========== IRecetaService ==========
    public interface IRecetaService
    {
        Task<List<Receta>> ObtenerPorPacienteAsync(int pacienteId);
        Task<Receta> ObtenerPorIdAsync(int recetaId);
        Task<bool> CrearAsync(RecetaViewModel model);
        Task<List<DetalleReceta>> ObtenerDetallesAsync(int recetaId);
    }
  
}