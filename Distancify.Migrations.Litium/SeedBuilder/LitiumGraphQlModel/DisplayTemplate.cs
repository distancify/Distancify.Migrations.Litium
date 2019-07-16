using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class DisplayTemplate : GraphQlObject
    {
        public string TemplatePath { get; set; }
        public Guid SystemId { get; set; }

        public List<FieldLocalization> Localizations { get; set; }
        public List<DisplayTemplateToWebsiteLink> Templates { get; set; }
    }
}
