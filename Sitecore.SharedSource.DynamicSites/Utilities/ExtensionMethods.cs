using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sitecore.Sites;

namespace Sitecore.SharedSource.DynamicSites.Utilities
{
    public static class ExtensionMethods
    {
        public static bool ContainsKeyInList(this List<KeyValuePair<string, Site>> siteList, string keyName)
        {
            return siteList.Any(keyValuePair => keyValuePair.Key.Equals(keyName));
        }

        public static void AddSitePair(this List<KeyValuePair<string, Site>> siteList, string keyName, Site site)
        {
            var newPair = new KeyValuePair<string, Site>(keyName, site);
            siteList.Add(newPair);
        }

        public static Site GetSiteByKey(this List<KeyValuePair<string, Site>> siteList, string keyName)
        {
            return (from keyValuePair in siteList where keyValuePair.Key.Equals(keyName) select keyValuePair.Value).FirstOrDefault();
        }
    }
}
