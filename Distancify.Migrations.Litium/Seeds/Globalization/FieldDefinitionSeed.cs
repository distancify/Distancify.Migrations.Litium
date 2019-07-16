using Distancify.Migrations.Litium.Extensions;
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
        private readonly FieldDefinition _fieldDefinition;

        private FieldDefinitionSeed(FieldDefinition fieldDefinition)
        {
            _fieldDefinition = fieldDefinition;
        }

        public void Commit()
        {
            var fieldDefinitionService = IoC.Resolve<FieldDefinitionService>();

            if (_fieldDefinition.SystemId == Guid.Empty)
            {
                _fieldDefinition.SystemId = Guid.NewGuid();
                fieldDefinitionService.Create(_fieldDefinition);
            }
            else
            {
                fieldDefinitionService.Update(_fieldDefinition);
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
            if (_fieldDefinition.SystemId == Guid.Empty)//Cannot change this value for existing fields
            {
                _fieldDefinition.MultiCulture = on;
            }

            return this;
        }

        public FieldDefinitionSeed CanBeGridColumn(bool on)
        {
            _fieldDefinition.CanBeGridColumn = on;
            return this;
        }


        public FieldDefinitionSeed CanBeGridFilter(bool on)
        {
            _fieldDefinition.CanBeGridFilter = on;
            return this;
        }

        public FieldDefinitionSeed WithNames(Dictionary<string, string> localizedNamesByCulture)
        {
            foreach (var item in localizedNamesByCulture)
            {
                if (!_fieldDefinition.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                    !_fieldDefinition.Localizations[item.Key].Name.Equals(item.Value))
                {
                    _fieldDefinition.Localizations[item.Key].Name = item.Value;
                }
            }

            return this;
        }

        public FieldDefinitionSeed WithDescriptions(Dictionary<string, string> localizedDescriptionsByCulture)
        {
            foreach (var item in localizedDescriptionsByCulture)
            {
                if (!_fieldDefinition.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                    !item.Value.Equals(_fieldDefinition.Localizations[item.Key].Description))
                {
                    _fieldDefinition.Localizations[item.Key].Description = item.Value;
                }
            }

            return this;
        }

        public FieldDefinitionSeed WithTextOption(TextOption option)
        {
            if (!(_fieldDefinition.Option is TextOption))
            {
                _fieldDefinition.Option = new TextOption();
            }

            var textOption = _fieldDefinition.Option as TextOption;
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

        public FieldDefinitionSeed WithPointerOption(PointerOption pointerOption)
        {
            _fieldDefinition.Option = pointerOption;

            return this;
        }

        public static FieldDefinitionSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.FieldDefinition graphQlItem)
        {
            var areaType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(t => t.Name == graphQlItem.AreaType);

            if (areaType == null)
                throw new Exception($"Cannot find the type for the areaType {graphQlItem.AreaType}");

            var seed = new FieldDefinitionSeed(new FieldDefinition(graphQlItem.Id, graphQlItem.FieldType, areaType));
            return (FieldDefinitionSeed)seed.Update(graphQlItem);
        }
        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.FieldDefinition> Update(SeedBuilder.LitiumGraphQlModel.FieldDefinition data)
        {
            _fieldDefinition.MultiCulture = data.MultiCulture;
            _fieldDefinition.CanBeGridColumn = data.CanBeGridColumn;
            _fieldDefinition.CanBeGridFilter = data.CanBeGridFilter;
            _fieldDefinition.FieldType = data.FieldType;

            if (data.FieldType.Equals(SystemFieldTypeConstants.TextOption))
            {
                _fieldDefinition.Option = new TextOption()
                {
                    MultiSelect = data.Option.MultiSelect,
                    Items = data.Option.Items.Select(i => new TextOption.Item
                    {
                        Name = i.Localizations.ToDictionary(k => k.Culture, v => v.Name),
                        Value = i.Value
                    }).ToList()
                };
            }
            else if (data.FieldType.Equals(SystemFieldTypeConstants.Pointer))
            {
                _fieldDefinition.Option = new PointerOption()
                {
                    MultiSelect = data.Option.MultiSelect,
                    EntityType = data.Option.EntityType
                };
            }

            foreach (var localization in data.Localizations)
            {
                if (!string.IsNullOrEmpty(localization.Culture) && !string.IsNullOrEmpty(localization.Name))
                {
                    _fieldDefinition.Localizations[localization.Culture].Name = localization.Name;
                }
                else
                {
                    this.Log().Warn("The field definition with system id {FieldDefinitionSystemId} contains a localization with an empty culture and/or name!", data.SystemId.ToString());
                }
            }

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(FieldDefinitionSeed)}.{nameof(Ensure)}<{_fieldDefinition.AreaType.Name}>(\"{_fieldDefinition.Id}\", \"{_fieldDefinition.FieldType}\")");

            if (_fieldDefinition.Localizations.Any())
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithNames)}({_fieldDefinition.Localizations.ToDictionary(k => k.Key, v => v.Value.Name).GetMigration(4)})");
            }

            if (_fieldDefinition.FieldType.Equals(SystemFieldTypeConstants.TextOption))
            {
                var textOption = _fieldDefinition.Option as TextOption;
                builder.AppendLine($"\t\t\t\t.{nameof(WithTextOption)}(new TextOption()\r\n\t\t\t\t{{\r\n\t\t\t\t\t{nameof(TextOption.MultiSelect)} = {textOption.MultiSelect.ToString().ToLower()}," +
                                   $"\r\n\t\t\t\t\t{nameof(TextOption.Items)} = new List<TextOption.Item>\r\n\t\t\t\t\t{{\r\n\t\t\t\t\t\t{GetTextOptions(textOption)}" +
                                    "\r\n\t\t\t\t\t}\r\n\t\t\t\t})");
            }
            else if (_fieldDefinition.FieldType.Equals(SystemFieldTypeConstants.Pointer))
            {
                var pointerOption = _fieldDefinition.Option as PointerOption;
                builder.AppendLine($"\t\t\t\t.{nameof(WithPointerOption)}(new PointerOption\r\n\t\t\t\t{{" + 
                                   $"\r\n\t\t\t\t\tEntityType = \"{pointerOption.EntityType}\"," + 
                                   $"\r\n\t\t\t\t\tMultiSelect = {pointerOption.MultiSelect.ToString().ToLower()}" + 
                                    "\r\n\t\t\t\t})");
            }

            builder.AppendLine($"\t\t\t\t.{nameof(IsMultiCulture)}({_fieldDefinition.MultiCulture.ToString().ToLower()})");
            builder.AppendLine($"\t\t\t\t.{nameof(CanBeGridColumn)}({_fieldDefinition.CanBeGridColumn.ToString().ToLower()})");
            builder.AppendLine($"\t\t\t\t.{nameof(CanBeGridFilter)}({_fieldDefinition.CanBeGridFilter.ToString().ToLower()})");
            builder.AppendLine("\t\t\t\t.Commit();");

            string GetTextOptions(TextOption textOption)
                => string.Join(",\r\n\t\t\t\t\t\t", textOption.Items.Select(i => "new TextOption.Item\r\n\t\t\t\t\t\t{" +
                                                                                 $"\r\n\t\t\t\t\t\t\tValue = \"{i.Value}\"," +
                                                                                 $"\r\n\t\t\t\t\t\t\tName = {i.Name.GetMigration(7)}" +
                                                                                 "\r\n\t\t\t\t\t\t}"));

        }
    }
}