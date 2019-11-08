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
                _productList.Items = baseProductSystemIds.Select(b => new ProductListToBaseProductLink(b)).ToList();
            }
            else
            {
                if (_productList.Items == null)
                {
                    _productList.Items = new List<ProductListToBaseProductLink>();

                    foreach (var baseProduct in baseProductSystemIds)
                    {
                        if (!_productList.Items.Any(i => i.BaseProductSystemId == baseProduct))
                        {
                            _productList.Items.Add(new ProductListToBaseProductLink(baseProduct));
                        }
                    }
                }
            }

            return this;
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<ProductListService>();

            if (_productList.SystemId == Guid.Empty)
            {
                _productList.SystemId = Guid.NewGuid();
                service.Create(_productList);
            }
            else
            {
                service.Update(_productList);
            }

            return _productList.SystemId;
        }
    }
}
