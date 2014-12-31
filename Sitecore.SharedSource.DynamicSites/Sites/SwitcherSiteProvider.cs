using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Diagnostics;
using Sitecore.SharedSource.DynamicSites.Caching;
using Sitecore.SharedSource.DynamicSites.Utilities;
using Sitecore.Sites;
using Sitecore.Web;

namespace Sitecore.SharedSource.DynamicSites.Sites
{
    [UsedImplicitly]
    public class SwitcherSiteProvider : SiteProvider
    {
        //Create Sitecore Cache
        private readonly SiteCache _siteCache = DynamicSiteSettings.GetSiteCache;
        private static readonly List<string> OrderedList = new List<string>(); 

        public override Site GetSite(string siteName)
        {
            Assert.ArgumentNotNullOrEmpty(siteName,"siteName");
            InitializeCache();
            return _siteCache.GetSite(siteName);
        }

        public override SiteCollection GetSites()
        {
            InitializeCache();
            return _siteCache.GetAllSites(OrderedList);
        }

        private void InitializeCache()
        {
            if (_siteCache == null) return;
            if (_siteCache.Count() > 0) return;

            //Reset Ordered List
            if (OrderedList.Count > 0)
                OrderedList.Clear();

            var deferredList = new List<string>();

            //Reset SiteContextFactory so that static variables clear.
            SiteContextFactory.Reset();

            //For Sitecore 7, Need to load Config Provider First.
            //For Sitecore 8, this is resolved. The difference is that in 8, there's a provider called "sitecore".
            //If the "sitecore" provider exists, we don't need to make any changes.
            //If it doesn't, then we're running Sitecore 7 and need to load some config sites first.
            var sitecoreProvider = SiteManager.Providers["sitecore"];
            if (sitecoreProvider == null)
            {
                //Sitecore 7
                var configProvider = SiteManager.Providers["config"];
                foreach (var site in configProvider.GetSites())
                {
                    var info = new SiteInfo(site.Properties);
                    if (string.IsNullOrEmpty(info.HostName) &&
                        (string.IsNullOrEmpty(info.VirtualFolder) || info.VirtualFolder.Equals("/")))
                    {
                        if (!deferredList.Contains(site.Name))
                            deferredList.Add(site.Name);
                    }
                    else
                    {
                        if (!OrderedList.Contains(site.Name))
                            OrderedList.Add(site.Name);
                    }
                }
            }
           
            //Ininitalize The Cache based off of all Site Providers
            foreach (var site in from SiteProvider siteProvider in SiteManager.Providers where string.Compare(siteProvider.Name, Name, StringComparison.InvariantCultureIgnoreCase) != 0 from site in siteProvider.GetSites() select site)
            {
                //Have to show Sitecore Sites in front of website first.
                //Ignore Website and any other site that doesn't have a hostname or a Virtual Folder from Config provider.

                var info = new SiteInfo(site.Properties);
                if (string.IsNullOrEmpty(info.HostName) &&
                    (string.IsNullOrEmpty(info.VirtualFolder) || info.VirtualFolder.Equals("/")))
                {
                    if (!deferredList.Contains(site.Name))
                        deferredList.Add(site.Name);
                }
                else
                {
                    if (!OrderedList.Contains(site.Name))
                        OrderedList.Add(site.Name);
                }

                if (!_siteCache.ContainsSite(site))
                    _siteCache.AddSite(site);
            }

            if (deferredList.Count > 0)
                OrderedList.AddRange(deferredList);
        }
    }
}
