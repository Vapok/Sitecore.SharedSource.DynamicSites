using System;
using System.Collections.Generic;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using Sitecore.SharedSource.DynamicSites.Items.BaseTemplates;
using Sitecore.Sites;
using Sitecore.StringExtensions;
using Sitecore.Web;

namespace Sitecore.SharedSource.DynamicSites.Utilities
{
    internal static class DynamicSiteManager
    {
        public static void AddBaseTemplate([NotNull] TemplateItem item)
        {
            var template = DynamicSiteSettings.BaseSiteDefinitionTemplateItem;
            
            //Do Nothing If Template Is Self
            if (template.ID.Equals(item.ID)) return;

            //Get List of Existing Base Templates
            var existingTemplates = GetBaseTemplates(item);
            
            //Double check Base Template doesn't already exist.
            if (HasBaseTemplate(item)) return;

            //Add Template
            existingTemplates.Add(template.ID.ToString());

            //Save Item
            SaveItemBaseTemplates(item, existingTemplates);

            //Publish Changes
            PublishItemChanges(item);
        }

        public static void RemoveBaseTemplate([NotNull] TemplateItem item)
        {

            var template = DynamicSiteSettings.BaseSiteDefinitionTemplateItem;

            //Do Nothing If Template Is Self
            if (template.ID.Equals(item.ID)) return;

            //Get List of Existing Base Templates
            var existingTemplates = GetBaseTemplates(item);

            //Double check Base Template exists.
            if (!HasBaseTemplate(item)) return;
            
            //Add Template
            existingTemplates.Remove(template.ID.ToString());
                
            //Save Item
            SaveItemBaseTemplates(item,existingTemplates);

            //Publish Changes
            PublishItemChanges(item);
        }

        public static void PublishItemChanges(Item item)
        {
            if (!DynamicSiteSettings.AutoPublish) return;

            var targets = GetPublishingTargets();

            if (targets.Length == 0) return;

            var languages = LanguageManager.GetLanguages(Context.ContentDatabase);
            if (languages == null || languages.Count == 0) return;

            Log.Audit(string.Format("Publish item now: {0}", AuditFormatter.FormatItem(item)),typeof(DynamicSiteManager));
            PublishManager.PublishItem(item, targets, languages.ToArray(), false, true);
        }

        public static bool HasBaseTemplate([NotNull] Item item)
        {
            var template = DynamicSiteSettings.BaseSiteDefinitionTemplateItem;

            //Check to see if Item is made of Template we want, else dig further.
            return item.TemplateID.Equals(template.ID) || HasBaseTemplate(item.Template);
        }

        private static bool HasBaseTemplate([NotNull] TemplateItem item)
        {
            var template = DynamicSiteSettings.BaseSiteDefinitionTemplateItem;
            
            //Get List of Existing Base Templates
            var existingTemplates = new List<string>(item.InnerItem[FieldIDs.BaseTemplate].Split('|'));
            return existingTemplates.Contains(template.ID.ToString());
        }

        private static List<string> GetBaseTemplates([NotNull] BaseItem item)
        {
            return new List<string>(item[FieldIDs.BaseTemplate].Split('|'));
        }

        private static void SaveItemBaseTemplates([NotNull] Item item, [NotNull] IEnumerable<string> baseTemplates)
        {
            try
            {
                using (new SecurityDisabler())
                {
                    item.Editing.BeginEdit();
                    item[FieldIDs.BaseTemplate] = string.Join("|", baseTemplates);
                    item.Editing.EndEdit();
                }

            }
            catch (Exception e)
            {
                Log.Error(String.Format("Error Saving Item Base Template for Dynamic Site: {0}\r\n{1}", e.Message, e.StackTrace), e);
            }
        }

        public static bool SettingsInitialized()
        {
            var settingsItem = DynamicSiteSettings.GetSettingsItem;
            if (settingsItem == null) return false;

            if (settingsItem.SiteDefinitionTemplate.Item == null) return false;
            if (settingsItem.SitesFolder.Item == null) return false;

            return true;
        }

        public static void InitializeSettings()
        {
            if (SettingsInitialized())
                return;
            try
            {
                var settingsItem = DynamicSiteSettings.GetSettingsItem;
                if (settingsItem == null) return;

                var defaultItem = SiteManager.GetSite("website");
                string rootPath;
                string defaultPath;

                if (defaultItem == null)
                {
                    //This is a fail safe, so that if the website definition has been removed
                    //we can fall back to something.
                    rootPath = "/sitecore/content";
                    defaultPath = rootPath + "/Home";
                }
                else
                {
                    rootPath = defaultItem.Properties["rootPath"];
                    defaultPath = rootPath + defaultItem.Properties["startItem"];
                }

                if (defaultPath.IsNullOrEmpty() || rootPath.IsNullOrEmpty()) return;

                var siteFolderItem = DynamicSiteSettings.GetCurrentDatabase.GetItem(rootPath);
                var siteItem = DynamicSiteSettings.GetCurrentDatabase.GetItem(defaultPath);
                if (siteFolderItem == null || siteItem == null) return;

                try
                {
                    using (new SecurityDisabler())
                    {
                        settingsItem.InnerItem.Editing.BeginEdit();
                        settingsItem.InnerItem[settingsItem.SitesFolder.Field.InnerField.ID] = siteFolderItem.ID.ToString();
                        settingsItem.InnerItem[settingsItem.SiteDefinitionTemplate.Field.InnerField.ID] = siteItem.TemplateID.ToString();
                        settingsItem.InnerItem.Editing.EndEdit();
                    }

                    //Publish Changes
                    PublishItemChanges(settingsItem.InnerItem);

                }
                catch (Exception e)
                {
                    Log.Error(String.Format("Error Saving Initial Item State: {0}\r\n{1}", e.Message, e.StackTrace), e);
                }
            }
            catch (Exception e)
            {
                Log.Error(String.Format("Error Initializing Dynamic Sites: {0}\r\n{1}", e.Message, e.StackTrace), e);
            }
        }

        private static Site CreateSite([NotNull] DynamicSiteDefinitionBaseItem item, [NotNull] Site defaultSite)
        {
            try
            {
                if (item.HomeItem.Item == null) return null;
                var properties = new StringDictionary(defaultSite.Properties);

                //Required Properties
                properties["mode"] = item.SiteEnabled ? "on" : "off";
                properties["name"] = item.Name;
                properties["hostName"] = item.Hostname.Text;
                properties["startItem"] = string.Format("/{0}", item.HomeItem.Item.Name);
                properties["rootPath"] = item.HomeItem.Item.Parent.Paths.FullPath;
                
                //Enhanced Properties
                if (!item.Language.Value.IsNullOrEmpty())
                    properties["language"] = item.Language.Value;

                if (!item.TargetHostName.Value.IsNullOrEmpty())
                    properties["targetHostName"] = item.TargetHostName.Value;

                if (!item.Port.Text.IsNullOrEmpty())
                    properties["port"] = item.Port.Text;

                if (!item.DatabaseName.Value.IsNullOrEmpty())
                    properties["database"] = item.DatabaseName.Value;

                if (!item.Inherit.Value.IsNullOrEmpty())
                    properties["inherits"] = item.Inherit.Value;

                properties["cacheHtml"] = item.CacheHtml.Checked.ToString();
                properties["cacheMedia"] = item.CacheMedia.Checked.ToString();
                properties["enableDebugger"] = item.EnableDebugger.Checked.ToString();
                properties["enableAnalytics"] = item.EnableAnalytics.Checked.ToString();
                
                //Custom Properties
                if (item.Properties.ToStringDictionary.Count> 0)
                    properties.AddRange(item.Properties.ToStringDictionary);
                
                var newSite = new Site(item.Name, properties);
                return newSite;
            }
            catch (Exception e)
            {
                Log.Error(String.Format("Error Creating Dynamic Site Definition: {0}\r\n{1}",e.Message,e.StackTrace),e);
                return null;
            }

        }

        public static SafeDictionary<string, Site> GetDynamicSitesDictionary(Site defaultSite)
        {
            Assert.ArgumentNotNull(defaultSite, "defaultSite");
            var sites = new SafeDictionary<string, Site>();

            var siteRoot = DynamicSiteSettings.SitesFolder;
            if (siteRoot == null) return sites;
            try
            {
                foreach (Item dynamicSite in siteRoot.Children)
                {
                    try
                    {
                        if (!HasBaseTemplate(dynamicSite)) continue;

                        var dynamicSiteItem = (DynamicSiteDefinitionBaseItem)dynamicSite;

                        if (dynamicSiteItem == null || dynamicSiteItem.HomeItem == null)
                        {
                            continue;
                        }
                        var newSite = CreateSite(dynamicSiteItem, defaultSite);

                        if (newSite == null) continue;

                        if (sites.ContainsKey(newSite.Name)) continue;
                        
                        var info = new SiteInfo(newSite.Properties);
                        if (info.IsActive)
                            sites.Add(newSite.Name, newSite);
                    }
                    catch (Exception e)
                    {
                        Log.Error(String.Format("Error Fetching Dynamic Sites: {0}\r\n{1}", e.Message, e.StackTrace), e);
                    }

                }
            }
            catch (Exception e)
            {
                Log.Error(String.Format("Error in Dynamic Sites: {0}\r\n{1}", e.Message, e.StackTrace), e);
            }

            return sites;
        }

        public static void ClearCache()
        {
            if (DynamicSiteSettings.GetSiteCache.Count() > 0)
                DynamicSiteSettings.GetSiteCache.Clear();
        }

        public static ItemCollection GetHomeItems()
        {
            var collection = new ItemCollection();

            foreach (var site in DynamicSiteSettings.GetSiteCache.GetAllSites())
            {
                var info = new SiteInfo(site.Properties);
                var context = new SiteContext(info);
                var item = DynamicSiteSettings.GetCurrentDatabase.GetItem(context.StartPath);

                if (item != null)
                    collection.Add(item);
            }

            return collection;
        }

        public static Database[] GetPublishingTargets()
        {
            Item itemNotNull = Client.GetItemNotNull("/sitecore/system/publishing targets");
            var arrayList = new List<Database>();
            foreach (BaseItem baseItem in itemNotNull.Children)
            {
                Database database = Factory.GetDatabase(baseItem["Target database"], false);
                if (database != null)
                    arrayList.Add(database);
            }
            return Assert.ResultNotNull(arrayList.ToArray());
        }

    }
}
