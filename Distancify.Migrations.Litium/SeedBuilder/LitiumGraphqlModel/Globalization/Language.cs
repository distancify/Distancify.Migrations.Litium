using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel.Globalization
{
    public class Language : GraphQlObject
    {
        public Guid SystemId { get; set; }
        public bool? IsDefaultLanguage { get; set; }
    }
}
