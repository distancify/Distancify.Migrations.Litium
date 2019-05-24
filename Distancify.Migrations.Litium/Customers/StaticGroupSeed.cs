
using Distancify.Migrations.Litium.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Litium.Security;

namespace Distancify.Migrations.Litium.Customers
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
    }
}
