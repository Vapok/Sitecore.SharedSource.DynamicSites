using System.Linq;
using Sitecore.Buckets.FieldTypes;
using Sitecore.Data.Items;
using Sitecore.SharedSource.DynamicSites.Pipelines.GetLookupSourceValueLists;
using Sitecore.SharedSource.DynamicSites.Utilities;
using Sitecore.Sites;

namespace Sitecore.SharedSource.DynamicSites.Fields.Datasources
{
    [UsedImplicitly]
    internal class Inherits : IDataSource, IValueList
    {
        public Item[] ListQuery(Item item)
        {
            return DynamicSiteManager.GetHomeItems().ToArray();
        }

        public string[] ValueListQuery()
        {
            return SiteManager.Providers["dynamic"].GetSites().Select(site => site.Name).ToArray();
        }
    }
}
