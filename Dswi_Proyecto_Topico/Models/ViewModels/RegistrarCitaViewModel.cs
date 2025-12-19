namespace Dswi_Proyecto_Topico.Models.ViewModels
{
    public class RegistrarCitaViewModel
    {
        public int AlumnoId { get; set; }

        //solo para mostrar 
        public string NombreAlumno { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string MotivoConsulta { get; set; }

        public string TipoCita { get; set; }



    } 



}
