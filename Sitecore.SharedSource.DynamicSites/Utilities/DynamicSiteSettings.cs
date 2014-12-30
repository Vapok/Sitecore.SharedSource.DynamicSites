using System.Collections.Generic;
using System.Linq;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SharedSource.DynamicSites.Caching;
using Sitecore.SharedSource.DynamicSites.Items.BaseTemplates;
using Sitecore.SharedSource.DynamicSites.Items.ModuleSettings;
using Sitecore.StringExtensions;

namespace Sitecore.SharedSource.DynamicSites.Utilities
{
    internal static class DynamicSiteSettings
    {
        private const string SiteSetting = "DynamicSites.SiteName";
        private const string ItemPathSetting = "DynamicSites.SettingsItemPath";
        private const string DisabledSetting = "DynamicSites.Disabled";
        private const string MaxCacheSetting = "DynamicSites.MaxCacheSize";
        private const string DefaultSitename = "dynamicsites";
        private const string DefaultSettingsItemPath = "/sitecore/system/Modules/Dynamic Sites/Dynamic Site Settings";
        private const string CacheKeySetting = "DynamicSites.SiteCache";

        private static readonly SiteCache _siteCache = new SiteCache(StringUtil.ParseSizeString(MaxCacheSize))
        {
            Enabled = true
        };

        public static string MaxCacheSize
        {
            get { return Settings.GetSetting(MaxCacheSetting, "50MB"); }
        }

        public static bool Disabled
        {
            get { return Settings.GetBoolSetting(DisabledSetting, false); }
        }

        public static string SiteName
        {
            get
            {
                return Settings.GetSetting(SiteSetting, DefaultSitename);
            }
        }

        private static string SettingsItemPath
        {
            get
            {
                return Settings.GetSetting(ItemPathSetting, DefaultSettingsItemPath);
            }
        }

        public static bool IsInitialized
        {
            get { return DynamicSiteManager.SettingsInitialized(); }
        }

        public static string CacheKey
        {
            get { return CacheKeySetting; }
        }

        public static DynamicSiteSettingsItem GetSettingsItem
        {
            get
            {
                return GetCurrentDatabase.GetItem(SettingsItemPath);
            }
        }


        private static SafeDictionary<string, ISet<Item>> GetDynamicSites
        {
            get
            {
                var sites = new SafeDictionary<string, ISet<Item>>();

                var siteRoot = SitesFolder;
                if (siteRoot == null) return sites;

                foreach (Item dynamicSite in siteRoot.Children)
                {
                    if (!DynamicSiteManager.HasBaseTemplate(dynamicSite)) continue;

                    var dynamicSiteItem = (DynamicSiteDefinitionBaseItem) dynamicSite;

                    if (dynamicSiteItem == null || dynamicSiteItem.HomeItem == null || dynamicSiteItem.Hostname.Text.IsNullOrEmpty())
                    {
                        continue;
                    }

                    var hostnames = dynamicSiteItem.Hostname.Text.Split('|')
                        .Select(hostname => hostname.Trim())
                        .Where(hostname => !string.IsNullOrEmpty(hostname));

                    foreach (var hostname in hostnames)
                    {
                        if (!sites.ContainsKey(hostname))
                        {
                            sites.Add(hostname, new HashSet<Item>());
                        }
                        sites[hostname].Add(dynamicSiteItem.HomeItem.Item);
                    }
                }

                return sites;

            }
        }

        public static TemplateItem BaseSiteDefinitionTemplateItem
        {
            get { return GetCurrentDatabase.GetItem(new ID(DynamicSiteDefinitionBaseItem.TemplateId)); }
        }
        public static Item SiteDefinitionTemplate
        {
            get
            {
                return GetSettingsItem == null ? null : GetSettingsItem.SiteDefinitionTemplate.Item;
            }
        }

        public static Item SitesFolder
        {
            get
            {
                return GetSettingsItem == null ? null : GetSettingsItem.SitesFolder.Item;
            }
        }

        public static void ClearCache()
        {
            _siteCache.Clear();
        }

        public static Database GetCurrentDatabase
        {
            get
            {
                if (Context.Database == null) return Database.GetDatabase("master");
                return Context.Database.Name.Equals("core") ? Database.GetDatabase("master") : Context.Database;
            }
        }
    }
}
