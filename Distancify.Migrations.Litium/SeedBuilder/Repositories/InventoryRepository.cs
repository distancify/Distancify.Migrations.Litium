using Distancify.Migrations.Litium.Seeds.Product;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class InventoryRepository : Repository<LitiumGraphQlModel.Products.Inventory, InventorySeed>
    {
        protected override InventorySeed CreateFrom(LitiumGraphQlModel.Products.Inventory graphQlItem)
        {
            return InventorySeed.CreateFrom(graphQlItem);
        }
    }
}
