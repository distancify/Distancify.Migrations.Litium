using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.Blocks;
using Litium.FieldFramework;
using System;

namespace Distancify.Migrations.Litium.Seeds.Website
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

        public BlockFieldTemplateSeed WithCategory(string blockCategoryId)
        {
            var blockCatagorySystemGuid = IoC.Resolve<CategoryService>().Get(blockCategoryId).SystemId;
            fieldTemplate.CategorySystemId = blockCatagorySystemGuid;

            return this;
        }

        /*TODO
         * TemplatePath
         * Icon
         */
    }
}
