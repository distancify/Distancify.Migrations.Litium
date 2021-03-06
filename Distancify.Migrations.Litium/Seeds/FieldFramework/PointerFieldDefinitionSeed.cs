﻿using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Definitions;
using Distancify.Migrations.Litium.Seeds.Globalization;
using Litium;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Seeds.FieldFramework
{
    public class PointerFieldDefinitionSeed : FieldDefinitionSeed, ISeedGenerator<PointerFieldDefinition>
    {
        private PointerFieldDefinitionSeed(FieldDefinition fieldDefinition) : base(fieldDefinition)
        {
        }

        public new static PointerFieldDefinitionSeed Ensure<TArea>(string id, string fieldType)
            where TArea : IArea
        {
            var fieldDefinitionService = IoC.Resolve<FieldDefinitionService>();
            var fieldDefinition = fieldDefinitionService.Get<TArea>(id)?.MakeWritableClone() ??
                new FieldDefinition<TArea>(id, fieldType)
                {
                    SystemId = Guid.Empty
                };

            return new PointerFieldDefinitionSeed(fieldDefinition);
        }

        public static PointerFieldDefinitionSeed CreateFrom(PointerFieldDefinition graphQlItem)
        {
            var areaType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(t => t.Name == graphQlItem.AreaType);

            if (areaType == null)
                throw new Exception($"Cannot find the type for the areaType {graphQlItem.AreaType}");

            var seed = new PointerFieldDefinitionSeed(new FieldDefinition(graphQlItem.Id, graphQlItem.FieldType, areaType));
            return (PointerFieldDefinitionSeed)seed.Update(graphQlItem);
        }

        public ISeedGenerator<PointerFieldDefinition> Update(PointerFieldDefinition data)
        {
            base.Update(data);

            _fieldDefinition.Option = new PointerOption()
            {
                MultiSelect = data.Option.MultiSelect,
                EntityType = data.Option.EntityType
            };

            return this;
        }


        public PointerFieldDefinitionSeed WithPointerOption(PointerOption pointerOption)
        {
            _fieldDefinition.Option = pointerOption;

            return this;
        }

        public new void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(PointerFieldDefinitionSeed)}.{nameof(Ensure)}<{_fieldDefinition.AreaType.Name}>(\"{_fieldDefinition.Id}\", \"{_fieldDefinition.FieldType}\")");

            var pointerOption = _fieldDefinition.Option as PointerOption;
            builder.AppendLine($"\t\t\t\t.{nameof(WithPointerOption)}(new PointerOption\r\n\t\t\t\t{{" +
                               $"\r\n\t\t\t\t\tEntityType = \"{pointerOption.EntityType}\"," +
                               $"\r\n\t\t\t\t\tMultiSelect = {pointerOption.MultiSelect.ToString().ToLower()}" +
                                "\r\n\t\t\t\t})");

            WritePropertiesMigration(builder);
            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
