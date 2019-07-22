using Litium;
using Litium.Blobs;
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
    public class FileSeed : ISeed
    {
        private readonly File _file;
        private readonly string _filePath;
        private readonly BlobContainer _blobContainer;
        private readonly FileFieldTemplate _fieldTemplate;

        private FileSeed(File file, string filePath, FileFieldTemplate fieldTemplate, BlobContainer blobContainer)
        {
            _file = file;
            _filePath = filePath;
            _blobContainer = blobContainer;
            _fieldTemplate = fieldTemplate;
        }

        public static FileSeed Ensure(string fileId, string filePath, string fileFieldTemplateId, Guid folderSystemId)
        {
            var fileName = System.IO.Path.GetFileName(filePath);
            var file = IoC.Resolve<FileService>().Get(fileName)?.MakeWritableClone();
            var fileFieldTemplate = IoC.Resolve<FieldTemplateService>().Get<FileFieldTemplate>(fileFieldTemplateId);

            if (file is null)
            {
                var blobContainer = IoC.Resolve<BlobService>().Create(File.BlobAuthority);

                file = new File(fileFieldTemplate.SystemId, folderSystemId, blobContainer.Uri, fileName)
                {
                    SystemId = Guid.Empty,
                    Id = fileId
                };

                return new FileSeed(file, filePath, fileFieldTemplate, blobContainer);
            }

            return new FileSeed(file, filePath, fileFieldTemplate, null);
        }


        public static FileSeed Ensure(string fileId, string filePath, string fileFieldTemplateId, string folderName)
        {
            var folderSystemId = IoC.Resolve<FolderService>().Get(folderName).SystemId;

            return Ensure(fileId, filePath, fileFieldTemplateId, folderSystemId);
        }

        /// <summary>
        /// The file will be put in the first root folder.
        /// </summary>
        public static FileSeed Ensure(string fileId, string filePath, string fileFieldTemplateId)
        {
            var folderSystemId = IoC.Resolve<FolderService>().GetChildFolders(Guid.Empty).First().SystemId;

            return Ensure(fileId, filePath, fileFieldTemplateId, folderSystemId);
        }

        /// <summary>
        /// The id of the file will be the name of the file, taken from the path.
        /// The file will be put in the first root folder.
        /// </summary>
        public static FileSeed Ensure(string filePath, string fileFieldTemplateId)
        {
            var fileName = System.IO.Path.GetFileName(filePath);
            var folderSystemId = IoC.Resolve<FolderService>().GetChildFolders(Guid.Empty).First().SystemId;

            return Ensure(fileName, filePath, fileFieldTemplateId, folderSystemId);
        }

        public void Commit()
        {
            var service = IoC.Resolve<FileService>();

            if (_file.SystemId.Equals(Guid.Empty))
            {
                _file.FileSize = GetFileSize();
                _file.SystemId = Guid.NewGuid();
                IoC.Resolve<FileMetadataExtractorService>().UpdateMetadata(_fieldTemplate, _file, null, _file.BlobUri);

                service.Create(_file);
            }
            else
            {
                service.Update(_file);
            }


            long GetFileSize()
            {
                var fileSize = (long)0;

                using (var fileStream = System.IO.File.OpenRead(_filePath))
                {
                    using (var stream = _blobContainer.GetDefault().OpenWrite())
                    {
                        fileStream.CopyTo(stream);
                        fileSize = fileStream.Length;
                    }
                }

                return fileSize;
            }
        }

        public FileSeed WithVisitorReadPermission()
        {
            var visitorGroupSystemId = IoC.Resolve<GroupService>().Get<StaticGroup>(LitiumConstants.Visitors).SystemId;

            if (!_file.AccessControlList.Any(a => a.GroupSystemId == visitorGroupSystemId))
            {
                _file.AccessControlList.Add(new AccessControlEntry(Operations.Entity.Read, visitorGroupSystemId));
            }

            return this;
        }
    }
}
