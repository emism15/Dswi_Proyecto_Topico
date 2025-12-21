using Dswi_Proyecto_Topico.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Helpers;
using Dswi_Proyecto_Topico.Models.Entitties;
using Dswi_Proyecto_Topico.Models.ViewModels;

public class UsuarioService : IUsuarioService
{
    private readonly TopicoDbContext _context;

    public UsuarioService(TopicoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync();
    }

    public async Task<List<Usuario>> ObtenerPorRolAsync(int rolId)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .Where(u => u.RolId == rolId && u.Activo)
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync();
    }

    public async Task<Usuario> ObtenerPorIdAsync(int usuarioId)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
    }

    public async Task<bool> CrearAsync(UsuarioViewModel model, int usuarioCreadorId)
    {
        // 🔴 VALIDACIONES CRÍTICAS
        if (!model.RolId.HasValue)
            throw new Exception("Debe seleccionar un rol.");

        if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == model.NombreUsuario))
            throw new Exception("El nombre de usuario ya existe.");

        var usuario = new Usuario
        {
            RolId = model.RolId.Value,
            NombreCompleto = model.NombreCompleto,
            DNI = model.DNI,
            Email = model.Email,
            Telefono = model.Telefono,
            NombreUsuario = model.NombreUsuario,
            PasswordHash = PasswordHelper.HashPassword(model.Contraseña ?? "Temporal123!"),
            FechaNacimiento = model.FechaNacimiento,
            Direccion = model.Direccion,
            Activo = model.Activo,
            DebecambiarContraseña = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Auditoría
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var auditoria = new Auditoria
            {
                UsuarioId = usuarioCreadorId,
                Accion = "INSERT",
                Tabla = "Usuarios",
                RegistroId = usuario.UsuarioId,
                FechaAccion = DateTime.Now,
                IPAddress = "Sistema",
                ValoresNuevos = $"Usuario: {usuario.NombreUsuario}, Rol: {usuario.RolId}"
            };

            _context.Auditorias.Add(auditoria);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> ActualizarAsync(UsuarioViewModel model)
    {
        var usuario = await _context.Usuarios.FindAsync(model.UsuarioId);
        if (usuario == null) return false;

        usuario.NombreCompleto = model.NombreCompleto;
        usuario.Email = model.Email;
        usuario.Telefono = model.Telefono;
        usuario.FechaNacimiento = model.FechaNacimiento;
        usuario.Direccion = model.Direccion;
        usuario.Activo = model.Activo;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int usuarioId)
    {
        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null) return false;

        usuario.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExisteUsuarioAsync(string nombreUsuario)
    {
        return await _context.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario);
    }

    public async Task<bool> ExisteDNIAsync(string dni)
    {
        return await _context.Usuarios.AnyAsync(u => u.DNI == dni);
    }
}
