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
using FieldData = Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldData;

namespace Distancify.Migrations.Litium.Seeds.Media
{
    public class FileSeed : ISeed
    {
        private readonly File _file;
        private readonly string _filePath;
        private readonly BlobContainer _blobContainer;
        private readonly FileFieldTemplate _fieldTemplate;
        private Guid _newSystemId;
        private readonly List<FieldData> _fields;

        private FileSeed(File file, string filePath, FileFieldTemplate fieldTemplate, BlobContainer blobContainer)
        {
            _file = file;
            _filePath = filePath;
            _blobContainer = blobContainer;
            _fieldTemplate = fieldTemplate;
            _newSystemId = Guid.NewGuid();
            _fields = new List<FieldData>();
        }

        public static FileSeed Ensure(string fileId, string filePath, string fileFieldTemplateId, Guid folderSystemId)
        {
            var file = IoC.Resolve<FileService>().Get(fileId)?.MakeWritableClone();
            var fileFieldTemplate = IoC.Resolve<FieldTemplateService>().Get<FileFieldTemplate>(fileFieldTemplateId);

            if (file is null)
            {
                var fileName = System.IO.Path.GetFileName(filePath);
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

        /// <summary>
        /// Sets the future system id of new entities.
        /// </summary>
        public FileSeed WithSystemId(Guid systemId)
        {
            _newSystemId = systemId;
            return this;
        }

        public FileSeed WithField(string fieldName, Dictionary<string, object> values)
        {
            foreach (var localization in values.Keys)
            {
                _file.Fields.AddOrUpdateValue(fieldName, localization, values[localization]);
                _fields.Add(new FieldData(fieldName, values[localization], localization));
            }

            return this;
        }

        public FileSeed WithField(string fieldName, object value)
        {
            _file.Fields.AddOrUpdateValue(fieldName, value);
            _fields.Add(new FieldData(fieldName, value));

            return this;
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<FileService>();

            if (_file.SystemId.Equals(Guid.Empty))
            {
                _file.FileSize = GetFileSize();
                _file.SystemId = _newSystemId;
                IoC.Resolve<FileMetadataExtractorService>().UpdateMetadata(_fieldTemplate, _file, null, _file.BlobUri);

                UpdateFields();

                service.Create(_file);
            }

            UpdateFields();
            service.Update(_file);

            return _file.SystemId;

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

            void UpdateFields()
            {
                foreach (var field in _fields)
                {
                    if (string.IsNullOrEmpty(field.Culture))
                    {
                        _file.Fields.AddOrUpdateValue(field.FieldId, field.Value);
                    }
                    else
                    {
                        _file.Fields.AddOrUpdateValue(field.FieldId, field.Culture, field.Value);
                    }
                }
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
