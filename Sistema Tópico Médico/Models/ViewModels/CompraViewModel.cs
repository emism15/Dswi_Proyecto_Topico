using System.ComponentModel.DataAnnotations;
using TopicoMedico.Models.Entities;

namespace TopicoMedico.Models.ViewModels
{
    public class CompraViewModel
    {
        public int CompraId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un proveedor")]
        [Display(Name = "Proveedor")]
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El número de comprobante es obligatorio")]
        [Display(Name = "Número de Comprobante")]
        public string NumeroComprobante { get; set; }

        [Required(ErrorMessage = "El tipo de comprobante es obligatorio")]
        [Display(Name = "Tipo de Comprobante")]
        public string TipoComprobante { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Compra")]
        public DateTime FechaCompra { get; set; } = DateTime.Now;

        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Display(Name = "Monto Total")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal MontoTotal { get; set; }

        public List<DetalleCompraViewModel> Detalles { get; set; } = new List<DetalleCompraViewModel>();
        public List<Proveedor> ProveedoresDisponibles { get; set; }
        public List<Producto> ProductosDisponibles { get; set; }
    }
}
