using Sitecore.Caching;
using Sitecore.Reflection;
using Sitecore.Sites;

namespace Sitecore.SharedSource.DynamicSites.Caching
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
