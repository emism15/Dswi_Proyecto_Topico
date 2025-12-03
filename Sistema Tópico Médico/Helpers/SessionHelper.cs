using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace TopicoMedico.Helpers
{
    public static class SessionHelper
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }

        public static bool IsAuthenticated(this ISession session)
        {
            return session.GetInt32("UsuarioId").HasValue;
        }

        public static int? GetUsuarioId(this ISession session)
        {
            return session.GetInt32("UsuarioId");
        }

        public static string GetNombreUsuario(this ISession session)
        {
            return session.GetString("NombreUsuario");
        }

        public static string GetNombreCompleto(this ISession session)
        {
            return session.GetString("NombreCompleto");
        }

        public static int? GetRolId(this ISession session)
        {
            return session.GetInt32("RolId");
        }

        public static string GetNombreRol(this ISession session)
        {
            return session.GetString("NombreRol");
        }
    }
}
