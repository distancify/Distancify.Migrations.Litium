using Distancify.Migrations.Litium.Generator.Data;
using Distancify.Migrations.Litium.Generator.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;

namespace Distancify.Migrations.Litium.Generator
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
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            Config config;

            if (!File.Exists(ConfigFileName))
            {
                Console.WriteLine($"Could not find {ConfigFileName}");
                return;
            }
            config = deserializer.Deserialize<Config>(File.OpenText(ConfigFileName));


            var data = Fetch(config).GetAwaiter().GetResult();
            if(data == null)
            {
                throw new NullReferenceException("Data object from GrapQL is null, something might be wrong with the query")
            }
            var repositories = new Repositories();
            data.Add(repositories);
            var migration = repositories.ToMigration(config);

            if (File.Exists(config.Output))
                File.Delete(config.Output);
            File.WriteAllText(config.Output, migration);
        }

        public static async Task<ResponseModel> Fetch(Config config)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsync(config.Host + "/graphql", new StringContent(config.Query));

                return JsonConvert.DeserializeObject<ResponseModel>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}