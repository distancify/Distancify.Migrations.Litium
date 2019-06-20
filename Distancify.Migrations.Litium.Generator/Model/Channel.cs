using System;
using System.Collections.Generic;
using System.Text;
using Distancify.Migrations.Litium.Generator.Data;

namespace Distancify.Migrations.Litium.Generator.Model
{
    public class Channel : SeedWithFields, IMigrationSeed
    {
        public string Id { get; set; }

        public IEnumerable<Country> Countries { get; set; }

        public FieldTemplate FieldTemplate { get; set; }

        public void Add(Repositories repos)
        {
            if (Countries == null)
            {
                return;
            }

            foreach (var c in Countries)
            {
                c.Add(repos);
            }

            repos.Channels.Add(this);
        }

    }
}

