using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Litium;
using Litium.Blocks;
using Litium.Common;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Websites;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class PageSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Websites.Page>
    {
        private readonly Page _page;
        private string _fieldTemplateId;
        private bool _isPublished;
        private bool _isNewPage;

        private List<string> _channelLinksIds;

        protected PageSeed(Page page, string fieldTemplateId, bool isNewPage = false)
        {
            _page = page;
            _fieldTemplateId = fieldTemplateId;
            _isNewPage = isNewPage;
        }

        public void Commit()
        {
            var service = IoC.Resolve<PageService>();

            if (_isNewPage)
            {
                service.Create(_page);
            }
            else
            {
                service.Update(_page);
            }

            if (_isPublished)
            {
                var dS = IoC.Resolve<DraftPageService>();
                var draftPageClone = dS.Get(_page.SystemId).MakeWritableClone();

                UpdateDraftPageWithBlocks(draftPageClone);

                dS.Update(draftPageClone);
                dS.Publish(draftPageClone);
            }

            void UpdateDraftPageWithBlocks(DraftPage draftPage)
            {
                foreach (var block in _page.Blocks)
                {
                    var blockContainer = draftPage.Blocks.FirstOrDefault(b => b.Id == block.Id);

                    if (blockContainer == null)
                    {
                        blockContainer = new BlockItemContainer(block.Id);
                        draftPage.Blocks.Add(blockContainer);
                    }

                    foreach (var blockLink in block.Items.OfType<BlockItemLink>())
                    {
                        if (!blockContainer.Items.Any(i => i is BlockItemLink && ((BlockItemLink)i).BlockSystemId == blockLink.BlockSystemId))
                        {
                            blockContainer.Items.Add(new BlockItemLink(blockLink.BlockSystemId));
                        }
                    }

                }
            }
        }

        public static PageSeed Ensure(string pageId, string pageFieldTemplateId)
        {
            var pageFieldTemplateSystemGuid = IoC.Resolve<FieldTemplateService>().Get<PageFieldTemplate>(pageFieldTemplateId).SystemId;
            var pageClone = IoC.Resolve<PageService>().Get(pageId)?.MakeWritableClone();
            var isNewPage = false;

            if (pageClone is null)
            {
                pageClone = new Page(pageFieldTemplateSystemGuid, Guid.Empty)
                {
                    Id = pageId,
                    SystemId = Guid.NewGuid()
                };
                isNewPage = true;
            }

            return new PageSeed(pageClone, pageFieldTemplateId, isNewPage);
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

            return new PageSeed(new Page(pageFieldTemplateSystemId, Guid.Empty) { SystemId = pageSystemId }, pageFieldTemplateId, true);
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

        public PageSeed WithChannelLink(string channelId)
        {
            var channelSystemId = IoC.Resolve<ChannelService>().Get(channelId).SystemId;

            return this.WithChannelLink(channelSystemId);
        }

        public PageSeed WithBlock(string containerId, Guid blockSystemId)
        {
            //BUG: For some reason is blocks not added to the page.. why???
            var blockContainer = _page.Blocks.FirstOrDefault(c => c.Id == containerId);
            if (blockContainer == null)
            {
                blockContainer = new BlockItemContainer(containerId);
                _page.Blocks.Add(blockContainer);
            }

            if (blockContainer.Items.Any(i => i is BlockItemLink && ((BlockItemLink)i).BlockSystemId == blockSystemId))
            {
                return this;
            }

            blockContainer.Items.Add(new BlockItemLink(blockSystemId));
            return this;
        }

        public PageSeed IsPublished()
        {
            _isPublished = true;
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

            _channelLinksIds = data.ChannelLinks?.Select(c => c.Channel.Id).ToList() ?? new List<string>();

            foreach (var blockContainer in data.BlockContainers)
            {
                _page.Blocks.Add(new BlockItemContainer(blockContainer.Id)
                {
                    Items = blockContainer.Blocks.Select(b => (BlockItem)new BlockItemLink(b.SystemId)).ToList()
                });
            }

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

            foreach (var channelLink in _channelLinksIds)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithChannelLink)}(\"{channelLink}\")");
            }

            foreach (var block in _page.Blocks)
            {
                foreach (var item in block.Items.OfType<BlockItemLink>())
                {
                    builder.AppendLine($"\t\t\t\t.{nameof(WithBlock)}(\"{block.Id}\", Guid.Parse(\"{item.BlockSystemId.ToString()}\"))");
                }
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
