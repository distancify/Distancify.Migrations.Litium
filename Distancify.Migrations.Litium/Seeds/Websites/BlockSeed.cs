using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distancify.Migrations.Litium.Extensions;
using Litium;
using Litium.Blocks;
using Litium.Common;
using Litium.FieldFramework;
using Litium.Globalization;
using FieldData = Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldData;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class BlockSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Blocks.Block>
    {
        private readonly Block _block;
        private bool _isPublished = false;
        private bool _isNewBlock = false;
        private string _fieldTemplateId;

        private List<string> _channelLinksIds;
        private List<FieldData> _fields;

        protected BlockSeed(Block block, string fieldTemplateId, bool isNewBlock = false)
        {
            _block = block;
            _fieldTemplateId = fieldTemplateId;
            _isNewBlock = isNewBlock;
            _fields = new List<FieldData>();
        }

        public void Commit()
        {
            var service = IoC.Resolve<BlockService>();

            if (_isNewBlock)
            {
                service.Create(_block);
            }
            else
            {
                service.Update(_block);
            }

            var draftService = IoC.Resolve<DraftBlockService>();
            var draftBlockClone = draftService.Get(_block.SystemId).MakeWritableClone();

            UpdateDraftBlockWithFields();

            draftService.Update(draftBlockClone);

            if (_isPublished)
            {
                draftService.Publish(draftBlockClone);
            }

            void UpdateDraftBlockWithFields()
            {
                foreach (var field in _fields)
                {
                    if (string.IsNullOrEmpty(field.Culture))
                    {
                        draftBlockClone.Fields.AddOrUpdateValue(field.FieldId, field.Value);
                    }
                    else
                    {
                        draftBlockClone.Fields.AddOrUpdateValue(field.FieldId, field.Culture, field.Value);
                    }
                }
            }
        }

        public static BlockSeed Ensure(Guid blockSystemId, string blockTemplateId)
        {
            var blockFieldTemplateSystemGuid = IoC.Resolve<FieldTemplateService>().Get<BlockFieldTemplate>(blockTemplateId).SystemId;
            var blockClone = IoC.Resolve<BlockService>().Get(blockSystemId)?.MakeWritableClone();
            var isNewBlock = false;

            if (blockClone is null)
            {
                blockClone = new Block(blockFieldTemplateSystemGuid)
                {
                    SystemId = blockSystemId
                };
                isNewBlock = true;
            }

            return new BlockSeed(blockClone, blockTemplateId, isNewBlock);
        }

        public BlockSeed IsGlobal()
        {
            _block.Global = true;
            return this;
        }

        public BlockSeed IsPublished()
        {
            _isPublished = true;

            return this;
        }

        public BlockSeed WithStatus(ContentStatus status)
        {
            _block.Status = status;

            return this;
        }

        public BlockSeed WithStatus(short status)
        {
            _block.Status = (ContentStatus)status;

            return this;
        }

        public BlockSeed WithChannelLink(Guid channelSystemId)
        {
            if (_block.ChannelLinks == null)
            {
                _block.ChannelLinks = new List<BlockToChannelLink>();
            }

            if (!_block.ChannelLinks.Any(c => c.ChannelSystemId == channelSystemId))
            {
                _block.ChannelLinks.Add(new BlockToChannelLink(channelSystemId));
            }

            return this;
        }


        public BlockSeed WithChannelLink(string channelId)
        {
            var channelSystemId = IoC.Resolve<ChannelService>().Get(channelId).SystemId;

            return this.WithChannelLink(channelSystemId);
        }

        public BlockSeed WithField(string fieldName, object value)
        {
            _block.Fields.AddOrUpdateValue(fieldName, value);
            _fields.Add(new FieldData(fieldName, value));

            return this;
        }

        public BlockSeed WithField(string fieldName, object value, string culture)
        {
            _block.Fields.AddOrUpdateValue(fieldName, culture, value);
            _fields.Add(new FieldData(fieldName, value, culture));

            return this;
        }

        public static BlockSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Blocks.Block block)
        {
            var seed = new BlockSeed(new Block(block.SystemId), block.FieldTemplate.Id);
            return (BlockSeed)seed.Update(block);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Blocks.Block> Update(SeedBuilder.LitiumGraphQlModel.Blocks.Block data)
        {
            _block.SystemId = data.SystemId;
            _block.Status = (ContentStatus)data.Status;
            _block.Global = data.Global;

            _channelLinksIds = data.ChannelLinks?.Select(c => c.Channel.Id).ToList() ?? new List<string>();

            _fieldTemplateId = data.FieldTemplate.Id;
            _fields = data.Fields.GetFieldData();

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(BlockSeed)}.{nameof(BlockSeed.Ensure)}(Guid.Parse(\"{_block.SystemId.ToString()}\"), \"{_fieldTemplateId}\")");

            foreach (var channelLink in _channelLinksIds)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithChannelLink)}(\"{channelLink}\")");
            }

            foreach (var field in _fields)
            {
                field.WriteMigration(builder);
            }

            if (_block.Status.Equals(ContentStatus.Published))
            {
                builder.AppendLine($"\t\t\t\t.{nameof(IsPublished)}()");
            }
            else
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithStatus)}({(short)_block.Status})");
            }

            if (_block.Global)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(IsGlobal)}()");
            }

            builder.AppendLine("\t\t\t\t.Commit();");
        }

        /* TODO
         * AccessControlList
         * Fields
         * Localizations
         */
    }
}
