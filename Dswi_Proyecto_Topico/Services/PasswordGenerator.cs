using System.Security.Cryptography;
using System.Text;

namespace Dswi_Proyecto_Topico.Services
{
    public static class PasswordGenerator
    {
        public static string Generar()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$";
            var data = new byte[10];
            using var rgn = RandomNumberGenerator.Create();
            rgn.GetBytes(data);

            var sb = new StringBuilder();
            foreach (var b in data)
                sb.Append(chars[b % chars.Length]);
            return $"Tmp#{RandomNumberGenerator.GetInt32(1000, 9999)}Aa";
        }

    }
}
