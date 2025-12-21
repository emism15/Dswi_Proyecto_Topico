using Dswi_Proyecto_Topico.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Dswi_Proyecto_Topico.Data;
using Dswi_Proyecto_Topico.Helpers;
using Dswi_Proyecto_Topico.Models.Entitties;

namespace Dswi_Proyecto_Topico.Service
{

    // ========== AlertaService ==========
    public class AlertaService : IAlertaService
    {
        private readonly TopicoDbContext _context;

        public AlertaService(TopicoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Alerta>> ObtenerPorRolAsync(string rol)
        {
            return await _context.Alertas
                .Where(a => a.RolDestino == rol && !a.Leida)
                .OrderByDescending(a => a.FechaGeneracion)
                .ToListAsync();
        }

        public async Task<List<Alerta>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _context.Alertas
                .Where(a => a.UsuarioDestinoId == usuarioId && !a.Leida)
                .OrderByDescending(a => a.FechaGeneracion)
                .ToListAsync();
        }

        public async Task GenerarAlertasStockAsync()
        {
            var productosStockBajo = await _context.Productos
                .Where(p => p.StockActual <= p.StockMinimo && p.Activo)
                .ToListAsync();

            foreach (var producto in productosStockBajo)
            {
                    var alertaExiste = await _context.Alertas.AnyAsync(a =>
                      a.TipoAlerta == "StockBajo" &&
                      a.ReferenciaId == producto.ProductoId &&
                      a.RolDestino == "Administrador" &&
                      !a.Leida);


                if (!alertaExiste)
                {
                    var alerta = new Alerta
                    {
                        TipoAlerta = "StockBajo",
                        Prioridad = "Alta",
                        Mensaje = $"Stock bajo del producto {producto.NombreProducto}",
                        ReferenciaId = producto.ProductoId,
                        TipoReferencia = "Producto",
                        RolDestino = "Administrador",   // ✅ CLAVE
                        FechaGeneracion = DateTime.Now,
                        Leida = false
                    };

                    _context.Alertas.Add(alerta);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task GenerarAlertasCitasAsync()
        {
            var citasProximas = await _context.Citas
                .Include(c => c.Alumno)
                .Where(c => c.FechaCita >= DateTime.Now &&
                           c.FechaCita <= DateTime.Now.AddHours(24) &&
                           c.EstadoCita == "Pendiente")
                .ToListAsync();

            foreach (var cita in citasProximas)
            {
                // Alerta para el paciente
                var alertaPaciente = new Alerta
                {
                    TipoAlerta = "CitaProxima",
                    Mensaje = $"Recordatorio: Tienes una cita el {cita.FechaCita:dd/MM/yyyy HH:mm}",
                    UsuarioDestinoId = cita.AlumnoId,
                    RolDestino = "Paciente",          // ✅
                    Prioridad = "Media",
                    ReferenciaId = cita.CitaId,
                    TipoReferencia = "Cita"
                };

                _context.Alertas.Add(alertaPaciente);

                // Alerta para enfermera
                var alertaEnfermera = new Alerta
                {
                    TipoAlerta = "CitaProxima",
                    Mensaje = $"Cita próxima: {cita.Alumno.NombreCompleto} el {cita.FechaCita:dd/MM/yyyy HH:mm}",
                    RolDestino = "Enfermera",
                    Prioridad = "Media",
                    ReferenciaId = cita.CitaId,
                    TipoReferencia = "Cita"
                };
                _context.Alertas.Add(alertaEnfermera);
            }

            await _context.SaveChangesAsync();
        }

        public async Task MarcarComoLeidaAsync(int alertaId)
        {
            var alerta = await _context.Alertas.FindAsync(alertaId);
            if (alerta != null)
            {
                alerta.Leida = true;
                alerta.FechaLectura = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}