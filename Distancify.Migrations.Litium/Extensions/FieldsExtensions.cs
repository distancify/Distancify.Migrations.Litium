using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Extensions
{
    public static class FieldsExtensions
    {
        public static List<FieldData> GetFieldData(this Dictionary<string, Field> fields)
            => fields?.Where(f => f.Value.Value != null || f.Value.Localizations != null)
                      .Select(f => f.Value.Localizations?.Select(l => new FieldData(f.Key, l.Value, l.Culture)) ?? new[] { new FieldData(f.Key, f.Value.Value) })
                      .SelectMany(f => f).Where(f => f.Value != null).ToList() ?? new List<FieldData>();
    }
}
