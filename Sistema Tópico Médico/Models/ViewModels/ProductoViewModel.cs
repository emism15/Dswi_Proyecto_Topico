using System.ComponentModel.DataAnnotations;
using TopicoMedico.Models.Entities;

namespace TopicoMedico.Models.ViewModels
{
    public class ProductoViewModel
    {
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El código del producto es obligatorio")]
        [Display(Name = "Código")]
        public string CodigoProducto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [Display(Name = "Nombre del Producto")]
        public string NombreProducto { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El tipo de producto es obligatorio")]
        [Display(Name = "Tipo")]
        public string TipoProducto { get; set; }

        [Display(Name = "Unidad de Medida")]
        public string UnidadMedida { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Stock Actual")]
        public int StockActual { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El stock mínimo debe ser mayor a 0")]
        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; }

        [Display(Name = "Precio Unitario")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal? PrecioUnitario { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Vencimiento")]
        public DateTime? FechaVencimiento { get; set; }

        [Display(Name = "Laboratorio")]
        public string Laboratorio { get; set; }

        [Display(Name = "Lote")]
        public string Lote { get; set; }

        [Display(Name = "Requiere Receta")]
        public bool RequiereReceta { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        public List<CategoriaProducto> CategoriasDisponibles { get; set; }
    }

}
