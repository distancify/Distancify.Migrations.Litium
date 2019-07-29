using Distancify.Migrations.Litium.Extensions;
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
    public class IntOptionFieldDefinitionSeed : FieldDefinitionSeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.IntOptionFieldDefinition>
    {
        private IntOptionFieldDefinitionSeed(FieldDefinition fieldDefinition) : base(fieldDefinition)
        {
        }

        public new static IntOptionFieldDefinitionSeed Ensure<TArea>(string id, string fieldType)
            where TArea : IArea
        {
            var fieldDefinitionService = IoC.Resolve<FieldDefinitionService>();
            var fieldDefinition = fieldDefinitionService.Get<TArea>(id)?.MakeWritableClone() ??
                new FieldDefinition<TArea>(id, fieldType)
                {
                    SystemId = Guid.Empty
                };

            return new IntOptionFieldDefinitionSeed(fieldDefinition);
        }

        public static IntOptionFieldDefinitionSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.IntOptionFieldDefinition graphQlItem)
        {
            var areaType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(t => t.Name == graphQlItem.AreaType);

            if (areaType == null)
                throw new Exception($"Cannot find the type for the areaType {graphQlItem.AreaType}");

            var seed = new IntOptionFieldDefinitionSeed(new FieldDefinition(graphQlItem.Id, graphQlItem.FieldType, areaType));
            return (IntOptionFieldDefinitionSeed)seed.Update(graphQlItem);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.IntOptionFieldDefinition> Update(SeedBuilder.LitiumGraphQlModel.IntOptionFieldDefinition data)
        {
            base.Update(data);

            _fieldDefinition.Option = new IntOption()
            {
                MultiSelect = data.Option.MultiSelect,
                Items = data.Option.Items.Select(i => new IntOption.Item
                {
                    Name = i.Localizations.ToDictionary(k => k.Culture, v => v.Name),
                    Value = i.Value
                }).ToList()
            };

            return this;
        }

        public IntOptionFieldDefinitionSeed WithIntOption(IntOption option)
        {
            if (!(_fieldDefinition.Option is IntOption))
            {
                _fieldDefinition.Option = new IntOption();
            }

            var intOption = _fieldDefinition.Option as IntOption;
            var fieldDefinitionItems = intOption.Items;

            intOption.MultiSelect = option.MultiSelect;

            foreach (var item in option.Items)
            {
                if (fieldDefinitionItems.FirstOrDefault(i => i.Value == item.Value) is IntOption.Item fieldDefinitionItem)
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
            builder.AppendLine($"\r\n\t\t\t{nameof(IntOptionFieldDefinitionSeed)}.{nameof(Ensure)}<{_fieldDefinition.AreaType.Name}>(\"{_fieldDefinition.Id}\", \"{_fieldDefinition.FieldType}\")");

            var decimalOption = _fieldDefinition.Option as IntOption;
            builder.AppendLine($"\t\t\t\t.{nameof(WithIntOption)}(new IntOption()\r\n\t\t\t\t{{" +
                               $"\r\n\t\t\t\t\t{nameof(TextOption.MultiSelect)} = {decimalOption.MultiSelect.ToString().ToLower()}," +
                               $"\r\n\t\t\t\t\t{nameof(TextOption.Items)} = new List<IntOption.Item>\r\n\t\t\t\t\t{{\r\n\t\t\t\t\t\t{GetIntOptions()}" +
                                "\r\n\t\t\t\t\t}\r\n\t\t\t\t})");

            WritePropertiesMigration(builder);
            builder.AppendLine("\t\t\t\t.Commit();");

            string GetIntOptions()
                => string.Join(",\r\n\t\t\t\t\t\t", decimalOption.Items.Select(i => "new IntOption.Item\r\n\t\t\t\t\t\t{" +
                                                                                 $"\r\n\t\t\t\t\t\t\tValue = {i.Value.ToString()}," +
                                                                                 $"\r\n\t\t\t\t\t\t\tName = {i.Name.GetMigration(7)}" +
                                                                                 "\r\n\t\t\t\t\t\t}"));
        }

    }
}
