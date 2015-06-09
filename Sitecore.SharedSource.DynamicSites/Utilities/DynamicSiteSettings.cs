using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SharedSource.DynamicSites.Caching;
using Sitecore.SharedSource.DynamicSites.Items.BaseTemplates;
using Sitecore.SharedSource.DynamicSites.Items.ModuleSettings;

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
        private const string AutoPublishSetting = "DynamicSites.AutoPublish";

        private static string MaxCacheSize
        {
            get { return Settings.GetSetting(MaxCacheSetting, "50MB"); }
        }

        public static bool Disabled
        {
            get { return Settings.GetBoolSetting(DisabledSetting, false); }
        }

        public static bool AutoPublish
        {
            get { return Settings.GetBoolSetting(AutoPublishSetting, true); }
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


        public static TemplateItem BaseSiteDefinitionTemplateItem
        {
            get { return GetCurrentDatabase.GetItem(new ID(DynamicSiteDefinitionBaseItem.TemplateId)); }
        }

        public static Item SitesFolder
        {
            get
            {
                return GetSettingsItem == null ? null : GetSettingsItem.SitesFolder.Item;
            }
        }

        public static Database GetCurrentDatabase
        {
            get
            {
                return Context.ContentDatabase ?? Context.Database ?? Database.GetDatabase("master");
            }
        }

        public static SiteCache GetSiteCache
        {
            get { return new SiteCache(StringUtil.ParseSizeString(MaxCacheSize)); }
        }
    }
}
