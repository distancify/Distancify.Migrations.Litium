using Litium;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class StaticProductListSeed : ISeed
    {
        private readonly StaticProductList _productList;
        private HashSet<Guid> _listItems = new HashSet<Guid>();

        public StaticProductListSeed(StaticProductList productList)
        {
            _productList = productList;
        }

        public static StaticProductListSeed Ensure(string id)
        {
            var productList = IoC.Resolve<ProductListService>().Get<StaticProductList>(id)?.MakeWritableClone();

            if (productList == null)
            {
                productList = new StaticProductList()
                {
                    Id = id,
                    SystemId = Guid.Empty
                };
            }

            return new StaticProductListSeed(productList);
        }

        public StaticProductListSeed WithName(string culture, string name)
        {
            if (!_productList.Localizations.Any(l => l.Key.Equals(culture)) ||
                !_productList.Localizations[culture].Name.Equals(name))
            {
                _productList.Localizations[culture].Name = name;
            }

            return this;
        }

        public StaticProductListSeed WithItems(List<string> baseProductIds, bool overrideProducts = false)
        {
            var baseProductService = IoC.Resolve<BaseProductService>();
            return WithItems(baseProductIds.Select(b => baseProductService.Get(b).SystemId).ToList(), overrideProducts);
        }

        public StaticProductListSeed WithItems(List<Guid> baseProductSystemIds, bool overrideProducts = false)
        {
            if (overrideProducts)
            {
                _listItems = baseProductSystemIds.Distinct().ToHashSet();
            }
            else
            {
                foreach (var baseProductSystemId in baseProductSystemIds)
                {
                    if (!_listItems.Contains(baseProductSystemId))
                    {
                        _listItems.Add(baseProductSystemId);
                    }
                }
            }

            return this;
        }

        public Guid Commit()
        {
            var productListService = IoC.Resolve<ProductListService>();
            var productListItemService = IoC.Resolve<ProductListItemService>();
            var variantService = IoC.Resolve<VariantService>();

            if (_productList.SystemId == Guid.Empty)
            {
                _productList.SystemId = Guid.NewGuid();
                productListService.Create(_productList);
            }
            else
            {
                productListService.Update(_productList);
            }

            if (_listItems.Count > 0)
            {
                var existingItems = productListItemService.GetByProductList(_productList.SystemId);

                var itemsToRemove = existingItems.Where(i => !_listItems.Contains(i.BaseProductSystemId));
                foreach (var itemToRemove in itemsToRemove)
                {
                    productListItemService.Delete(itemToRemove);
                }

                var existingItemIds = existingItems.Select(i => i.BaseProductSystemId);
                var itemsToCreate = _listItems.Where(i => !existingItemIds.Contains(i));
                foreach(var itemToCreate in itemsToCreate)
                {
                    productListItemService.Create(new ProductListItem(itemToCreate, _productList.SystemId)
                    {
                        ActiveVariantSystemIds = variantService.GetByBaseProduct(itemToCreate).Select(v=>v.SystemId).ToHashSet()
                    });
                }   
            }

            return _productList.SystemId;
        }
    }
}
