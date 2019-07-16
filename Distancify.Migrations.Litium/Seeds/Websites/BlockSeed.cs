using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Litium;
using Litium.Blocks;
using Litium.Common;
using Litium.FieldFramework;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class BlockSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Blocks.Block>
    {
        private readonly Block _block;
        private bool _isPublished = false;
        private string _fieldTemplateId;

        protected BlockSeed(Block block, string fieldTemplateId)
        {
            _block = block;
            _fieldTemplateId = fieldTemplateId;
        }

        public void Commit()
        {
            var service = IoC.Resolve<BlockService>();

            if (_block.SystemId == Guid.Empty)
            {
                _block.SystemId = Guid.NewGuid();
                service.Create(_block);
            }

            service.Update(_block);

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

            if (blockClone is null)
            {
                blockClone = new Block(blockFieldTemplateSystemGuid)
                {
                    SystemId = Guid.Empty
                };
            }

            return new BlockSeed(blockClone, blockTemplateId);
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

            _block.ChannelLinks = data.ChannelLinks?.Select(c => new BlockToChannelLink(c.ChannelSystemId)).ToList()
                ?? new List<BlockToChannelLink>();

            _fieldTemplateId = data.FieldTemplate.Id;

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            throw new NotImplementedException();
        }

        /* TODO
         * AccessControlList
         * ChannelLinks
         * Fields
         * FieldTemplateSystemId
         * Global
         * Localizations
         * Status
         */
    }
}
