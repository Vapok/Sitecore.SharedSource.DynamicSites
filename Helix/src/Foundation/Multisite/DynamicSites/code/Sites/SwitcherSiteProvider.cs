using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Foundation.Multisite.DynamicSites.Caching;
using Sitecore.Foundation.Multisite.DynamicSites.Utilities;
using Sitecore.Sites;
using Sitecore.Web;

namespace Sitecore.Foundation.Multisite.DynamicSites.Sites
{
    [UsedImplicitly]
    public class SwitcherSiteProvider : SiteProvider
    {
        //Create Dynamic Site Cache
        private readonly SiteCache _siteCache = DynamicSiteSettings.GetSiteCache;
        private readonly ProviderHelper<SiteProvider, SiteProviderCollection> _providerHelper;
        private static readonly List<string> OrderedList = new List<string>();

        public SwitcherSiteProvider(ProviderHelper<SiteProvider, SiteProviderCollection> providerHelper)
        {
            _providerHelper = providerHelper;
        }

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

            //Ininitalize The Cache based off of all Site Providers
            foreach (var site in from SiteProvider siteProvider in _providerHelper.Providers
                                 where string.Compare(siteProvider.Name, Name, StringComparison.InvariantCultureIgnoreCase) != 0
                                 from site in siteProvider.GetSites()
                                 select site)
            {
                //Have to show Sitecore Sites in front of website first.
                //Ignore Website and any other site that doesn't have a hostname or a Virtual Folder from Config provider.

                var info = new SiteInfo(site.Properties);

                //If site has been set to not active, then remove it from Provider.
                if (!info.IsActive) continue;
                
                if (string.IsNullOrEmpty(info.HostName) &&
                    (string.IsNullOrEmpty(info.VirtualFolder) || info.VirtualFolder.Equals("/")) &&
                    string.IsNullOrEmpty(info.StartItem))
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
