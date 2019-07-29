using Distancify.Migrations.Litium.Extensions;
using Distancify.Migrations.Litium.Seeds.Globalization;
using Litium;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Seeds.FieldFramework
{
    public class TextOptionFieldDefinitionSeed : FieldDefinitionSeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.TextOptionFieldDefinition>
    {

        private TextOptionFieldDefinitionSeed(FieldDefinition fieldDefinition):base(fieldDefinition)
        {
        }

        public new static TextOptionFieldDefinitionSeed Ensure<TArea>(string id, string fieldType)
            where TArea : IArea
        {
            var fieldDefinitionService = IoC.Resolve<FieldDefinitionService>();
            var fieldDefinition = fieldDefinitionService.Get<TArea>(id)?.MakeWritableClone() ??
                new FieldDefinition<TArea>(id, fieldType)
                {
                    SystemId = Guid.Empty
                };

            return new TextOptionFieldDefinitionSeed(fieldDefinition);
        }

        public static TextOptionFieldDefinitionSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.TextOptionFieldDefinition graphQlItem)
        {
            var areaType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(t => t.Name == graphQlItem.AreaType);

            if (areaType == null)
                throw new Exception($"Cannot find the type for the areaType {graphQlItem.AreaType}");

            var seed = new TextOptionFieldDefinitionSeed(new FieldDefinition(graphQlItem.Id, graphQlItem.FieldType, areaType));
            return (TextOptionFieldDefinitionSeed)seed.Update(graphQlItem);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.TextOptionFieldDefinition> Update(SeedBuilder.LitiumGraphQlModel.TextOptionFieldDefinition data)
        {
            base.Update(data);

            _fieldDefinition.Option = new TextOption()
            {
                MultiSelect = data.Option.MultiSelect,
                Items = data.Option.Items.Select(i => new TextOption.Item
                {
                    Name = i.Localizations.ToDictionary(k => k.Culture, v => v.Name),
                    Value = i.Value
                }).ToList()
            };

            return this;
        }

        public TextOptionFieldDefinitionSeed WithTextOption(TextOption option)
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

        public new void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(TextOptionFieldDefinitionSeed)}.{nameof(Ensure)}<{_fieldDefinition.AreaType.Name}>(\"{_fieldDefinition.Id}\", \"{_fieldDefinition.FieldType}\")");

            var textOption = _fieldDefinition.Option as TextOption;
            builder.AppendLine($"\t\t\t\t.{nameof(WithTextOption)}(new TextOption()\r\n\t\t\t\t{{\r\n\t\t\t\t\t{nameof(TextOption.MultiSelect)} = {textOption.MultiSelect.ToString().ToLower()}," +
                               $"\r\n\t\t\t\t\t{nameof(TextOption.Items)} = new List<TextOption.Item>\r\n\t\t\t\t\t{{\r\n\t\t\t\t\t\t{GetTextOptions()}" +
                                "\r\n\t\t\t\t\t}\r\n\t\t\t\t})");

            WritePropertiesMigration(builder);
            builder.AppendLine("\t\t\t\t.Commit();");

            string GetTextOptions()
                => string.Join(",\r\n\t\t\t\t\t\t", textOption.Items.Select(i => "new TextOption.Item\r\n\t\t\t\t\t\t{" +
                                                                                 $"\r\n\t\t\t\t\t\t\tValue = \"{i.Value}\"," +
                                                                                 $"\r\n\t\t\t\t\t\t\tName = {i.Name.GetMigration(7)}" +
                                                                                 "\r\n\t\t\t\t\t\t}"));
        }
    }
}
