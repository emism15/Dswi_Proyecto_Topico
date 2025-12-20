// Archivo: Models/Entities/Alerta.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace TopicoMedico.Models.Entities
{
    public class Alerta
    {
        [Key]
        public int AlertaId { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoAlerta { get; set; } // StockBajo, ProductoVencido, etc.

        [Required]
        [StringLength(20)]
        public string Prioridad { get; set; } // Crítica, Alta, Media

        [Required]
        public string Mensaje { get; set; }

        public DateTime FechaGeneracion { get; set; } = DateTime.Now;

        public bool Leida { get; set; } = false;

        public DateTime? FechaLectura { get; set; }

        // Referencia al objeto que generó la alerta
        public int? ReferenciaId { get; set; }

        [StringLength(50)]
        public string TipoReferencia { get; set; }

        // ✅ Usuario específico destino
        public int? UsuarioDestinoId { get; set; }
        public Usuario? UsuarioDestino { get; set; }

        // ✅ NUEVO: Rol destino de la alerta
        [Required]
        [StringLength(30)]
        public string RolDestino { get; set; } // Administrador, Enfermera, Paciente
    }
}
