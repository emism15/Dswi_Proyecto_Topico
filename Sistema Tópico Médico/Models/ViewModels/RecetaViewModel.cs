using System.ComponentModel.DataAnnotations;
using TopicoMedico.Models.Entities;

namespace TopicoMedico.Models.ViewModels
{
    public class RecetaViewModel
    {
        public int RecetaId { get; set; }

        [Required]
        public int CitaId { get; set; }

        [Required]
        public int PacienteId { get; set; }

        public int EnfermeraId { get; set; }

        [Display(Name = "Indicaciones Generales")]
        [StringLength(500)]
        public string Indicaciones { get; set; }

        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        // Información de la cita
        public string NombrePaciente { get; set; }
        public string Diagnostico { get; set; }
        public DateTime FechaCita { get; set; }

        // Detalles de medicamentos/implementos
        public List<DetalleRecetaViewModel> Detalles { get; set; } = new List<DetalleRecetaViewModel>();
        public List<Producto> ProductosDisponibles { get; set; }
    }

}
