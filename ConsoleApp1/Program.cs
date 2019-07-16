using Distancify.Migrations.Litium;
using System;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            new LitiumMigrationCmdlet()
            {
                ConfigFileName = Directory.GetCurrentDirectory() + @"\migrationBuilder.yml"
            }.ProcessRecord();

            Console.WriteLine("Done");
            Console.Read();
        }
    }
}
