using System.Text;
using Distancify.Migrations.Litium.Generator.Data;

namespace Distancify.Migrations.Litium.Generator.Model
{
    public class Currency : IMigrationSeed
    {
        public string Id { get; set; }
        public bool IsBaseCurrency { get; set; }

        public void Add(Repositories repos)
        {
            repos.Currencies.Add(this);
        }
    }
}