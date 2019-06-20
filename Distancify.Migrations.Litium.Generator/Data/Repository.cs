using Distancify.Migrations.Litium.Generator.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Generator.Data
{
    public abstract class Repository<T>
        where T : IMigrationSeed
    {
        protected readonly IDictionary<string, T> Items = new Dictionary<string, T>();

        public void Add(T item)
        {
            if (Items.TryGetValue(item.Id, out var existing))
            {
                Merge(item, existing);
            }
            else
            {
                Items.Add(item.Id, item);
            }
        }

        private static void Merge(T source, T dest)
        {
            var props = typeof(T).GetProperties().Where(x => x.CanRead && x.CanWrite).ToList();

            foreach (var p in props)
            {
                p.SetValue(dest, p.GetValue(source, null), null);
            }
        }

        public abstract void AppendMigration(StringBuilder builder);

        protected void AppendFields(SeedWithFields source, StringBuilder builder)
        {
            foreach (var f in source.Fields)
            {
                if (f.Value is JObject value)
                {
                    var isLocalized = false;
                    foreach (var c in CultureInfo.GetCultures(CultureTypes.AllCultures))
                    {
                        if (value.TryGetValue(c.Name.Replace("-", "_"), out var localizedValue))
                        {
                            builder.AppendLine($"\t\t\t\t.WithField(\"{f.Key}\", \"{c.Name}\", \"{localizedValue}\")");
                            isLocalized = true;
                        }
                    }

                    if (isLocalized)
                    {
                        continue;
                    }
                }

                builder.AppendLine($"\t\t\t\t.WithField(\"{f.Key}\", \"{f.Value["value"]}\")");
            }
        }
    }
}
