using System;
using Litium;
using Litium.Blocks;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class BlockCategorySeed : ISeed
    {
        private readonly Category blockCategory;

        protected BlockCategorySeed(Category blockCategory)
        {
            this.blockCategory = blockCategory;
        }

        public void Commit()
        {
            var service = IoC.Resolve<CategoryService>();

            if (blockCategory.SystemId == null || blockCategory.SystemId == Guid.Empty)
            {
                blockCategory.SystemId = Guid.NewGuid();
                service.Create(blockCategory);
                return;
            }

            service.Update(blockCategory);
        }

        public static BlockCategorySeed Ensure(string blockCategoryId)
        {
            var blockCatagoryClone = IoC.Resolve<CategoryService>().Get(blockCategoryId)?.MakeWritableClone();
            if (blockCatagoryClone is null)
            {
                blockCatagoryClone = new Category();
                blockCatagoryClone.Id = blockCategoryId;
                blockCatagoryClone.SystemId = Guid.Empty;
                blockCatagoryClone.Localizations["en-US"].Name = blockCategoryId;
            }

            return new BlockCategorySeed(blockCatagoryClone);
        }

        //Fields
        //Localizations
    }
}
