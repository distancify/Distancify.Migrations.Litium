namespace Distancify.Migrations.Litium.SeedBuilder
{
    public interface IGenerator
    {
        string GenerateMigration();
        void PopulateSeedsWithData(LitiumGraphqlModel.Data data);
    }
}
