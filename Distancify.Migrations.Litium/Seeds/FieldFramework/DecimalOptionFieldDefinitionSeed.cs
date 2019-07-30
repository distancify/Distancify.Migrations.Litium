using Distancify.Migrations.Litium.Extensions;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Definitions;
using Litium;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Seeds.FieldFramework
{
    public class DecimalOptionFieldDefinitionSeed : FieldDefinitionSeed, ISeedGenerator<DecimalOptionFieldDefinition>
    {

        private DecimalOptionFieldDefinitionSeed(FieldDefinition fieldDefinition) : base(fieldDefinition)
        {
        }

        public new static DecimalOptionFieldDefinitionSeed Ensure<TArea>(string id, string fieldType)
            where TArea : IArea
        {
            var fieldDefinitionService = IoC.Resolve<FieldDefinitionService>();
            var fieldDefinition = fieldDefinitionService.Get<TArea>(id)?.MakeWritableClone() ??
                new FieldDefinition<TArea>(id, fieldType)
                {
                    SystemId = Guid.Empty
                };

            return new DecimalOptionFieldDefinitionSeed(fieldDefinition);
        }

        public static DecimalOptionFieldDefinitionSeed CreateFrom(DecimalOptionFieldDefinition graphQlItem)
        {
            var areaType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(t => t.Name == graphQlItem.AreaType);

            if (areaType == null)
                throw new Exception($"Cannot find the type for the areaType {graphQlItem.AreaType}");

            var seed = new DecimalOptionFieldDefinitionSeed(new FieldDefinition(graphQlItem.Id, graphQlItem.FieldType, areaType));
            return (DecimalOptionFieldDefinitionSeed)seed.Update(graphQlItem);
        }

        public ISeedGenerator<DecimalOptionFieldDefinition> Update(DecimalOptionFieldDefinition data)
        {
            base.Update(data);

            _fieldDefinition.Option = new DecimalOption()
            {
                MultiSelect = data.Option.MultiSelect,
                Items = data.Option.Items.Select(i => new DecimalOption.Item
                {
                    Name = i.Localizations.ToDictionary(k => k.Culture, v => v.Name),
                    Value = i.Value
                }).ToList()
            };

            return this;
        }

        public DecimalOptionFieldDefinitionSeed WithDecimalOption(DecimalOption option)
        {
            if (!(_fieldDefinition.Option is DecimalOption))
            {
                _fieldDefinition.Option = new DecimalOption();
            }

            var textOption = _fieldDefinition.Option as DecimalOption;
            var fieldDefinitionItems = textOption.Items;

            textOption.MultiSelect = option.MultiSelect;

            foreach (var item in option.Items)
            {
                if (fieldDefinitionItems.FirstOrDefault(i => i.Value == item.Value) is DecimalOption.Item fieldDefinitionItem)
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
            builder.AppendLine($"\r\n\t\t\t{nameof(DecimalOptionFieldDefinitionSeed)}.{nameof(Ensure)}<{_fieldDefinition.AreaType.Name}>(\"{_fieldDefinition.Id}\", \"{_fieldDefinition.FieldType}\")");

            var decimalOption = _fieldDefinition.Option as DecimalOption;
            builder.AppendLine($"\t\t\t\t.{nameof(WithDecimalOption)}(new DecimalOption()\r\n\t\t\t\t{{" +
                               $"\r\n\t\t\t\t\t{nameof(TextOption.MultiSelect)} = {decimalOption.MultiSelect.ToString().ToLower()}," +
                               $"\r\n\t\t\t\t\t{nameof(TextOption.Items)} = new List<DecimalOption.Item>\r\n\t\t\t\t\t{{\r\n\t\t\t\t\t\t{GetDecimalOptions()}" +
                                "\r\n\t\t\t\t\t}\r\n\t\t\t\t})");

            WritePropertiesMigration(builder);
            builder.AppendLine("\t\t\t\t.Commit();");

            string GetDecimalOptions()
                => string.Join(",\r\n\t\t\t\t\t\t", decimalOption.Items.Select(i => "new DecimalOption.Item\r\n\t\t\t\t\t\t{" +
                                                                                 $"\r\n\t\t\t\t\t\t\tValue = {i.Value.ToString(CultureInfo.InvariantCulture)}m," +
                                                                                 $"\r\n\t\t\t\t\t\t\tName = {i.Name.GetMigration(7)}" +
                                                                                 "\r\n\t\t\t\t\t\t}"));
        }
    }
}
