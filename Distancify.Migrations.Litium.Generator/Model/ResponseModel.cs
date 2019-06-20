using Distancify.Migrations.Litium.Generator.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Generator.Model
{
    public class ResponseModel
    {
        public Data Data { get; set; }

        public void Add(Repositories repos)
        {
            Data.Add(repos);
        }
    }
}

