using Distancify.Migrations.Litium.Data;
using Distancify.Migrations.Litium.Seeds;
using Distancify.Migrations.Litium.Seeds.Customer;
using Distancify.Migrations.Litium.Seeds.Media;
using Litium.Media;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Distancify.Migrations.Litium.Seeds.Websites;

namespace Distancify.Migrations.Litium.Migrations
{
    public class Bootstrap : LitiumMigration
    {
        public override void Apply()
        {
            using (var connection = ConnectionProvider.Open())
            {
                connection.Open();
                EnsureOperations(connection);
                EnsureModules(connection);
            }

            UrlRedirectSeed.EnsureBatch()
                .Add("/", "~/Litium/", true)
                .Commit();

            GroupFieldTemplateSeed.Ensure(Constants.DefaultSystemGroupTemplate)
                .Commit();
            PersonFieldTemplateSeed.Ensure(Constants.DefaultSystemUserTemplate)
                .Commit();

            StaticGroupSeed.Ensure(Constants.Visitors, Constants.DefaultSystemGroupTemplate)
                .Commit();

            PersonSeed.EnsureSystem(Constants.DefaultSystemUserTemplate)
                .Commit();
            PersonSeed.EnsureEveryone(Constants.DefaultSystemUserTemplate)
                .WithGroupLink(Constants.Visitors)
                .Commit();

            FolderFieldTemplateSeed.Ensure("DefaultFolderTemplate")
                .Commit();
            FileFieldTemplateSeed.Ensure("DefaultFileTemplate")
                .WithTemplateType(FileTemplateType.Other)
                .Commit();
        }

        private void EnsureModules(SqlConnection connection)
        {
            var modules = new HashSet<string>();
            var command = new SqlCommand("SELECT ModuleName FROM [dbo].[Foundation_Module]", connection);
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    modules.Add(reader.GetString(0));
                }
            }
            finally
            {
                reader.Close();
            }

            EnsureModule(connection, modules, new Guid("251929E6-67B2-481E-908A-10F742C22DB9"), "Websites", "Litium.Studio", "Litium.Foundation.Modules.CMS.ModuleCMS", "", "/Litium/UI/websites", "~/Litium/CMS/Framework/Settings.config", "~/Site/CMS/Error.aspx", "/Litium/CMS/images/main_publishing_16px.png", "/Litium/CMS/images/main_publishing_24px.png", "/Litium/CMS/images/main_publishing_32px.png");
            EnsureModule(connection, modules, new Guid("463853B6-9F9A-44C2-A03B-50BF919B804E"), "Sales", "Litium.Studio", "Litium.Foundation.Modules.ECommerce.ModuleECommerce", null, "/Litium/ECommerce/Orders.aspx", "~/Litium/ECommerce/Framework/Settings.config", "~/Site/CMS/Error.aspx", "/Litium/ECommerce/images/main_ecommerce_16px.png", "/Litium/ECommerce/images/main_ecommerce_24px.png", "/Litium/ECommerce/images/main_ecommerce_32px.png");
            EnsureModule(connection, modules, new Guid("28EF0C19-8936-4CC2-B57B-55A5BBC3F4E7"), "Customers", "Litium.Studio", "Litium.Foundation.Modules.Relations.ModuleRelations", "", "/Litium/UI/customers", "~/Litium/Relations/Framework/Settings.config", "~/Site/CMS/Error.aspx", "/Litium/Relations/images/main_relations_16px.png", "/Litium/Relations/images/main_relations_24px.png", "/Litium/Relations/images/main_relations_32px.png");
            EnsureModule(connection, modules, new Guid("1883639C-661D-4C1A-B68B-78E36D05A9AE"), "Media", "Litium.Studio", "Litium.Foundation.Modules.MediaArchive.ModuleMediaArchive", "", "/Litium/UI/media", "~/Litium/MediaArchive/Framework/Settings.config", "~/Site/CMS/Error.aspx", "/Litium/MediaArchive/Images/main_mediaarchive_16px.png", "/Litium/MediaArchive/Images/main_mediaarchive_24px.png", "/Litium/MediaArchive/Images/main_mediaarchive_32px.png");
            EnsureModule(connection, modules, new Guid("7B1373FD-BB59-497A-9A91-7D4CFCFC5DB0"), "Dashboard", "Litium.Studio", "Litium.Foundation.Modules.Dashboard.ModuleDashboard", null, "/Litium/Dashboard/Dashboard.aspx", "~/Litium/Dashboard/Framework/Settings.config", "~/Site/CMS/Error.aspx", "/Litium/Dashboard/images/main_dashboard_16px.png", "/Litium/Dashboard/images/main_dashboard_24px.png", "/Litium/Dashboard/images/main_dashboard_32px.png");
            EnsureModule(connection, modules, new Guid("2E5D37A2-4C2D-4EC0-A15A-FBAC4D5E07EF"), "Products", "Litium.Studio", "Litium.Foundation.Modules.ProductCatalog.ModuleProductCatalog", "", "/Litium/app/pim", "~/Litium/ProductCatalog/Framework/Settings.config","~/Site/CMS/Error.aspx", "/Litium/ProductCatalog/images/main_productcatalog_16px.png", "/Litium/ProductCatalog/images/main_productcatalog_24px.png", "/Litium/ProductCatalog/images/main_productcatalog_32px.png");
        }

        private void EnsureModule(SqlConnection connection, ISet<string> modules, Guid id, string moduleName, string assemblyName, string className, string licenseInfo, string startPageUrl, string settingsPageUrl, string errorPageUrl, string iconUrlSmall, string iconUrlMedium, string iconUrlLarge)
        {
            if (!modules.Contains(moduleName))
            {
                var command = new SqlCommand("INSERT INTO [dbo].[Foundation_Module] ([ModuleID],[ModuleName],[AssemblyName],[ClassName],[StartPageUrl],[SettingsPageUrl],[ErrorPageUrl],[IconUrlSmall],[IconUrlMedium],[IconUrlLarge]) VALUES (@id, @moduleName, @assemblyName, @className, @startPageUrl, @settingsPageUrl, @errorPageUrl, @iconUrlSmall, @iconUrlMedium, @iconUrlLarge)", connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@moduleName", moduleName);
                command.Parameters.AddWithValue("@assemblyName", assemblyName);
                command.Parameters.AddWithValue("@className", className);
                command.Parameters.AddWithValue("@startPageUrl", startPageUrl);
                command.Parameters.AddWithValue("@settingsPageUrl", settingsPageUrl);
                command.Parameters.AddWithValue("@errorPageUrl", errorPageUrl);
                command.Parameters.AddWithValue("@iconUrlSmall", iconUrlSmall);
                command.Parameters.AddWithValue("@iconUrlMedium", iconUrlMedium);
                command.Parameters.AddWithValue("@iconUrlLarge", iconUrlLarge);
                command.ExecuteNonQuery();
            }
        }

        private void EnsureOperations(SqlConnection connection)
        {
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
