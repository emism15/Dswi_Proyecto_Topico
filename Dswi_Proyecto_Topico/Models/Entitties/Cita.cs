using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dswi_Proyecto_Topico.Models.Entitties
{
    public class Cita
    {
      
        public int CitaId { get; set; }

        public int AlumnoId { get; set; }

        public int? EnfermeraId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una fecha de cita")]
        [DataType(DataType.DateTime)]
        public DateTime FechaCita { get; set; }

        [Required(ErrorMessage = "El motivo de consulta es obligatorio")]
        [StringLength(300, MinimumLength = 10)]
        public string MotivoConsulta { get; set; }

        public string TipoCita { get; set; }


        [StringLength(20)]
        public string EstadoCita { get; set; } = "Pendiente"; // 'Pendiente', 'Atendida', 'Cancelada', 'NoAsistió'

      
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public DateTime? FechaAtencion { get; set; }

        public  Alumno Alumno { get; set; }

    }
}
