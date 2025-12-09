namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class AtencionMedicamentoModel
    {
        public int AtencionMedicamentoId { get; set; }
        public int AtencionId { get; set; }
        public int MedicamentoId { get; set; }
        public int CantidadAdministrada { get; set; }
        public MedicamentoModel Medicamento { get; set; }
    }
}
