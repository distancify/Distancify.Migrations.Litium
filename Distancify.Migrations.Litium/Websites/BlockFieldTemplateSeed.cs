using Distancify.Migrations.Litium.BaseSeeds;
using Litium;
using Litium.Blocks;
using Litium.FieldFramework;
using Litium.Websites;
using System;

namespace Distancify.Migrations.Litium.Websites
{
    public class BlockFieldTemplateSeed : FieldTemplateSeed<BlockFieldTemplate>
    {
        public BlockFieldTemplateSeed(BlockFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static BlockFieldTemplateSeed Ensure(string blockFieldTemplateId)
        {
            var blockFieldTemplate = (BlockFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<BlockFieldTemplate>(blockFieldTemplateId)?.MakeWritableClone();
            if (blockFieldTemplate is null)
            {
                blockFieldTemplate = new BlockFieldTemplate(blockFieldTemplateId);
                blockFieldTemplate.SystemId = Guid.Empty;
            }

            return new BlockFieldTemplateSeed(blockFieldTemplate);
        }

        public override string GenerateMigration()
        {
            throw new NotImplementedException();
        }

        public BlockFieldTemplateSeed WithCategory(string blockCategoryId)
        {
            var blockCatagorySystemGuid = IoC.Resolve<CategoryService>().Get(blockCategoryId).SystemId;
            base.fieldTemplate.CategorySystemId = blockCatagorySystemGuid;

            return this;
        }

        /*TODO
         * TemplatePath
         * Icon
         */
    }
}
