using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel
{
    public class Language : GraphQlObject
    {
        public Guid SystemId { get; set; }

        public bool? IsDefaultLanguage { get; set; }
    }
}
