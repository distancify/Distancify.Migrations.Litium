using System;
using System.Text;
using Distancify.Migrations.Litium.Generator.Data;

namespace Distancify.Migrations.Litium.Generator.Model
{
    public class Country : IMigrationSeed
    {
        public string Id { get; set; }

        public Currency Currency { get; set; }

        public void Add(Repositories repos)
        {
            Currency?.Add(repos);
            repos.Countries.Add(this);
        }
    }
}