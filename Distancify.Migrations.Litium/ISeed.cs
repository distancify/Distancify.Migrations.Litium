﻿namespace Distancify.Migrations.Litium
{
    public interface ISeed
    {
        void Commit();
    }

    public interface ISeed<T> where T : class
    {
        T Commit();
    }
}
