using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Distancify.Migrations.Litium.Migrations
{
    public class BootstrapCustomers : LitiumMigration
    {
        public override void Apply()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FoundationConnectionString"].ConnectionString))
            {
                connection.Open();

                var definitions = new HashSet<string>();
                var command = new SqlCommand("SELECT Id FROM [Security].[OperationDefinition]", connection);
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        definitions.Add(reader.GetString(0));
                    }
                }
                finally
                {
                    reader.Close();
                }

                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/SystemSettings");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Sales/Settings");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Customers/Content");
                EnsureOperation(connection, definitions, "EntityOperationDefinitionEntity", "Entity/Write");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Sales/UI");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Customers/UI");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Products/Settings");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Media/UI");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Websites/UI");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Products/UI");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Customers/Settings");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Media/Content");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Websites/Content");
                EnsureOperation(connection, definitions, "EntityOperationDefinitionEntity", "Entity/Publish");
                EnsureOperation(connection, definitions, "EntityOperationDefinitionEntity", "Entity/Read");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Media/Settings");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Sales/Content");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Products/Content");
                EnsureOperation(connection, definitions, "FunctionOperationDefinitionEntity", "Function/Websites/Settings");
            }

            Customers.GroupFieldTemplateSeed.Ensure(Constants.DefaultSystemGroupTemplate)
                .Commit();
            Customers.PersonFieldTemplateSeed.Ensure(Constants.DefaultSystemUserTemplate)
                .Commit();

            Customers.StaticGroupSeed.Ensure(Constants.Visitors, Constants.DefaultSystemGroupTemplate)
                .Commit();

            Customers.PersonSeed.EnsureSystem(Constants.DefaultSystemUserTemplate)
                .Commit();
            Customers.PersonSeed.EnsureEveryone(Constants.DefaultSystemUserTemplate)
                .WithGroupLink(Constants.Visitors)
                .Commit();
        }

        private void EnsureOperation(SqlConnection connection, ISet<string> definitions, string discriminator, string id)
        {
            if (!definitions.Contains(id))
            {
                var command = new SqlCommand("INSERT INTO [Security].[OperationDefinition] ([SystemId],[Discriminator],[Id]) VALUES (newId(), @discriminator, @id)", connection);
                command.Parameters.AddWithValue("@discriminator", discriminator);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }

        }
    }
}
