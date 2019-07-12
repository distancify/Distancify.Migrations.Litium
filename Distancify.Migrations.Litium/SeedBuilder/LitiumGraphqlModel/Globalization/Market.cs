using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization
{
    public class Market : GraphQlObject
    {
        private string _id;
        public override string Id
        {
            get => _id ?? (_id = SystemId.ToString());
            set => _id = value;
        }
        public Guid SystemId { get; set; }
        public string FieldTemplateId { get; set; }
        public Guid FieldTemplateSystemId { get; set; }
        public string AssortmentId { get; set; }
        public Guid AssortmentSystemId { get; set; }

        public List<FieldLocalization> Localizations { get; set; }
    }
}
