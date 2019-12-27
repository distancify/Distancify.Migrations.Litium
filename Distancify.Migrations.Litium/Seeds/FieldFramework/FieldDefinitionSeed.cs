using Distancify.Migrations.Litium.Extensions;
using Litium;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Litium.Validations;

namespace Distancify.Migrations.Litium.Seeds.FieldFramework
{
    public class FieldDefinitionSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.FieldFramework.FieldDefinition>
    {
        protected readonly FieldDefinition _fieldDefinition;

        protected FieldDefinitionSeed(FieldDefinition fieldDefinition)
        {
            _fieldDefinition = fieldDefinition;
        }

        public Guid Commit()
        {
            //TODO: Figure out why we get validation errors even if some fields are not marked as system defined (even though they are)
            try
            {
                if (_fieldDefinition.SystemDefined)
                    return _fieldDefinition.SystemId;

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

                return _fieldDefinition.SystemId;
            }
            catch (ValidationException ex)
            {
                this.Log().Error(ex.Message, ex);
                
            }

            return Guid.Empty;
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

        public FieldDefinitionSeed WithMultiCulture(bool value)
        {
            if (_fieldDefinition.SystemId == Guid.Empty)//Cannot change this value for existing fields
            {
                _fieldDefinition.MultiCulture = value;
            }

            return this;
        }

        public FieldDefinitionSeed WithCanBeGridColumn(bool value)
        {
            _fieldDefinition.CanBeGridColumn = value;
            return this;
        }


        public FieldDefinitionSeed WithCanBeGridFilter(bool value)
        {
            _fieldDefinition.CanBeGridFilter = value;
            return this;
        }

        public FieldDefinitionSeed WithEditable(bool value)
        {
            _fieldDefinition.Editable = value;
            return this;
        }

        public FieldDefinitionSeed WithName(string culture, string name)
        {
            if (!_fieldDefinition.Localizations.Any(l => l.Key.Equals(culture)) ||
                !_fieldDefinition.Localizations[culture].Name.Equals(name))
            {
                _fieldDefinition.Localizations[culture].Name = name;
            }
            return this;
        }

        public FieldDefinitionSeed WithNames(Dictionary<string, string> localizedNamesByCulture)
        {
            foreach (var item in localizedNamesByCulture)
            {
                WithName(item.Key, item.Value);
            }

            return this;
        }

        public FieldDefinitionSeed WithDescription(string culture, string description)
        {
            if (!_fieldDefinition.Localizations.Any(l => l.Key.Equals(culture)) ||
                !description.Equals(_fieldDefinition.Localizations[description].Description))
            {
                _fieldDefinition.Localizations[culture].Description = description;
            }

            return this;
        }

        public FieldDefinitionSeed WithDescriptions(Dictionary<string, string> localizedDescriptionsByCulture)
        {
            foreach (var item in localizedDescriptionsByCulture)
            {
                WithDescription(item.Key, item.Value);
            }

            return this;
        }

        public static FieldDefinitionSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.FieldFramework.FieldDefinition graphQlItem)
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

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.FieldFramework.FieldDefinition> Update(SeedBuilder.LitiumGraphQlModel.FieldFramework.FieldDefinition data)
        {
            _fieldDefinition.MultiCulture = data.MultiCulture;
            _fieldDefinition.CanBeGridColumn = data.CanBeGridColumn;
            _fieldDefinition.CanBeGridFilter = data.CanBeGridFilter;
            _fieldDefinition.FieldType = data.FieldType;

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

            WritePropertiesMigration(builder);

            builder.AppendLine("\t\t\t\t.Commit();");
        }

        protected void WritePropertiesMigration(StringBuilder builder)
        {
            if (_fieldDefinition.Localizations.Any())
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithNames)}({_fieldDefinition.Localizations.ToDictionary(k => k.Key, v => v.Value.Name).GetMigration(4)})");
            }

            builder.AppendLine($"\t\t\t\t.{nameof(WithMultiCulture)}({_fieldDefinition.MultiCulture.ToString().ToLower()})");
            builder.AppendLine($"\t\t\t\t.{nameof(WithCanBeGridColumn)}({_fieldDefinition.CanBeGridColumn.ToString().ToLower()})");
            builder.AppendLine($"\t\t\t\t.{nameof(WithCanBeGridFilter)}({_fieldDefinition.CanBeGridFilter.ToString().ToLower()})");
        }
    }
}