using System;
using Litium.Runtime.DependencyInjection;

namespace Distancify.Migrations.Litium
{
    internal class IoC
    {
        internal static T Resolve<T>() where T:class
        {
            return ServiceLocator.ServiceProvider.GetService(typeof(T)) as T;
        }
    }
}