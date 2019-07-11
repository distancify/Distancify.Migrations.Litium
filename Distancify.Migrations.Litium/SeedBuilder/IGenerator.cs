namespace Distancify.Migrations.Litium.SeedBuilder
{
    public interface IGenerator
    {
        string GenerateMigration();
        void PopulateSeedsWithData(LitiumGraphQlModel.Data data);
    }
}
