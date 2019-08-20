using System;

namespace Distancify.Migrations.Litium.Seeds
{
    public interface ISeed
    {
        Guid Commit();

    }
}
