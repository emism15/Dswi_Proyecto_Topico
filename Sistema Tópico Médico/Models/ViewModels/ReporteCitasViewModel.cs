using System.ComponentModel.DataAnnotations;
using TopicoMedico.Models.Entities;

namespace TopicoMedico.Models.ViewModels
{
    public class ReporteCitasViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Desde")]
        public DateTime FechaInicio { get; set; } = DateTime.Now.AddMonths(-1);

        [DataType(DataType.Date)]
        [Display(Name = "Hasta")]
        public DateTime FechaFin { get; set; } = DateTime.Now;

        [Display(Name = "Estado")]
        public string EstadoCita { get; set; }

        public int TotalCitas { get; set; }
        public List<Cita> Citas { get; set; }
    }
}
