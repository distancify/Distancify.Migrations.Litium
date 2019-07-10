using Distancify.Migrations.Litium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            new LitiumMigrationCmdlet()
            {
                ConfigFileName = @"C:\Projects\Distancify.Migrations.Litium\Distancify.Migrations.Litium\first.yml"
            }.ProcessRecord();

            Console.WriteLine("Done");
            Console.Read();
        }
    }
}
