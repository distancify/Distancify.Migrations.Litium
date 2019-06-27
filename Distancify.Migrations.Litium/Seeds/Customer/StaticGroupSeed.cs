using Litium;
using Litium.FieldFramework;
using Litium.Customers;
using System;
using Litium.Security;

namespace Distancify.Migrations.Litium.Seeds.Customer
{
    public class StaticGroupSeed : ISeed
    {
        private readonly StaticGroup group;

        protected StaticGroupSeed(StaticGroup group)
        {
            this.group = group;
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

            return new StaticGroupSeed(group);
        }

        public void Commit()
        {
            var service = IoC.Resolve<GroupService>();

            if (group.SystemId == Guid.Empty)
            {
                group.SystemId = Guid.NewGuid();

                service.Create(group);
            }
            else
            {
                service.Update(group);
            }
        }

        public StaticGroupSeed WithProductsContentAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Products.Content));
            return this;
        }

        public StaticGroupSeed WithProductsSettingsAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Products.Settings));
            return this;
        }

        public StaticGroupSeed WithProductsUIAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Products.UI));
            return this;
        }
        public StaticGroupSeed WithWebsitesContentAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Websites.Content));
            return this;
        }
        public StaticGroupSeed WithWebsitesSettingsAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Websites.Settings));
            return this;
        }
        public StaticGroupSeed WithWebsitesUIAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Websites.UI));
            return this;
        }

        public StaticGroupSeed WithCustomersUIAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Customers.UI));
            return this;
        }
        public StaticGroupSeed WithCustomersSettingsAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Customers.Settings));
            return this;
        }
        public StaticGroupSeed WithCustomersContentAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Customers.Content));
            return this;
        }
        public StaticGroupSeed WithSalesContentAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Sales.Content));
            return this;
        }
        public StaticGroupSeed WithSalesSettingsAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Sales.Settings));
            return this;
        }
        public StaticGroupSeed WithSalesUIAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Sales.UI));
            return this;
        }
        public StaticGroupSeed WithMediaUIAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Media.UI));
            return this;
        }
        public StaticGroupSeed WithMediaSettingsAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Media.Settings));
            return this;
        }
        public StaticGroupSeed WithMediaContentAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.Media.Content));
            return this;
        }
        public StaticGroupSeed WithSystemSettingsAccess()
        {
            group.AccessControlOperationList.Add(new AccessControlOperationEntry(Operations.Function.SystemSettings));
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

    }
}
