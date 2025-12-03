using System.Collections.Generic;
using System.Threading.Tasks;
using TopicoMedico.Models;
using TopicoMedico.Models.Entities;
using TopicoMedico.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace TopicoMedico.Services.Interfaces
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
