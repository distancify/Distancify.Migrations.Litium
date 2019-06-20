using System;
using Litium;
using Litium.Blocks;
using Litium.FieldFramework;

namespace Distancify.Migrations.Litium.Websites
{
    public class BlockSeed : ISeed<BlockSeed>
    {
        private readonly Block block;

        protected BlockSeed(Block block)
        {
            this.block = block;
        }

        public BlockSeed Commit()
        {
            var service = IoC.Resolve<BlockService>();

            if (block.SystemId == null || block.SystemId == Guid.Empty)
            {
                block.SystemId = Guid.NewGuid();
                service.Create(block);
                return this;
            }

            service.Update(block);
            return this;
        }


        public static BlockSeed Ensure(string blockId, string blockTemplateId)
        {
            var blockFieldTemplateSystemGuid = IoC.Resolve<FieldTemplateService>().Get<BlockFieldTemplate>(blockTemplateId).SystemId;

            var blockClone = IoC.Resolve<BlockService>().Get(blockId)?.MakeWritableClone();
            if (blockClone is null)
            {
                blockClone = new Block(blockFieldTemplateSystemGuid);
                blockClone.Id = blockId;
                blockClone.SystemId = Guid.Empty;
                blockClone.Localizations["en-US"].Name = blockId;
            }

            return new BlockSeed(blockClone);
        }

        public BlockSeed IsGlobal(bool isGlobal)
        {
            block.Global = isGlobal;
            return this;
        }

        public void Publish()
        {

            var service = IoC.Resolve<DraftBlockService>();
            var draftBlockClone = service.Get(block.SystemId).MakeWritableClone();
            service.Update(draftBlockClone);
            service.Publish(draftBlockClone);
        }

        public string Generate()
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
