namespace Dswi_Proyecto_Topico.Services.Interface
{
    public interface IEmailService
    {
        Task EnviarCredencialesAsync(string correo, string usuario, string password);
    }
}
