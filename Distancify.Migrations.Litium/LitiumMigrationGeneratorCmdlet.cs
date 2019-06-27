using Distancify.Migrations.Litium.SeedBuilder;
using System.IO;
using System.Management.Automation;

namespace Distancify.Migrations.Litium.MigrationGenerator
{

    [Cmdlet(VerbsCommon.Push, "LitiumMigration")]
    [OutputType(typeof(void))]
    public class LitiumMigrationGeneratorCmdlet : Cmdlet
    {

        [Parameter(Mandatory = true)]
        public string ConfigFileName { get; set; }

        protected override void ProcessRecord()
        {
            base.BeginProcessing();
            var generator = new LitiumMigrationGenerator(new GraphqlClient());
            var files = generator.GenerateAllFiles(ConfigurationReader.ReadConfiguration(ConfigFileName));

            foreach(var f in files)
            {
                if (File.Exists(f.Filepath))
                    File.Delete(f.Filepath);
                File.WriteAllText(f.Filepath, f.Content);
            }
        }


    }
}