using System.Collections.Generic;
using Sitecore.Caching;
using Sitecore.Data.Items;
using Sitecore.Reflection;
using Sitecore.SharedSource.DynamicSites.Utilities;

namespace Sitecore.SharedSource.DynamicSites.Caching
{
    internal class SiteCache : CustomCache
    {
        public SiteCache(long maxSize) : base(DynamicSiteSettings.CacheKey, maxSize)
        {
        }
    }

    internal class SiteCacheItem : Dictionary<string, ISet<Item>>, ICacheable
    {
        public SiteCacheItem(Dictionary<string, ISet<Item>> data)
            : base(data)
        {
        }

        public long GetDataLength()
        {
            return TypeUtil.SizeOfDictionary();
        }

        private bool _cacheable = true;
        public bool Cacheable
        {
            get { return _cacheable; }
            set { _cacheable = value; }
        }

        public bool Immutable
        {
            get { return true; }
        }

        public event DataLengthChangedDelegate DataLengthChanged;
    }
}
