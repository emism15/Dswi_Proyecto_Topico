using Dswi_Proyecto_Topico.Services.Interface;
using System.Net;
using System.Net.Mail;

namespace Dswi_Proyecto_Topico.Services

{
    public class EmailService : IEmailService
    {
        public async Task EnviarCredencialesAsync(string correo, string usuario, string password)
        {
            var mensaje = new MailMessage();
            mensaje.From = new MailAddress("luzdanicabanillas18@gmail.com", "Sistema CiberTopico");
            mensaje.To.Add(correo);
            mensaje.Subject = "Credenciales de acceso";
            mensaje.Body = $@"
Estimado alumno:

Su cuenta ha sido creada correctamente.

Usuario: {usuario}
Contraseña temporal: {password}

Debe cambiar su contraseña al ingresar al sistema.
";

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    "luzdanicabanillas18@gmail.com",
                    "cguy vjeu nrhf ytie" // CLAVE DE APLICACIÓN
                )
            };

            await smtp.SendMailAsync(mensaje);
        }
    }
}
