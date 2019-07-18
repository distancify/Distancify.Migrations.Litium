using Distancify.Migrations.Litium.SeedBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMigrations
{
    class Program
    {
        static void Main(string[] args)
        {
            var generator = new LitiumMigrationGenerator(new GraphqlClient());
            var files = generator.GenerateAllFiles(ConfigurationReader.ReadConfigurationsFromFile("migrationConfiguration.yml"));

            foreach (var f in files)
            {
                if (File.Exists(f.Filepath))
                    File.Delete(f.Filepath);
                File.WriteAllText(f.Filepath, f.Content);
            }
        }
    }
}
