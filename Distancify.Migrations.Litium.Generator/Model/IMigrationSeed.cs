using Distancify.Migrations.Litium.Generator.Data;
using System.Text;

namespace Distancify.Migrations.Litium.Generator.Model
{
    public interface IMigrationSeed
    {
        string Id { get; set; }

        void Add(Repositories repos);
    }
}

