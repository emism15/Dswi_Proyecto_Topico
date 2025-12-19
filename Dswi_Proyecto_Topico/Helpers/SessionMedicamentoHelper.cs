using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.Json;

namespace Dswi_Proyecto_Topico.Helpers
{
    public static class SessionMedicamentoHelper
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string Key)
        {
            var value = session.GetString(Key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
