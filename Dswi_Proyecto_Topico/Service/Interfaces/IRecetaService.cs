using System.Collections.Generic;
using System.Threading.Tasks;
using Dswi_Proyecto_Topico.Models;
using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Dswi_Proyecto_Topico.Service.Interfaces
{
    // ========== IRecetaService ==========
    public interface IRecetaService
    {
        Task<List<Receta>> ObtenerPorAlumnoAsync(int pacienteId);
        Task<Receta> ObtenerPorIdAsync(int recetaId);
        Task<bool> CrearAsync(RecetaViewModel model);
        Task<List<DetalleReceta>> ObtenerDetallesAsync(int recetaId);
    }
  
}