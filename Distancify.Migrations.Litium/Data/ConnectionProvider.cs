using System.Configuration;
using System.Data.SqlClient;

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
