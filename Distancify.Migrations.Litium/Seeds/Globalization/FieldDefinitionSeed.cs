using Litium;
using Litium.FieldFramework;
using Litium.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class FieldDefinitionSeed : ISeed
    {
        private readonly FieldDefinition FieldDefinition;

        private FieldDefinitionSeed(FieldDefinition fieldDefinition)
        {
            FieldDefinition = fieldDefinition;
        }

        public void Commit()
        {
            var fieldDefinitionService = IoC.Resolve<FieldDefinitionService>();

            if (FieldDefinition.SystemId == Guid.Empty)
            {
                FieldDefinition.SystemId = Guid.NewGuid();
                fieldDefinitionService.Create(FieldDefinition);
            }
            else
            {
                fieldDefinitionService.Update(FieldDefinition);
            }
        }

        public static FieldDefinitionSeed Ensure<TArea>(string id, string fieldType)
            where TArea : IArea
        {
            var fieldDefinitionService = IoC.Resolve<FieldDefinitionService>();
            var fieldDefinition = fieldDefinitionService.Get<TArea>(id)?.MakeWritableClone() ??
                new FieldDefinition<TArea>(id, fieldType)
                {
                    SystemId = Guid.Empty
                };

            return new FieldDefinitionSeed(fieldDefinition);
        }

        public FieldDefinitionSeed IsMultiCulture(bool on)
        {
            if (FieldDefinition.SystemId == Guid.Empty)//Cannot change this value for existing fields
            {
                FieldDefinition.MultiCulture = on;
            }

            return this;
        }

        public FieldDefinitionSeed CanBeGridColumn(bool on)
        {
            FieldDefinition.CanBeGridColumn = on;
            return this;
        }


        public FieldDefinitionSeed CanBeGridFilter(bool on)
        {
            FieldDefinition.CanBeGridFilter = on;
            return this;
        }

        public FieldDefinitionSeed WithNames(Dictionary<string, string> localizedNamesByCulture)
        {
            foreach (var item in localizedNamesByCulture)
            {
                if (!FieldDefinition.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                    !FieldDefinition.Localizations[item.Key].Name.Equals(item.Value))
                {
                    FieldDefinition.Localizations[item.Key].Name = item.Value;
                }
            }

            return this;
        }

        public FieldDefinitionSeed WithDescriptions(Dictionary<string, string> localizedDescriptionsByCulture)
        {
            foreach (var item in localizedDescriptionsByCulture)
            {
                if (!FieldDefinition.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                    !item.Value.Equals(FieldDefinition.Localizations[item.Key].Description))
                {
                    FieldDefinition.Localizations[item.Key].Description = item.Value;
                }
            }

            return this;
        }
    }
}