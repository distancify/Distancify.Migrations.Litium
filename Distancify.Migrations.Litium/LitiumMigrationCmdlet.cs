using Distancify.Migrations.Litium.SeedBuilder;
using System.IO;

namespace Distancify.Migrations.Litium
{

    //[Cmdlet(VerbsCommon.Push, "LitiumMigration")]
    //[OutputType(typeof(void))]
    public class LitiumMigrationCmdlet //: Cmdlet
    {

        //[Parameter]
        public string ConfigFileName { get; set; } = "migrationConfiguration.yml";

        public void ProcessRecord()
        {
            //base.BeginProcessing();
            var generator = new LitiumMigrationGenerator(new GraphqlClient());
            var files = generator.GenerateAllFiles(ConfigurationReader.ReadConfigurationsFromFile(ConfigFileName));

            foreach (var f in files)
            {
                if (File.Exists(f.Filepath))
                    File.Delete(f.Filepath);
                File.WriteAllText(f.Filepath, f.Content);
            }
        }


    }
}