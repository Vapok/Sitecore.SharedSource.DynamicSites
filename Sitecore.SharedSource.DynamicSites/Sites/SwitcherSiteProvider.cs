using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Diagnostics;
using Sitecore.SharedSource.DynamicSites.Caching;
using Sitecore.SharedSource.DynamicSites.Utilities;
using Sitecore.Sites;

namespace Sitecore.SharedSource.DynamicSites.Sites
{
    [UsedImplicitly]
    public class SwitcherSiteProvider : SiteProvider
    {
        //Create Sitecore Cache
        private static readonly SiteCache SiteCache = new SiteCache(StringUtil.ParseSizeString(DynamicSiteSettings.MaxCacheSize)){Enabled = true};
        private static List<string> OrderedList = new List<string>(); 
        private readonly object _lock = new object();

        public override Site GetSite(string siteName)
        {
            Assert.ArgumentNotNullOrEmpty(siteName,"siteName");
            InitializeCache();
            return SiteCache.GetSite(siteName);
        }

        public override SiteCollection GetSites()
        {
            InitializeCache();
            return SiteCache.GetAllSites(OrderedList);
        }

        private void InitializeCache()
        {
            if (SiteCache == null) return;
            if (SiteCache.Count() > 0) return;

            lock (_lock)
            {
                if (SiteCache == null) return;
                if (SiteCache.Count() > 0) return;

                //Ininitalize The Cache based off of all Site Providers
                foreach (var site in from SiteProvider siteProvider in SiteManager.Providers where string.Compare(siteProvider.Name, Name, StringComparison.InvariantCultureIgnoreCase) != 0 from site in siteProvider.GetSites() select site)
                {
                    OrderedList.Add(site.Name);
                    SiteCache.AddSite(site);
                }
            }
        }
    }
}
