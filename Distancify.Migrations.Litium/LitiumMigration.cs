﻿using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.Migrations.Litium
{
    /// <summary>
    /// Base migration for Litium projects. Use this as the root of your migration tree. It
    /// ensures that your Litium database is properly bootstrapped.
    /// </summary>
    public abstract class LitiumMigration : Migration
    {
        public static class Countries
        {
            public const string SE = nameof(SE);
            public const string GB = nameof(GB);
        }

        public static class Cultures
        {
            public const string en_GB = "en-GB";
            public const string en_US = "en-US";
            public const string de_DE = "de-DE";
            public const string sv_SE = "sv-SE";
            public const string da_DK = "da-DK";
            public const string nl_NL = "nl-NL";
            public const string fi_FI = "fi-FI";
            public const string fr_FR = "fr-FR";
            public const string nb_NO = "nb-NO";
            public const string es_ES = "es-ES";
        }

        public static class Currencies
        {
            public const string SEK = nameof(SEK);
            public const string USD = nameof(USD);
            public const string GBP = nameof(GBP);
        }

        public static class BootstrapConstants
        {
            public const string DefaultSystemUserTemplate = nameof(DefaultSystemUserTemplate);
            public const string DefaultSystemGroupTemplate = nameof(DefaultSystemGroupTemplate);
            public const string DefaultFileTemplate = nameof(DefaultFileTemplate);
            public const string DefaultFolderTemplate = nameof(DefaultFolderTemplate);
            public const string DefaultImageFileTemplate = "Image";
        }

        public static class SystemConstants
        {
            public const string Visitors = nameof(Visitors);
        }

        protected void SetDefaultValueForWebsiteField(string fieldId, object defaultValue)
        {
            var websiteService = IoC.Resolve<WebsiteService>();

            foreach (var website in websiteService.GetAll().Select(website => website.MakeWritableClone()))
            {
                if (!website.Fields.TryGetValue(fieldId, out object currentValue))
                {
                    website.Fields.AddOrUpdateValue(fieldId, defaultValue);
                    websiteService.Update(website);
                }
            }
        }

        protected void SetDefaultValueForChannelField(string fieldId, object defaultValue)
        {
            var channelService = IoC.Resolve<ChannelService>();

            foreach (var channel in channelService.GetAll().Select(channel => channel.MakeWritableClone()))
            {
                if (!channel.Fields.TryGetValue(fieldId, out object currentValue))
                {
                    channel.Fields.AddOrUpdateValue(fieldId, defaultValue);
                    channelService.Update(channel);
                }
            }
        }

        protected void SetDefaultValueForPageField(string fieldId, string templateId, object defaultValue)
        {
            var pageService = IoC.Resolve<PageService>();
            var templateSystemId = IoC.Resolve<FieldTemplateService>().Get<PageFieldTemplate>(templateId).SystemId;

            var pages = GetAllPages()
                .Where(page => page.FieldTemplateSystemId.Equals(templateSystemId))
                .Select(page => page.MakeWritableClone());

            foreach (var page in pages)
            {
                if (!page.Fields.TryGetValue(fieldId, out object currentValue))
                {
                    page.Fields.AddOrUpdateValue(fieldId, defaultValue);
                    pageService.Update(page);
                }
            }
        }

        protected void SetTemplatePath(string pageId, Type controllerType, string actionName)
        {
            var templatePath = $"~/MVC:{controllerType.MapTo<string>()}:{actionName}";
            var fieldTemplateService = IoC.Resolve<FieldTemplateService>();
            var checkoutPageFieldTemplate = fieldTemplateService.Get<PageFieldTemplate>(pageId).MakeWritableClone() as PageFieldTemplate;
            checkoutPageFieldTemplate.TemplatePath = templatePath;
            fieldTemplateService.Update(checkoutPageFieldTemplate);
        }

        private List<Page> GetAllPages()
        {
            var websiteSystemIds = IoC.Resolve<WebsiteService>().GetAll().Select(website => website.SystemId);
            var pageService = IoC.Resolve<PageService>();

            var pages = websiteSystemIds.SelectMany(websiteSystemId => pageService.GetChildPages(Guid.Empty, websiteSystemId)).ToList();
            var subpages = pages.SelectMany(page => GetDescendingPages(page)).ToList();
            pages.AddRange(subpages);

            return pages;

            List<Page> GetDescendingPages(Page page)
            {
                var descendants = pageService.GetChildPages(page.SystemId, page.WebsiteSystemId).ToList();
                var grandchildren = descendants.SelectMany(descendant => GetDescendingPages(descendant)).ToList();
                descendants.AddRange(grandchildren);

                return descendants;
            }
        }
    }
}