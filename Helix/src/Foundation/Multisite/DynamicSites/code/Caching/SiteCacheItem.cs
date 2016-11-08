using Sitecore.Caching;
using Sitecore.Reflection;
using Sitecore.Sites;

namespace Sitecore.Foundation.Multisite.DynamicSites.Caching
{
    internal class SiteCacheItem : Site, ICacheable
    {
        public SiteCacheItem(Site data)
            : base(data.Name, data.Properties)
        {

        }

        public long GetDataLength()
        {
            return TypeUtil.SizeOfDictionary();
        }

        public bool Cacheable { get; set; } = true;

        public bool Immutable => true;

        public event DataLengthChangedDelegate DataLengthChanged;
    }
}
