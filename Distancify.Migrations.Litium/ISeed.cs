namespace Distancify.Migrations.Litium
{
    public interface ISeed
    {
        void Commit();
        string GenerateMigration();
    }

    public interface ISeed<T> where T : class
    {
        T Commit();
        string Generate();
    }
}
