using Distancify.Migrations.Litium.LitiumGraphqlModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Generator
{
    public abstract class Repository<T>
        where T : GraphQlObject
        
    {
        protected readonly IDictionary<string, T> Items = new Dictionary<string, T>();

        public void AddOrMerge(T graphQlItem)
        {
            if (Items.TryGetValue(graphQlItem.Id, out var existing))
            {
                MergeGraphQlData(graphQlItem, existing);
            }
            else
            {
                Items.Add(graphQlItem.Id, graphQlItem);
            }
        }

        private static void MergeGraphQlData(T source, T dest)
        {
            var props = typeof(T).GetProperties().Where(x => x.CanRead && x.CanWrite).ToList();

            foreach (var p in props)
            {
                if (p.GetValue(source) == null)
                {
                    continue;
                }

                p.SetValue(dest, p.GetValue(source, null), null);
            }
        }

        public int NumberOfItems
        {
            get
            {
                return Items.Count();
            }
        }

        public abstract void AppendMigration(StringBuilder builder);

        //protected void AppendFields(SeedWithFields source, StringBuilder builder)
        //{
        //    foreach (var f in source.Fields)
        //    {
        //        if (f.Value is JObject value)
        //        {
        //            var isLocalized = false;
        //            foreach (var c in CultureInfo.GetCultures(CultureTypes.AllCultures))
        //            {
        //                if (value.TryGetValue(c.Name.Replace("-", "_"), out var localizedValue))
        //                {
        //                    builder.AppendLine($"\t\t\t\t.WithField(\"{f.Key}\", \"{c.Name}\", \"{localizedValue}\")");
        //                    isLocalized = true;
        //                }
        //            }

        //            if (isLocalized)
        //            {
        //                continue;
        //            }
        //        }

        //        builder.AppendLine($"\t\t\t\t.WithField(\"{f.Key}\", \"{f.Value["value"]}\")");
        //    }
        //}
    }
}
