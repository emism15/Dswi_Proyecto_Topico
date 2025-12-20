using TopicoMedico.Models.Entities;

public class Producto
{
    public int ProductoId { get; set; }

    public int CategoriaId { get; set; }

    public string CodigoProducto { get; set; }

    public string NombreProducto { get; set; }

    public string TipoProducto { get; set; }

    public string Descripcion { get; set; }

    public string UnidadMedida { get; set; }

    public int StockActual { get; set; }

    public int StockMinimo { get; set; }

    public decimal PrecioUnitario { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public string Laboratorio { get; set; }

    public string Lote { get; set; }

    public bool RequiereReceta { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool Activo { get; set; }

    // Relación
    public CategoriaProducto Categoria { get; set; }
}
