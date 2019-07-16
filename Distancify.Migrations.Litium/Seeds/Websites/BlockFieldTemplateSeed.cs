using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.Blocks;
using Litium.FieldFramework;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class BlockFieldTemplateSeed : FieldTemplateSeed<BlockFieldTemplate>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Blocks.BlockFieldTemplate>
    {
        public BlockFieldTemplateSeed(BlockFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static BlockFieldTemplateSeed Ensure(string blockFieldTemplateId)
        {
            var blockFieldTemplate = (BlockFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<BlockFieldTemplate>(blockFieldTemplateId)?.MakeWritableClone();
            if (blockFieldTemplate is null)
            {
                blockFieldTemplate = new BlockFieldTemplate(blockFieldTemplateId)
                {
                    SystemId = Guid.Empty,
                    FieldGroups = new List<FieldTemplateFieldGroup>()
                };
            }

            return new BlockFieldTemplateSeed(blockFieldTemplate);
        }

        public BlockFieldTemplateSeed WithCategory(string blockCategoryId)
        {
            var blockCatagorySystemGuid = IoC.Resolve<CategoryService>().Get(blockCategoryId).SystemId;
            fieldTemplate.CategorySystemId = blockCatagorySystemGuid;

            return this;
        }

        public BlockFieldTemplateSeed WithCategory(Guid blockCategorySystemId)
        {
            fieldTemplate.CategorySystemId = blockCategorySystemId;

            return this;
        }

        public BlockFieldTemplateSeed WithTemplatePath(string templatePath)
        {
            fieldTemplate.TemplatePath = templatePath;

            return this;
        }

        public static BlockFieldTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Blocks.BlockFieldTemplate blockFieldTemplate)
        {
            var seed = new BlockFieldTemplateSeed(new BlockFieldTemplate(blockFieldTemplate.Id));
            return (BlockFieldTemplateSeed)seed.Update(blockFieldTemplate);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Blocks.BlockFieldTemplate> Update(SeedBuilder.LitiumGraphQlModel.Blocks.BlockFieldTemplate data)
        {
            fieldTemplate.SystemId = data.SystemId;
            fieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
            fieldTemplate.CategorySystemId = data.CategorySystemId;
            fieldTemplate.TemplatePath = data.TemplatePath;

            foreach (var fieldGroup in data.FieldGroups)
            {
                AddOrUpdateFieldGroup(fieldTemplate.FieldGroups, fieldGroup.Id, fieldGroup.Fields,
                    fieldGroup.Localizations.ToDictionary(k => k.Culture, v => v.Name), fieldGroup.Collapsed);
            }

            foreach (var localization in data.Localizations)
            {
                if (!string.IsNullOrEmpty(localization.Culture) && !string.IsNullOrEmpty(localization.Name))
                {
                    fieldTemplate.Localizations[localization.Culture].Name = localization.Name;
                }
                else
                {
                    this.Log().Warn("The Field Template with system id {FieldTemplateSystemId} contains a localization with an empty culture and/or name!",
                        data.SystemId.ToString());
                }
            }

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            if (fieldTemplate == null || string.IsNullOrEmpty(fieldTemplate.Id))
            {
                throw new NullReferenceException("At least one Channel Field Template with an ID obtained from the GraphQL endpoint is needed in order to ensure the Channel Field Template");
            }

            builder.AppendLine($"\r\n\t\t\t{nameof(BlockFieldTemplateSeed)}.{nameof(BlockFieldTemplateSeed.Ensure)}(\"{fieldTemplate.Id}\")");

            if (!string.IsNullOrEmpty(fieldTemplate.TemplatePath))
            {
                builder.Append($"\t\t\t\t.{nameof(WithTemplatePath)}(\"{fieldTemplate.TemplatePath}\")");
            }

            if (fieldTemplate.CategorySystemId != Guid.Empty)
            {
                builder.Append($"\t\t\t\t.{nameof(WithCategory)}(Guid.Parse(\"{fieldTemplate.CategorySystemId.ToString()}\"))");
            }

            foreach (var localization in fieldTemplate.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            WriteFieldGroups(fieldTemplate.FieldGroups, builder);

            builder.AppendLine("\t\t\t\t.Commit();");
        }

        /*TODO
         * Icon
         */
    }
}
