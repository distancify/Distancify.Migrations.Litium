using Litium;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class FieldDefinitionSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.FieldDefinition>
    {
        private readonly FieldDefinition fieldDefinition;

        private FieldDefinitionSeed(FieldDefinition fieldDefinition)
        {
            this.fieldDefinition = fieldDefinition;
        }

        public void Commit()
        {
            var fieldDefinitionService = IoC.Resolve<FieldDefinitionService>();

            if (fieldDefinition.SystemId == Guid.Empty)
            {
                fieldDefinition.SystemId = Guid.NewGuid();
                fieldDefinitionService.Create(fieldDefinition);
            }
            else
            {
                fieldDefinitionService.Update(fieldDefinition);
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
            if (fieldDefinition.SystemId == Guid.Empty)//Cannot change this value for existing fields
            {
                fieldDefinition.MultiCulture = on;
            }

            return this;
        }

        public FieldDefinitionSeed CanBeGridColumn(bool on)
        {
            fieldDefinition.CanBeGridColumn = on;
            return this;
        }


        public FieldDefinitionSeed CanBeGridFilter(bool on)
        {
            fieldDefinition.CanBeGridFilter = on;
            return this;
        }

        public FieldDefinitionSeed WithNames(Dictionary<string, string> localizedNamesByCulture)
        {
            foreach (var item in localizedNamesByCulture)
            {
                if (!fieldDefinition.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                    !fieldDefinition.Localizations[item.Key].Name.Equals(item.Value))
                {
                    fieldDefinition.Localizations[item.Key].Name = item.Value;
                }
            }

            return this;
        }

        public FieldDefinitionSeed WithDescriptions(Dictionary<string, string> localizedDescriptionsByCulture)
        {
            foreach (var item in localizedDescriptionsByCulture)
            {
                if (!fieldDefinition.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                    !item.Value.Equals(fieldDefinition.Localizations[item.Key].Description))
                {
                    fieldDefinition.Localizations[item.Key].Description = item.Value;
                }
            }

            return this;
        }

        public FieldDefinitionSeed WithTextOption(TextOption option)
        {
            if (!(fieldDefinition.Option is TextOption))
            {
                fieldDefinition.Option = new TextOption();
            }

            var textOption = fieldDefinition.Option as TextOption;
            var fieldDefinitionItems = textOption.Items;

            textOption.MultiSelect = option.MultiSelect;

            foreach (var item in option.Items)
            {
                if (fieldDefinitionItems.FirstOrDefault(i => i.Value == item.Value) is TextOption.Item fieldDefinitionItem)
                {
                    foreach (var localization in item.Name.Keys)
                    {
                        if (!fieldDefinitionItem.Name.ContainsKey(localization))
                        {
                            fieldDefinitionItem.Name.Add(localization, item.Name[localization]);
                        }
                        else if (fieldDefinitionItem.Name[localization] != item.Name[localization])
                        {
                            fieldDefinitionItem.Name[localization] = item.Name[localization];
                        }
                    }
                }
                else
                {
                    fieldDefinitionItems.Add(item);
                }
            }

            return this;
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.FieldDefinition> Update(SeedBuilder.LitiumGraphQlModel.FieldDefinition graphQlFieldDefinition)
        {
            this.fieldDefinition.MultiCulture = graphQlFieldDefinition.MultiCulture;
            this.fieldDefinition.CanBeGridColumn = graphQlFieldDefinition.CanBeGridColumn;
            this.fieldDefinition.CanBeGridFilter = graphQlFieldDefinition.CanBeGridFilter;

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\t\t\t{nameof(FieldDefinitionSeed)}.{nameof(Ensure)}<{fieldDefinition.AreaType.Name}>(\"{fieldDefinition.Id}\", \"{fieldDefinition.FieldType}\")");
            builder.AppendLine($"\t\t\t\t.{nameof(IsMultiCulture)}({fieldDefinition.MultiCulture.ToString().ToLower()})");
            builder.AppendLine($"\t\t\t\t.{nameof(CanBeGridColumn)}({fieldDefinition.CanBeGridColumn.ToString().ToLower()})");
            builder.AppendLine($"\t\t\t\t.{nameof(CanBeGridFilter)}({fieldDefinition.CanBeGridFilter.ToString().ToLower()})");
            builder.AppendLine("\t\t\t\t.Commit();");
        }

        public static FieldDefinitionSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.FieldDefinition graphQlItem)
        {
            var areaType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(t => t.Name == graphQlItem.AreaType);

            if(areaType == null)
                throw new Exception($"Cannot find the type for the areaType {graphQlItem.AreaType}");

            var seed = new FieldDefinitionSeed(new FieldDefinition(graphQlItem.Id, graphQlItem.FieldType, areaType));
            return (FieldDefinitionSeed)seed.Update(graphQlItem);
        }

    }
}