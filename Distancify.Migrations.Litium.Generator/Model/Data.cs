using Distancify.Migrations.Litium.Generator.Data;
using System.Collections.Generic;
using System.Text;

namespace Distancify.Migrations.Litium.Generator.Model
{
    public class Data
    {
        public IEnumerable<Channel> Channels { get; set; }

        public void Add(Repositories repos)
        {
            foreach (var c in Channels)
            {
                c.Add(repos);
            }
        }
    }
}

