using Litium;
using Litium.FieldFramework;
using Litium.Customers;
using System;
using Litium.Security;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Customers
{
    public class StaticGroupSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Customers.StaticGroup>
    {
        private readonly StaticGroup _group;
        private readonly string _fieldTemplateId;

        protected StaticGroupSeed(StaticGroup group, string fieldTemplateId)
        {
            _group = group;
            _fieldTemplateId = fieldTemplateId;
        }

        public static StaticGroupSeed Ensure(string name, string fieldTemplateId)
        {
            var templateSystemId = IoC.Resolve<FieldTemplateService>().Get<GroupFieldTemplate>(fieldTemplateId).SystemId;
            var group = IoC.Resolve<GroupService>().Get<StaticGroup>(name)?.MakeWritableClone();

            if (group is null)
            {
                group = new StaticGroup(templateSystemId, name);
                group.SystemId = Guid.Empty;
                group.Id = name;
                group.Fields[SystemFieldDefinitionConstants.NameInvariantCulture] = name;
            }

            return new StaticGroupSeed(group, fieldTemplateId);
        }

        public static StaticGroupSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Customers.StaticGroup staticGroup)
        {
            var seed = new StaticGroupSeed(new StaticGroup(staticGroup.FieldTemplate.SystemId, staticGroup.Id), staticGroup.FieldTemplate.Id);
            return (StaticGroupSeed)seed.Update(staticGroup);
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<GroupService>();

            if (_group.SystemId == Guid.Empty)
            {
                _group.SystemId = Guid.NewGuid();

                service.Create(_group);
            }
            else
            {
                service.Update(_group);
            }

            return _group.SystemId;
        }

        public StaticGroupSeed WithProductsContentAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Products.Content));
            return this;
        }

        public StaticGroupSeed WithProductsSettingsAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Products.Settings));
            return this;
        }

        public StaticGroupSeed WithProductsUIAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Products.UI));
            return this;
        }
        public StaticGroupSeed WithWebsitesContentAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Websites.Content));
            return this;
        }
        public StaticGroupSeed WithWebsitesSettingsAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Websites.Settings));
            return this;
        }
        public StaticGroupSeed WithWebsitesUIAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Websites.UI));
            return this;
        }

        public StaticGroupSeed WithCustomersUIAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Customers.UI));
            return this;
        }
        public StaticGroupSeed WithCustomersSettingsAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Customers.Settings));
            return this;
        }
        public StaticGroupSeed WithCustomersContentAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Customers.Content));
            return this;
        }
        public StaticGroupSeed WithSalesContentAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Sales.Content));
            return this;
        }
        public StaticGroupSeed WithSalesSettingsAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Sales.Settings));
            return this;
        }
        public StaticGroupSeed WithSalesUIAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Sales.UI));
            return this;
        }
        public StaticGroupSeed WithMediaUIAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Media.UI));
            return this;
        }
        public StaticGroupSeed WithMediaSettingsAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Media.Settings));
            return this;
        }
        public StaticGroupSeed WithMediaContentAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Media.Content));
            return this;
        }
        public StaticGroupSeed WithSystemSettingsAccess()
        {
            _group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.SystemSettings));
            return this;
        }

        public StaticGroupSeed WithAllAccess()
        {
            WithProductsContentAccess();
            WithProductsSettingsAccess();
            WithProductsUIAccess();
            WithWebsitesContentAccess();
            WithWebsitesSettingsAccess();
            WithWebsitesUIAccess();
            WithCustomersUIAccess();
            WithCustomersSettingsAccess();
            WithCustomersContentAccess();
            WithSalesContentAccess();
            WithSalesSettingsAccess();
            WithSalesUIAccess();
            WithMediaUIAccess();
            WithMediaSettingsAccess();
            WithMediaContentAccess();
            WithSystemSettingsAccess();
            return this;
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Customers.StaticGroup> Update(SeedBuilder.LitiumGraphQlModel.Customers.StaticGroup data)
        {
            //TODO: Add access

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(StaticGroupSeed)}.{nameof(StaticGroupSeed.Ensure)}(\"{_group.Id}\", \"{_fieldTemplateId}\"");

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
