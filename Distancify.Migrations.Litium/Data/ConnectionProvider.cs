using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Data
{
    internal static class ConnectionProvider
    {
        public static SqlConnection Open()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["FoundationConnectionString"].ConnectionString);
        }
    }
}
