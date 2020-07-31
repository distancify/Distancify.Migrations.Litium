using Litium;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Media;
using Litium.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Seeds.Media
{
    public class FolderSeed : ISeed
    {
        private readonly Folder _folder;

        private FolderSeed(Folder folder)
        {
            _folder = folder;
        }

        public static FolderSeed Ensure(string folderName, string folderFieldTemplateId)
        {
            return Ensure(folderName, folderName, folderFieldTemplateId);
        }

        public static FolderSeed Ensure(string folderName, string folderId, string folderFieldTemplateId)
        {
            var folder = IoC.Resolve<FolderService>().Get(folderName)?.MakeWritableClone();

            if (folder is null)
            {
                var fieldTemplateSystemId = IoC.Resolve<FieldTemplateService>().Get<FolderFieldTemplate>(folderFieldTemplateId).SystemId;

                folder = new Folder(fieldTemplateSystemId, folderName)
                {
                    Id = folderId,
                    SystemId = Guid.Empty
                };
            }

            return new FolderSeed(folder);
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<FolderService>();

            if (_folder.SystemId.Equals(Guid.Empty))
            {
                _folder.SystemId = Guid.NewGuid();
                service.Create(_folder);
            }
            else
            {
                service.Update(_folder);
            }

            return _folder.SystemId;
        }

        public FolderSeed WithVisitorReadPermission()
        {
            var visitorGroupSystemId = IoC.Resolve<GroupService>().Get<StaticGroup>(LitiumMigration.SystemConstants.Visitors).SystemId;

            if (!_folder.AccessControlList.Any(a => a.GroupSystemId == visitorGroupSystemId))
            {
                _folder.AccessControlList.Add(new AccessControlEntry(Operations.Entity.Read, visitorGroupSystemId));
            }

            return this;
        }

        public FolderSeed WithParentFolder(string folderId)
        {
            _folder.ParentFolderSystemId = IoC.Resolve<FolderService>().Get(folderId).SystemId;

            return this;
        }

        public FolderSeed WithField(string fieldName, Dictionary<string, object> values)
        {
            foreach (var localization in values.Keys)
            {
                _folder.Fields.AddOrUpdateValue(fieldName, localization, values[localization]);
            }

            return this;
        }

        public FolderSeed WithField(string fieldName, object value)
        {
            _folder.Fields.AddOrUpdateValue(fieldName, value);

            return this;
        }

        public FolderSeed WithField(string fieldName, string culture, object value)
        {
            _folder.Fields.AddOrUpdateValue(fieldName, culture, value);

            return this;
        }
    }
}
