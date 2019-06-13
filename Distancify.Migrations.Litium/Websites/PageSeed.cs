﻿using Litium;
using Litium.Blocks;
using Litium.FieldFramework;
using Litium.Websites;
using System;
using System.Linq;

namespace Distancify.Migrations.Litium.Websites
{
    public class PageSeed : ISeed<PageSeed>
    {
        private readonly Page page;

        protected PageSeed(Page page)
        {
            this.page = page;
        }

        public PageSeed Commit()
        {
            var service = IoC.Resolve<PageService>();

            if (page.SystemId == null || page.SystemId == Guid.Empty)
            {
                page.SystemId = Guid.NewGuid();
                service.Create(page);
                return this;
            }

            service.Update(page);
            return this;
        }


        public static PageSeed Ensure(string pageId, string websiteName, string pageTemplateName)
        {
            var websiteSystemGuid = IoC.Resolve<WebsiteService>().Get(websiteName).SystemId;
            var pageFieldTemplateSystemGuid = IoC.Resolve<FieldTemplateService>().Get<PageFieldTemplate>(pageTemplateName).SystemId;

            var pageClone = IoC.Resolve<PageService>().Get(pageId)?.MakeWritableClone();
            if (pageClone is null)
            {
                pageClone = new Page(pageFieldTemplateSystemGuid, Guid.Empty);
                pageClone.Id = pageId;
                pageClone.SystemId = Guid.Empty;
                pageClone.Localizations["en-US"].Name = pageId;
                pageClone.WebsiteSystemId = websiteSystemGuid;
            }

            return new PageSeed(pageClone);
        }

        public PageSeed WithParrentPage(string parrentPageId)
        {
            page.ParentPageSystemId = IoC.Resolve<PageService>().Get(parrentPageId).SystemId;
            return this;
        }

        public PageSeed WithBlock(string blockId)
        {
            var blockLinkItem = page.Blocks.FirstOrDefault(c => c.Id == blockId);

            if (blockLinkItem == null)
            {
                page.Blocks.Add(
                    new BlockItemContainer(blockId));
                return this;
            }

            return this;
        }


        public void Publish()
        {
            var service = IoC.Resolve<DraftPageService>();
            var draftPageClone = service.Get(page.SystemId).MakeWritableClone();
            service.Update(draftPageClone);
            service.Publish(draftPageClone);
        }

        /*TODO
         * Blocks
         * Localizations
         * ChannelLinks
         * Fields
         * AccessControlList
         * Status
         * PublishedAtUtc
         * PublishedBySystemId
         * WebsiteSystemId
         * SortIndex
         */
    }
}
