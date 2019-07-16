using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Litium;
using Litium.Blocks;
using Litium.Common;
using Litium.FieldFramework;
using Litium.Websites;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class PageSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Websites.Page>
    {
        private readonly Page _page;
        private string _fieldTemplateId;
        private bool isPublished;

        protected PageSeed(Page page, string fieldTemplateId)
        {
            _page = page;
            _fieldTemplateId = fieldTemplateId;
        }

        public void Commit()
        {
            var service = IoC.Resolve<PageService>();

            if (_page.SystemId == null || _page.SystemId == Guid.Empty)
            {
                _page.SystemId = Guid.NewGuid();
                service.Create(_page);
            }

            service.Update(_page);
            if (isPublished)
            {
                var dS = IoC.Resolve<DraftPageService>();
                var draftPageClone = dS.Get(_page.SystemId).MakeWritableClone();
                dS.Update(draftPageClone);
                dS.Publish(draftPageClone);
            }
        }

        public static PageSeed Ensure(string pageId, string pageFieldTemplateId)
        {
            var pageFieldTemplateSystemGuid = IoC.Resolve<FieldTemplateService>().Get<PageFieldTemplate>(pageFieldTemplateId).SystemId;

            var pageClone = IoC.Resolve<PageService>().Get(pageId)?.MakeWritableClone();
            if (pageClone is null)
            {
                pageClone = new Page(pageFieldTemplateSystemGuid, Guid.Empty);
                pageClone.Id = pageId;
                pageClone.SystemId = Guid.Empty;
            }

            return new PageSeed(pageClone, pageFieldTemplateId);
        }

        public static PageSeed Ensure(Guid pageSystemId, string pageFieldTemplateId)
        {
            var pageFieldTemplateSystemId = IoC.Resolve<FieldTemplateService>().Get<PageFieldTemplate>(pageFieldTemplateId).SystemId;
            var pageClone = IoC.Resolve<PageService>().Get(pageSystemId)?.MakeWritableClone();

            if (pageClone != null)
            {
                pageClone.FieldTemplateSystemId = pageFieldTemplateSystemId;
                return new PageSeed(pageClone, pageFieldTemplateId);
            }

            return new PageSeed(new Page(pageFieldTemplateSystemId, Guid.Empty), pageFieldTemplateId);
        }

        public PageSeed IsRootPage()
        {
            _page.ParentPageSystemId = Guid.Empty;
            return this;
        }

        public PageSeed WithParentPage(string parrentPageId)
        {
            _page.ParentPageSystemId = IoC.Resolve<PageService>().Get(parrentPageId).SystemId;
            return this;
        }

        public PageSeed WithParentPage(Guid parentPageSystemId)
        {
            _page.ParentPageSystemId = parentPageSystemId;
            return this;
        }

        public PageSeed WithName(string culture, string name)
        {
            if (!_page.Localizations.Any(l => l.Key.Equals(culture)) ||
                !_page.Localizations[culture].Name.Equals(name))
            {
                _page.Localizations[culture].Name = name;
            }

            return this;
        }

        public PageSeed WithStatus(ContentStatus contentStatus)
        {
            _page.Status = contentStatus;

            return this;
        }

        public PageSeed WithStatus(short contentStatus)
        {
            _page.Status = (ContentStatus)contentStatus;

            return this;
        }

        public PageSeed WithWebsite(Guid websiteSystemId)
        {
            _page.WebsiteSystemId = websiteSystemId;
            return this;
        }

        public PageSeed WithChannelLink(Guid channelSystemId)
        {
            if (_page.ChannelLinks == null)
            {
                _page.ChannelLinks = new List<PageToChannelLink>();
            }

            if (!_page.ChannelLinks.Any(cl => cl.ChannelSystemId == channelSystemId))
            {
                _page.ChannelLinks.Add(new PageToChannelLink(channelSystemId));
            }

            return this;
        }

        public PageSeed WithBlock(string containerId, string blockId)
        {
            //BUG: For some reason is blocks not added to the page.. why???

            var blockItemContainerItem = _page.Blocks.FirstOrDefault(c => c.Id == containerId);
            if (blockItemContainerItem == null)
            {
                blockItemContainerItem = new BlockItemContainer(containerId);
                _page.Blocks.Add(blockItemContainerItem);
            }

            var blockSystemId = IoC.Resolve<BlockService>().Get(blockId).SystemId;

            if (blockItemContainerItem.Items.Any(i => i is BlockItemLink && ((BlockItemLink)i).BlockSystemId == blockSystemId))
            {
                return this;
            }

            blockItemContainerItem.Items.Add(new BlockItemLink(blockSystemId));
            return this;
        }

        public PageSeed IsPublished()
        {
            isPublished = true;
            return this;
        }

        public static PageSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Websites.Page page)
        {
            var seed = new PageSeed(new Page(page.FieldTemplate.SystemId, page.ParentPageSystemId), page.FieldTemplate.Id);
            return (PageSeed)seed.Update(page);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Websites.Page> Update(SeedBuilder.LitiumGraphQlModel.Websites.Page data)
        {
            _page.SystemId = data.SystemId;
            _page.WebsiteSystemId = data.WebsiteSystemId;
            _page.Status = (ContentStatus)data.Status;
            _fieldTemplateId = data.FieldTemplate.Id;

            foreach (var localization in data.Localizations)
            {
                if (!string.IsNullOrEmpty(localization.Culture) && !string.IsNullOrEmpty(localization.Name))
                {
                    _page.Localizations[localization.Culture].Name = localization.Name;
                }
                else
                {
                    this.Log().Warn("The page with system id {PageSystemId} contains a localization with an empty culture and/or name!", data.SystemId.ToString());
                }
            }

            _page.ChannelLinks = data.ChannelLinks?.Select(c => new PageToChannelLink(c.ChannelSystemId)).ToList() ?? new List<PageToChannelLink>();

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(PageSeed)}.{nameof(Ensure)}(Guid.Parse(\"{_page.SystemId.ToString()}\"), \"{_fieldTemplateId}\")");

            foreach (var localization in _page.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            builder.AppendLine($"\t\t\t\t.{nameof(WithWebsite)}(Guid.Parse(\"{_page.WebsiteSystemId}\"))");

            foreach (var channelLink in _page.ChannelLinks)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithChannelLink)}(Guid.Parse(\"{channelLink.ChannelSystemId}\"))");
            }

            if (_page.ParentPageSystemId == Guid.Empty)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(IsRootPage)}()");
            }
            else
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithParentPage)}(Guid.Parse(\"{_page.ParentPageSystemId}\"))");
            }

            if (_page.Status.Equals(ContentStatus.Published))
            {
                builder.AppendLine($"\t\t\t\t.{nameof(IsPublished)}()");
            }
            else
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithStatus)}({(short)_page.Status})");
            }

            builder.AppendLine("\t\t\t\t.Commit();");
        }


        /*TODO
         * Blocks
         * Fields
         * AccessControlList
         * PublishedAtUtc
         * PublishedBySystemId
         * SortIndex
         */
    }
}
