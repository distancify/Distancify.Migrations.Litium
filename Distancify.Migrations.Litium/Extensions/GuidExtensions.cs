using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Extensions
{
    public static class GuidUtils
    {
        /// <summary>
        /// Returns the first given Guid that is not null or empty. Otherwise returns Guid.Empty.
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public static Guid NonNullOrEmpty(params Guid?[] guids)
        {
            foreach (var g in guids)
            {
                if (g != null && g != Guid.Empty)
                    return (Guid)g;
            }
            return Guid.Empty;
        }
    }
}
