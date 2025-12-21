using System.ComponentModel.DataAnnotations;

namespace Dswi_Proyecto_Topico.Models.Entitties
{
    public class Alumno
    {
        [Key]
        public int AlumnoId { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required, StringLength(10)]
        public string CodAlumno { get; set; }

        [Required, StringLength(120)]
        public string NombreCompleto { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required, StringLength(8)]
        public string DNI { get; set; }

        [Required, StringLength(15)]
        public string Telefono { get; set; }

        [Required, StringLength(100)]
        public string Correo { get; set; }

        // Navegación
        public ICollection<Cita> Citas { get; set; }
    }

}
