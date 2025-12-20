using Dswi_Proyecto_Topico.Models.ViewModels;
using Microsoft.Reporting.NETCore;
using System.Data;

namespace Dswi_Proyecto_Topico.Helpers
{
    public class RdlcReportService
    {
        public byte[] GenerarReporteAtencionPdf(
            ReporteAtencionModel reporte, string rutaRdlc)
        {
            LocalReport report = new LocalReport();
            report.ReportPath = rutaRdlc;

            var dtAtencion = new DataTable("dsAtencion");
            dtAtencion.Columns.Add("CodAlumno");
            dtAtencion.Columns.Add("NombreCompleto");
            dtAtencion.Columns.Add("DNI");
            dtAtencion.Columns.Add("Telefono");
            dtAtencion.Columns.Add("Correo");
            dtAtencion.Columns.Add("Fecha");
            dtAtencion.Columns.Add("Hora");
            dtAtencion.Columns.Add("DetallesClinicos");
            dtAtencion.Columns.Add("DiagnosticoPreliminar");

            dtAtencion.Rows.Add(
                reporte.Codigo,
                reporte.NombreCompleto,
                reporte.DNI,
                reporte.Telefono,
                reporte.Correo,
                reporte.FechaAtencion.ToShortDateString(),
                reporte.HoraAtencion.ToString(@"hh\:mm"),
                reporte.DetallesClinicos,
                reporte.DiagnosticoPreliminar
                );

            var dtMedicamentos = new DataTable("dsMedicamentos");
            dtMedicamentos.Columns.Add("Nombre");
            dtMedicamentos.Columns.Add("Cantidad");

            foreach(var m in reporte.Medicamentos)
            {
                dtMedicamentos.Rows.Add(m.Nombre, m.Cantidad);
            }

            report.DataSources.Add(new ReportDataSource("dsAtencion", dtAtencion));
            report.DataSources.Add(new ReportDataSource("dsMedicamentos", dtMedicamentos));

            return report.Render("PDF");
        }
    }
}
