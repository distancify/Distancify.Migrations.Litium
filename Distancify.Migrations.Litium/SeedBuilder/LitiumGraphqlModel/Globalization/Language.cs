using System;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization
{
    public class Language : GraphQlObject
    {
        public Guid SystemId { get; set; }
        public bool? IsDefaultLanguage { get; set; }
    }
}
