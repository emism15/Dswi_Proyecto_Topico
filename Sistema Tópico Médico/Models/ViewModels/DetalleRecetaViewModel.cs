using System.ComponentModel.DataAnnotations;

namespace TopicoMedico.Models.ViewModels
{
    public class DetalleRecetaViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un producto")]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Dosis")]
        [StringLength(100)]
        public string Dosis { get; set; }

        [Display(Name = "Frecuencia")]
        [StringLength(100)]
        public string Frecuencia { get; set; }

        [Display(Name = "Duración del Tratamiento")]
        [StringLength(50)]
        public string Duracion { get; set; }

        [Display(Name = "Indicaciones")]
        [StringLength(300)]
        public string Indicaciones { get; set; }

        // Para vista
        public string NombreProducto { get; set; }
        public string TipoProducto { get; set; }
        public int StockDisponible { get; set; }
    }
}
