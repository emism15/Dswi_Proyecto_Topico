using System.ComponentModel.DataAnnotations;
using TopicoMedico.Models.Entities;

namespace TopicoMedico.Models.ViewModels
{
    public class ReporteComprasViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Desde")]
        public DateTime FechaInicio { get; set; } = DateTime.Now.AddMonths(-1);

        [DataType(DataType.Date)]
        [Display(Name = "Hasta")]
        public DateTime FechaFin { get; set; } = DateTime.Now;

        [Display(Name = "Proveedor")]
        public int? ProveedorId { get; set; }

        public decimal MontoTotal { get; set; }
        public List<Compra> Compras { get; set; }
        public List<Proveedor> Proveedores { get; set; }
    }
}
