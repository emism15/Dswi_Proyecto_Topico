using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dswi_Proyecto_Topico.Models.Entitties
{
    public class Alerta
    {
        [Key]
        public int AlertaId { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoAlerta { get; set; } // 'StockBajo', 'ProductoVencido', 'CitaProxima', 'CitaHoy'


        [Required]
        [StringLength(300)]
        public string Mensaje { get; set; }


        [StringLength(50)]
        public string RolDestino { get; set; }


        public int? UsuarioDestinoId { get; set; }

        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
        public bool Leida { get; set; } = false;
        public DateTime? FechaLectura { get; set; }


        [StringLength(20)]
        public string Prioridad { get; set; } = "Media"; // 'Baja', 'Media', 'Alta', 'Crítica'

        public int? ReferenciaId { get; set; }


        [StringLength(50)]
        public string TipoReferencia { get; set; }

        // Navegación
        [ForeignKey("UsuarioDestinoId")]
        public virtual Usuario UsuarioDestino { get; set; }
    }
}
