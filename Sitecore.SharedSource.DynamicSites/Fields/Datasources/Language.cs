using System.Collections.Generic;
using System.Linq;
using Sitecore.Buckets.FieldTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.SharedSource.DynamicSites.Pipelines.GetLookupSourceValueLists;
using Sitecore.SharedSource.DynamicSites.Utilities;

namespace Sitecore.SharedSource.DynamicSites.Fields.Datasources
{
    [UsedImplicitly]
    public class Language : IDataSource, IValueList
    {
        public Item[] ListQuery(Item item)
        {
            var langs = LanguageManager.GetLanguages(DynamicSiteSettings.GetCurrentDatabase);
            var itemList = new List<Item>();
            foreach (var lang in langs)
            {
                var langId = LanguageManager.GetLanguageItemId(lang, DynamicSiteSettings.GetCurrentDatabase);
                var langItem = ItemManager.GetItem(langId,lang,Version.Latest,DynamicSiteSettings.GetCurrentDatabase);
                if (langItem != null)
                    itemList.Add(langItem);
            }
            return itemList.ToArray();
        }

        public string[] ValueListQuery()
        {
            var langs = LanguageManager.GetLanguages(DynamicSiteSettings.GetCurrentDatabase);
            return langs.Select(lang => lang.Name).ToArray();
        }
    }
}
