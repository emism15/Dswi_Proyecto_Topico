namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class AtencionMedicamentoModel
    {
        public int MedicamentoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int StockDisponible { get; set; }
        public int Cantidad { get; set; }
        
    }
}
