using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.SharedSource.DynamicSites.Utilities;
using Sitecore.Sites;
using Sitecore.Xml;

namespace Sitecore.SharedSource.DynamicSites.Sites
{
    [UsedImplicitly]
    public class DynamicSitesProvider : SiteProvider
    {
        private string _dynamicConfigPath;
        private SiteCollection _sites;
        private List<KeyValuePair<string, Site>> _dynamicSiteDictionary;
 
        public override Site GetSite(string siteName)
        {
            Assert.ArgumentNotNullOrEmpty(siteName,"siteName");
            InitializeSites();
            return _dynamicSiteDictionary.GetSiteByKey(DynamicSiteManager.CleanCacheKeyName(siteName));
        }

        public override SiteCollection GetSites()
        {
            InitializeSites();
            return _sites;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            Assert.ArgumentNotNullOrEmpty(name, "name");
            Assert.ArgumentNotNull(config, "config");
            base.Initialize(name, config);
            _dynamicConfigPath = config["siteConfig"];
        }

        private void InitializeSites()
        {
            if (DynamicSiteSettings.GetSiteCache.Count() > 0 && _sites != null && _dynamicSiteDictionary.Count > 0) return;

            Assert.IsNotNullOrEmpty(_dynamicConfigPath, "No siteConfig specified in DynamicSiteProvider configuration.");
            var collection = new SiteCollection();

            var nodes = Factory.GetConfigNodes(FileUtil.MakePath(_dynamicConfigPath, "defaultsite", '/'));
            Assert.IsFalse((nodes.Count > 1 ? 1 : 0) != 0, "Duplicate Dynamic Default Site Definition.");

            if (nodes.Count == 0) return;

            var defaultSite = ParseDefaultNode(nodes[0]);

            //Create Dictionary
            var siteDictionary = DynamicSiteManager.GetDynamicSitesDictionary(defaultSite);
            
            //Set Site Collection
            foreach (var keyValuePair in siteDictionary)
            {
                collection.Add(keyValuePair.Value);
            }

            ResolveInheritance(collection, siteDictionary);

            _sites = collection;
            _dynamicSiteDictionary = siteDictionary;
        }

        private Site ParseDefaultNode(XmlNode node)
        {
            var attributeDictionary = XmlUtil.GetAttributeDictionary(node);
            return new Site(DynamicSiteSettings.SiteName, attributeDictionary);
        }

        private void AddInheritedProperties(Site site, List<KeyValuePair<string, Site>> siteDictionary)
        {
            var index = site.Properties["inherits"];
            var inheritedSite = siteDictionary.GetSiteByKey(DynamicSiteManager.CleanCacheKeyName(index));

            Assert.IsNotNull(inheritedSite, "Could not find base site '{0}' for site '{1}'.", DynamicSiteManager.CleanCacheKeyName(index), site.Name);

            foreach (var keyValuePair in inheritedSite.Properties.Where(keyValuePair => !site.Properties.ContainsKey(keyValuePair.Key)))
            {
                site.Properties[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        private void ResolveInheritance(SiteCollection sites, List<KeyValuePair<string, Site>> siteDictionary)
        {
            foreach (var site in sites.Where(site => !string.IsNullOrEmpty(site.Properties["inherits"])))
            {
                AddInheritedProperties(site, siteDictionary);
            }
        }
    }
}
