using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TopicoMedico.Data;
using TopicoMedico.Helpers;
using TopicoMedico.Models.Entities;
using TopicoMedico.Models.ViewModels;
using TopicoMedico.Services.Interfaces;

namespace TopicoMedico.Services
{


    // ========== CitaService ==========
    public class CitaService : ICitaService
    {
        private readonly TopicoDbContext _context;

        public CitaService(TopicoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cita>> ObtenerTodasAsync()
        {
            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Enfermera)
                .OrderByDescending(c => c.FechaCita)
                .ToListAsync();
        }

        public async Task<List<Cita>> ObtenerPorPacienteAsync(int pacienteId, string estado = null)
        {
            var query = _context.Citas
                .Include(c => c.Enfermera)
                .Where(c => c.PacienteId == pacienteId);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(c => c.EstadoCita == estado);

            return await query.OrderByDescending(c => c.FechaCita).ToListAsync();
        }

        public async Task<List<Cita>> ObtenerPorEnfermeraAsync(int enfermeraId)
        {
            return await _context.Citas
                .Include(c => c.Paciente)
                .Where(c => c.EnfermeraId == enfermeraId)
                .OrderByDescending(c => c.FechaCita)
                .ToListAsync();
        }

        public async Task<List<Cita>> ObtenerProximasAsync(int diasAnticipacion)
        {
            var fechaLimite = DateTime.Now.AddDays(diasAnticipacion);
            return await _context.Citas
                .Include(c => c.Paciente)
                .Where(c => c.FechaCita <= fechaLimite &&
                           c.FechaCita >= DateTime.Now &&
                           c.EstadoCita == "Pendiente")
                .OrderBy(c => c.FechaCita)
                .ToListAsync();
        }

        public async Task<Cita> ObtenerPorIdAsync(int citaId)
        {
            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Enfermera)
                .FirstOrDefaultAsync(c => c.CitaId == citaId);
        }

        public async Task<bool> CrearAsync(CitaViewModel model, int usuarioRegistroId)
        {
            var cita = new Cita
            {
                PacienteId = model.PacienteId,
                FechaCita = model.FechaCita,
                MotivoConsulta = model.MotivoConsulta,
                Observaciones = model.Observaciones,
                EstadoCita = "Pendiente"
            };

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();

            // Auditoría
            var auditoria = new Auditoria
            {
                UsuarioId = usuarioRegistroId,
                Accion = "INSERT",
                Tabla = "Citas",
                RegistroId = cita.CitaId
            };
            _context.Auditorias.Add(auditoria);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AtenderAsync(AtenderCitaViewModel model, int enfermeraId)
        {
            var cita = await _context.Citas.FindAsync(model.CitaId);
            if (cita == null) return false;

            // Crear JSON de signos vitales
            var signosVitales = JsonSerializer.Serialize(new
            {
                temperatura = model.Temperatura,
                presionArterial = model.PresionArterial,
                pulso = model.Pulso,
                saturacionO2 = model.SaturacionO2,
                frecuenciaRespiratoria = model.FrecuenciaRespiratoria
            });

            cita.EnfermeraId = enfermeraId;
            cita.Diagnostico = model.Diagnostico;
            cita.Observaciones = model.Observaciones;
            cita.SignosVitales = signosVitales;
            cita.EstadoCita = "Atendida";
            cita.FechaAtencion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelarAsync(int citaId)
        {
            var cita = await _context.Citas.FindAsync(citaId);
            if (cita == null) return false;

            cita.EstadoCita = "Cancelada";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

