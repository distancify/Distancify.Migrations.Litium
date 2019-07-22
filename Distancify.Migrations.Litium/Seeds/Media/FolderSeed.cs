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
            var folder = IoC.Resolve<FolderService>().Get(folderName)?.MakeWritableClone();

            if (folder is null)
            {
                var fieldTemplateSystemId = IoC.Resolve<FieldTemplateService>().Get<FolderFieldTemplate>(folderFieldTemplateId).SystemId;

                folder = new Folder(fieldTemplateSystemId, folderName)
                {
                    Id = folderName,
                    SystemId = Guid.Empty
                };
            }

            return new FolderSeed(folder);
        }

        public void Commit()
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
        }

        public FolderSeed WithVisitorReadPermission()
        {
            var visitorGroupSystemId = IoC.Resolve<GroupService>().Get<StaticGroup>(LitiumConstants.Visitors).SystemId;

            if (!_folder.AccessControlList.Any(a => a.GroupSystemId == visitorGroupSystemId))
            {
                _folder.AccessControlList.Add(new AccessControlEntry(Operations.Entity.Read, visitorGroupSystemId));
            }

            return this;
        }

        public FolderSeed WithParentFolder(string folderName)
        {
            _folder.ParentFolderSystemId = IoC.Resolve<FolderService>().Get(folderName).SystemId;

            return this;
        }
    }
}
