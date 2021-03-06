﻿using Distancify.Migrations.Litium.Extensions;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Fields;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class FieldData
    {
        public string FieldId { get; set; }
        public string Culture { get; set; }
        public object Value { get; set; }

        public FieldData(string fieldId, object value, string culture = null)
        {
            FieldId = fieldId;
            Value = value;
            Culture = culture;
        }

        public void WriteMigration(StringBuilder builder, int spacing = 4)
        {
            builder.Append($"{new string('\t', spacing)}.WithField(\"{FieldId.CapitalizeFirstLetter()}\", ");

            if (int.TryParse(Value.ToString(), out int i))
            {
                builder.Append($"{i.ToString()}");
            }
            else if (Guid.TryParse(Value.ToString(), out Guid g))
            {
                builder.Append($"Guid.Parse(\"{g.ToString()}\")");
            }
            else if (bool.TryParse(Value.ToString(), out bool b))
            {
                builder.Append($"{b.ToString().ToLower()}");
            }
            else if (decimal.TryParse(Value.ToString(), out decimal d))
            {
                builder.Append($"{d.ToString()}m");
            }
            else if (TryParseJson<List<string>>(Value.ToString(), out var items))
            {
                builder.Append($"new List<string> {{ {string.Join(", ", items.Select(it => $"\"{it}\""))} }}");
            }
            else if (TryParseJson<PointerPageItem>(Value.ToString(), out var p))
            {
                builder.Append(GetPointerPageItemMigration(p, spacing));
            }
            else if (TryParseJson<List<PointerPageItem>>(Value.ToString(), out var pl))
            {
                builder.Append($"new List<PointerPageItem> {{\r\n{new string('\t', spacing + 1)}" +
                               string.Join($",\r\n{new string('\t', spacing + 1)}", pl.Select(pi => GetPointerPageItemMigration(pi, spacing + 1))) +
                               $"\r\n{new string('\t', spacing)}}}");
            }
            else if (Value is string s)
            {
                builder.Append($"{s.ToLiteral()}");
            }
            else
            {
                throw new NotSupportedException($"The field type for field Id {FieldId} is not supported when building field values");
            }

            builder.Append(!string.IsNullOrEmpty(Culture) ? $", \"{Culture.Replace("_", "-")}\"" : "");
            builder.Append(")\r\n");

            bool TryParseJson<T>(string text, out T result)
            {
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                result = JsonConvert.DeserializeObject<T>(text, settings);
                return success;
            }

            string GetPointerPageItemMigration(PointerPageItem p, int padding)
                => $"new PointerPageItem {{\r\n{new string('\t', padding + 1)}" +
                                       $"{nameof(PointerPageItem.EntitySystemId)} = Guid.Parse(\"{p.EntitySystemId.ToString()}\")," +
                                       $"\r\n{new string('\t', padding + 1)}" +
                                       $"{nameof(PointerPageItem.ChannelSystemId)} = Guid.Parse(\"{p.ChannelSystemId.ToString()}\")" +
                                       $"\r\n{new string('\t', padding)}}}";
        }

    }
}
