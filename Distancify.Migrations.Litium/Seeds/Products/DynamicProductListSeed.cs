using Litium;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class DynamicProductListSeed : ISeed
    {
        private readonly DynamicProductList _productList;

        public DynamicProductListSeed(DynamicProductList productList)
        {
            _productList = productList;
        }

        public static DynamicProductListSeed Ensure(string id)
        {
            var productList = IoC.Resolve<ProductListService>().Get<DynamicProductList>(id)?.MakeWritableClone();

            if (productList == null)
            {
                productList = new DynamicProductList()
                {
                    Id = id,
                    SystemId = Guid.Empty
                };
            }

            return new DynamicProductListSeed(productList);
        }

        public DynamicProductListSeed WithName(string culture, string name)
        {
            if (!_productList.Localizations.Any(l => l.Key.Equals(culture)) ||
                !_productList.Localizations[culture].Name.Equals(name))
            {
                _productList.Localizations[culture].Name = name;
            }

            return this;
        }

        public DynamicProductListSeed WithCondition(DynamicProductListCondition condition)
        {
            if (_productList.Conditions == null)
            {
                _productList.Conditions = new List<DynamicProductListCondition>();
            }

            if (_productList.Conditions.FirstOrDefault(c => c.Id == condition.Id) is DynamicProductListCondition exisitingCondition)
            {
                exisitingCondition.DataType = condition.DataType;
                exisitingCondition.Data = condition.Data;
            }
            else
            {
                _productList.Conditions.Add(condition);
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
