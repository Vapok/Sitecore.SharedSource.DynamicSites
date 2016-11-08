using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Foundation.Multisite.DynamicSites.Caching;
using Sitecore.Foundation.Multisite.DynamicSites.Items.BaseTemplates;
using Sitecore.Foundation.Multisite.DynamicSites.Items.ModuleSettings;

namespace Sitecore.Foundation.Multisite.DynamicSites.Utilities
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

        private static string MaxCacheSize => Settings.GetSetting(MaxCacheSetting, "50MB");

        public static bool Disabled => Settings.GetBoolSetting(DisabledSetting, false);

        public static bool AutoPublish => Settings.GetBoolSetting(AutoPublishSetting, true);

        public static string SiteName => Settings.GetSetting(SiteSetting, DefaultSitename);

        private static string SettingsItemPath => Settings.GetSetting(ItemPathSetting, DefaultSettingsItemPath);

        public static bool IsInitialized => DynamicSiteManager.SettingsInitialized();

        public static string CacheKey => CacheKeySetting;

        public static DynamicSiteSettingsItem GetSettingsItem => GetCurrentDatabase.GetItem(SettingsItemPath);


        public static TemplateItem BaseSiteDefinitionTemplateItem => GetCurrentDatabase.GetItem(new ID(DynamicSiteDefinitionBaseItem.TemplateId));

        public static Item SitesFolder => GetSettingsItem?.SitesFolder.Item;

        public static Database GetCurrentDatabase => Context.ContentDatabase ?? Context.Database ?? Database.GetDatabase("master");

        public static SiteCache GetSiteCache => new SiteCache(StringUtil.ParseSizeString(MaxCacheSize));
    }
}
