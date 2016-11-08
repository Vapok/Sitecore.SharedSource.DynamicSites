using System.Linq;
using Sitecore.Data.Managers;
using Sitecore.Foundation.Multisite.DynamicSites.Utilities;
using Sitecore.SharedSource.ValueListField.Pipelines.GetLookupSourceValueLists;

namespace Sitecore.Foundation.Multisite.DynamicSites.Fields.Datasources
{
    [UsedImplicitly]
    public class Language : IValueList
    {
        public string[] ValueListQuery()
        {
            var langs = LanguageManager.GetLanguages(DynamicSiteSettings.GetCurrentDatabase);
            return langs.Select(lang => lang.Name).ToArray();
        }
    }
}