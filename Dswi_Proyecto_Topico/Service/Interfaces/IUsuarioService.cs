using System.Collections.Generic;
using System.Threading.Tasks;
using Dswi_Proyecto_Topico.Models;
using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Dswi_Proyecto_Topico.Service.Interfaces
{

    public interface IUsuarioService
    {
        Task<List<Usuario>> ObtenerTodosAsync();
        Task<List<Usuario>> ObtenerPorRolAsync(int rolId);
        Task<Usuario> ObtenerPorIdAsync(int usuarioId);
        Task<bool> CrearAsync(UsuarioViewModel model, int usuarioCreadorId);
        Task<bool> ActualizarAsync(UsuarioViewModel model);
        Task<bool> EliminarAsync(int usuarioId);
        Task<bool> ExisteUsuarioAsync(string nombreUsuario);
        Task<bool> ExisteDNIAsync(string dni);
    }
}
