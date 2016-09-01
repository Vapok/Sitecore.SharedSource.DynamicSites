using System.Linq;
using Sitecore.SharedSource.DynamicSites.Sites;
using Sitecore.SharedSource.ValueListField.Pipelines.GetLookupSourceValueLists;
using Sitecore.Sites;

namespace Sitecore.SharedSource.DynamicSites.Fields.Datasources
{
    [UsedImplicitly]
    internal class Inherits : IValueList
    {
        public string[] ValueListQuery()
        {
            return SiteManager.GetSites().Select(site => site.Name).ToArray();
        }
    }
}