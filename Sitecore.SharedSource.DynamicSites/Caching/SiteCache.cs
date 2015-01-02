using System.Collections;
using Sitecore.Caching;
using Sitecore.SharedSource.DynamicSites.Utilities;
using Sitecore.Sites;

namespace Sitecore.SharedSource.DynamicSites.Caching
{
    internal class SiteCache : CustomCache
    {
        public SiteCache(long maxSize) : base(DynamicSiteSettings.CacheKey, maxSize)
        {
        }

        //AddSite
        public void AddSite(Site siteItem)
        {
            if (ContainsSite(siteItem))
            {
                RemoveSite(siteItem);
            }

            var cacheItem = new SiteCacheItem(siteItem);
            InnerCache.Add(siteItem.Name, cacheItem);
        }

        //GetSite
        public Site GetSite(string name)
        {
            if (!ContainsSite(name)) return null;
            return (Site)InnerCache.GetValue(name);
        }

        private void RemoveSite(Site siteItem)
        {
            if (ContainsSite(siteItem))
            {
                //Refresh Information
                InnerCache.Remove(siteItem.Name);
            }
        }

        //GetAllSites
        public SiteCollection GetAllSites()
        {
            return GetAllSites(InnerCache.GetCacheKeys());
        }

        //ContainsSite
        public bool ContainsSite(Site siteItem)
        {
            return InnerCache.ContainsKey(siteItem.Name);
        }

        public bool ContainsSite(string name)
        {
            return InnerCache.ContainsKey(name);
        }

        // Count
        public int Count()
        {
            return InnerCache.Count;
        }

        public SiteCollection GetAllSites([NotNull] IEnumerable orderedList)
        {
            var collection = new SiteCollection();

            foreach (string siteName in orderedList)
            {
                collection.Add(GetSite(siteName));
            }

            return collection;
            
        }
    }
}
