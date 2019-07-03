using Litium;
using Litium.Blocks;
using Litium.FieldFramework;
using Litium.Websites;
using System;
using System.Linq;

namespace Distancify.Migrations.Litium.Seeds.Website
{
    public class PageSeed : ISeed
    {
        private readonly Page page;
        private bool isPublished;

        protected PageSeed(Page page)
        {
            this.page = page;
        }

        public void Commit()
        {
            var service = IoC.Resolve<PageService>();

            if (page.SystemId == null || page.SystemId == Guid.Empty)
            {
                page.SystemId = Guid.NewGuid();
                service.Create(page);
            }

            service.Update(page);
            if (isPublished)
            {
                var dS = IoC.Resolve<DraftPageService>();
                var draftPageClone = dS.Get(page.SystemId).MakeWritableClone();
                dS.Update(draftPageClone);
                dS.Publish(draftPageClone);
            }
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

        public PageSeed IsParentPage()
        {
            page.ParentPageSystemId = Guid.Empty;
            return this;
        }

        public PageSeed WithParentPage(string parrentPageId)
        {
            page.ParentPageSystemId = IoC.Resolve<PageService>().Get(parrentPageId).SystemId;
            return this;
        }

        public PageSeed WithBlock(string containerId, string blockId)
        {
            //BUG: For some reason is blocks not added to the page.. why???

            var blockItemContainerItem = page.Blocks.FirstOrDefault(c => c.Id == containerId);
            if (blockItemContainerItem == null)
            {
                blockItemContainerItem = new BlockItemContainer(containerId);
                page.Blocks.Add(blockItemContainerItem);
            }


            var blockGuid = IoC.Resolve<BlockService>().Get(blockId).SystemId;

            foreach (var i in blockItemContainerItem.Items)
            {
                if (i is BlockItemLink && ((BlockItemLink)i).BlockSystemId == blockGuid)
                {
                    return this;
                }

                continue;
            }

            blockItemContainerItem.Items.Add(new BlockItemLink(blockGuid));

            return this;
        }


        public PageSeed IsPublished()
        {
            isPublished = true;
            return this;
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
