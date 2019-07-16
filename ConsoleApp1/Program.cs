using Distancify.Migrations.Litium;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            new LitiumMigrationCmdlet()
            {
                ConfigFileName = @"C:\Projects\Distancify.Migrations.Litium\ConsoleApp1\migrationBuilder.yml"
            }.ProcessRecord();

            Console.WriteLine("Done");
            Console.Read();
        }
    }
}
