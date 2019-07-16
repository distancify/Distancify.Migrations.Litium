using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Litium;
using Litium.Blocks;
using Litium.Common;
using Litium.FieldFramework;
using Litium.Globalization;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class BlockSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Blocks.Block>
    {
        private readonly Block _block;
        private bool _isPublished = false;
        private bool _isNewBlock = false;
        private string _fieldTemplateId;

        private List<string> _channelLinksIds;

        protected BlockSeed(Block block, string fieldTemplateId, bool isNewBlock = false)
        {
            _block = block;
            _fieldTemplateId = fieldTemplateId;
            _isNewBlock = isNewBlock;
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

            if (_isPublished)
            {
                var draftService = IoC.Resolve<DraftBlockService>();
                var draftBlockClone = draftService.Get(_block.SystemId).MakeWritableClone();

                draftService.Update(draftBlockClone);
                draftService.Publish(draftBlockClone);
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

        public BlockSeed IsGlobal(bool isGlobal)
        {
            _block.Global = isGlobal;
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

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(BlockSeed)}.{nameof(BlockSeed.Ensure)}(Guid.Parse(\"{_block.SystemId.ToString()}\"), \"{_fieldTemplateId}\")");

            foreach (var channelLink in _channelLinksIds)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithChannelLink)}(\"{channelLink}\")");
            }

            if (_block.Status.Equals(ContentStatus.Published))
            {
                builder.AppendLine($"\t\t\t\t.{nameof(IsPublished)}()");
            }
            else
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithStatus)}({(short)_block.Status})");
            }

            builder.AppendLine("\t\t\t\t.Commit();");
        }

        /* TODO
         * AccessControlList
         * Fields
         * Global
         * Localizations
         */
    }
}
