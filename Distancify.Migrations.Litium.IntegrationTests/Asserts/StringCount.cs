using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Distancify.Migrations.Litium.IntegrationTests.Asserts
{
    public static class AssertExtentions
    {
        public static void StringCount(int expected, string stringToCount, string inputText)
        {

            int count = 0;
            int i = 0;
            while ((i = inputText.IndexOf(stringToCount, i)) != -1)
            {
                i += stringToCount.Length;
                count++;
            }

            if (count == expected)
                return;

            throw new EqualException(expected, count);
        }
    }
}
