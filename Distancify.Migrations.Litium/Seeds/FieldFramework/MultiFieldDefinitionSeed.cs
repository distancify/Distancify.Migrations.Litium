using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Definitions;
using Litium;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime;
using System;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.FieldFramework
{
    public class MultiFieldDefinitionSeed : FieldDefinitionSeed, ISeedGenerator<MultiFieldDefinition>
    {
        private MultiFieldDefinitionSeed(FieldDefinition fieldDefinition) : base(fieldDefinition)
        {
        }

        public static MultiFieldDefinitionSeed Ensure<TArea>(string id)
            where TArea : IArea
        {
            var fieldDefinitionService = IoC.Resolve<FieldDefinitionService>();
            var fieldDefinition = fieldDefinitionService.Get<TArea>(id)?.MakeWritableClone() ??
                new FieldDefinition<TArea>(id, SystemFieldTypeConstants.MultiField)
                {
                    SystemId = Guid.Empty
                };

            return new MultiFieldDefinitionSeed(fieldDefinition);
        }

        public static MultiFieldDefinitionSeed CreateFrom(MultiFieldDefinition graphQlItem)
        {
            var areaType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(t => t.Name == graphQlItem.AreaType);

            if (areaType == null)
                throw new Exception($"Cannot find the type for the areaType {graphQlItem.AreaType}");

            var seed = new MultiFieldDefinitionSeed(new FieldDefinition(graphQlItem.Id, graphQlItem.FieldType, areaType));
            return (MultiFieldDefinitionSeed)seed.Update(graphQlItem);
        }

        public MultiFieldDefinitionSeed WithMultiFieldOption(MultiFieldOption option)
        {
            if (!(_fieldDefinition.Option is MultiFieldOption))
            {
                _fieldDefinition.Option = new MultiFieldOption();
            }

            var multiFieldOption = _fieldDefinition.Option as MultiFieldOption;

            multiFieldOption.Fields = option.Fields;
            multiFieldOption.IsArray = option.IsArray;

            return this;
        }

        public ISeedGenerator<MultiFieldDefinition> Update(MultiFieldDefinition data)
        {
            base.Update(data);

            _fieldDefinition.Option = new MultiFieldOption()
            {
                Fields = data.Option.Fields,
                IsArray = data.Option.IsArray
            };

            return this;
        }

        public new void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(MultiFieldDefinitionSeed)}.{nameof(Ensure)}<{_fieldDefinition.AreaType.Name}>(\"{_fieldDefinition.Id}\", \"{_fieldDefinition.FieldType}\")");

            var multiFieldOption = _fieldDefinition.Option as MultiFieldOption;

            builder.AppendLine($"\t\t\t\t.{nameof(WithMultiFieldOption)}(new MultiFieldOption\r\n\t\t\t\t{{" +
                               $"\r\n\t\t\t\t\tFields = new List<string>{{{string.Join(", ", multiFieldOption.Fields.Select(f => $"\"{f}\""))}}}," +
                               $"\r\n\t\t\t\t\tIsArray = {multiFieldOption.IsArray.ToString().ToLower()}" +
                               $"\r\n\t\t\t\t}})");

            WritePropertiesMigration(builder);
            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
