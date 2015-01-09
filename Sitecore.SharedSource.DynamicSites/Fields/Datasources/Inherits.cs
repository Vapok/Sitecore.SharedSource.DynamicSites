using System.Linq;
using Sitecore.SharedSource.ValueListField.Pipelines.GetLookupSourceValueLists;
using Sitecore.Sites;

namespace Sitecore.SharedSource.DynamicSites.Fields.Datasources
{
    [UsedImplicitly]
    internal class Inherits : IValueList
    {
        public string[] ValueListQuery()
        {
            return SiteManager.Providers["dynamic"].GetSites().Select(site => site.Name).ToArray();
        }
    }
}