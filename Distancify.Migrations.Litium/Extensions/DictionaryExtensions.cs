using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Distancify.Migrations.Litium.Extensions
{
    public static class DictionaryExtensions
    {
        public static string GetMigration(this IDictionary<string, string> dictionary, int spacing = 4)
            => $"new Dictionary<string, string> \r\n {new string('\t', spacing)}{{\r\n{new string('\t', spacing + 1)}" +
                   string.Join($",\r\n{new string('\t', spacing + 1)}", 
                               dictionary.Select(e => $"{{\"{e.Key}\", {e.Value.ToLiteral()}}}")) +
                   $"\r\n{new string('\t', spacing)}}}";

    }
}
