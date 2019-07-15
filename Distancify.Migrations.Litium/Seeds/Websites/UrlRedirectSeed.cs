using System;
using System.Collections.Generic;
using System.Linq;
using Litium;
using Litium.Data;
using Litium.Data.Batching;
using Litium.Websites;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class UrlRedirectSeed : ISeed
    {
        private readonly IList<UrlRedirect> entities = new List<UrlRedirect>();

        protected UrlRedirectSeed()
        {
        }

        /// <summary>
        /// Create new batch of redirect updates. Use UrlRedirectSeed.Add(...) to add entries.
        /// </summary>
        /// <returns></returns>
        public static UrlRedirectSeed EnsureBatch()
        {
            return new UrlRedirectSeed();
        }

        public UrlRedirectSeed Add(string url, string redirectToUrl, bool appendRequestQueryString)
        {
            var urlRedirect = new UrlRedirect(url, redirectToUrl);
            urlRedirect.AppendRequestQueryString = appendRequestQueryString;
            entities.Add(urlRedirect);
            return this;
        }

        public void Commit()
        {
            var dataService = IoC.Resolve<DataService>();
            var all = IoC.Resolve<DataService>().CreateQuery<UrlRedirect>().ToList();

            using (BatchData batch = dataService.CreateBatch(null))
            {
                foreach (var ur in entities)
                {
                    var entity = all.FirstOrDefault(r => r.Url.Equals(ur.Url, StringComparison.OrdinalIgnoreCase));
                    if (entity != null)
                    {
                        entity.RedirectToUrl = ur.RedirectToUrl;
                        entity.AppendRequestQueryString = ur.AppendRequestQueryString;
                        batch.Update(entity);
                    }
                    else
                    {
                        batch.Create(ur);
                    }
                }
                batch.Commit();
            }
        }
    }
}
