using System;
using System.Linq;
using Litium;
using Litium.Blocks;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class BlockCategorySeed : ISeed
    {
        private readonly Category _blockCategory;

        protected BlockCategorySeed(Category blockCategory)
        {
            _blockCategory = blockCategory;
        }

        public void Commit()
        {
            var service = IoC.Resolve<CategoryService>();

            if (_blockCategory.SystemId == null || _blockCategory.SystemId == Guid.Empty)
            {
                _blockCategory.SystemId = Guid.NewGuid();
                service.Create(_blockCategory);
                return;
            }

            service.Update(_blockCategory);
        }

        public static BlockCategorySeed Ensure(string blockCategoryId)
        {
            var blockCatagoryClone = IoC.Resolve<CategoryService>().Get(blockCategoryId)?.MakeWritableClone();
            if (blockCatagoryClone is null)
            {
                blockCatagoryClone = new Category();
                blockCatagoryClone.Id = blockCategoryId;
                blockCatagoryClone.SystemId = Guid.Empty;
            }

            return new BlockCategorySeed(blockCatagoryClone);
        }

        public BlockCategorySeed WithName(string culture, string name)
        {
            if (!_blockCategory.Localizations.Any(l => l.Key.Equals(culture)) ||
                !_blockCategory.Localizations[culture].Name.Equals(name))
            {
                _blockCategory.Localizations[culture].Name = name;
            }

            return this;
        }

        //Fields
        //Localizations
    }
}
