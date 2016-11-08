using System;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Foundation.Multisite.DynamicSites.Items.ModuleSettings;
using Sitecore.Foundation.Multisite.DynamicSites.Utilities;
using Sitecore.StringExtensions;

namespace Sitecore.Foundation.Multisite.DynamicSites.Events
{
    public class ItemHandler
    {
        /// <summary>
        /// Called when the Dynamic Sites Settings item has been saved.
        /// - Purpose is to apply base template to user preferenced template.
        /// </summary>
        /// <param name="sender">The sender.
        ///             </param><param name="args">The arguments.
        ///             </param>
        /// 

        [UsedImplicitly]
        internal void OnItemDeleted(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");

            var sitecoreArgs = args as SitecoreEventArgs;
            if (sitecoreArgs == null) return;

            var arguments = new ItemDeletedEventArgs((Item)sitecoreArgs.Parameters[0], (ID)sitecoreArgs.Parameters[1]);
            
            if (arguments.Item == null) return;

            try
            {
                //Reset Caches
                ResetDynamicSitesCache(arguments.Item);
            }
            catch (NullReferenceException)
            {
                //Do nothing. 
            }
            catch (Exception e)
            {
                Log.Error($"[DynamicSites] Error: {e.Message} \r\n Stack: {e.StackTrace}", e);
            }
        }

        [UsedImplicitly]
        internal void OnItemRenamed(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");

            var sitecoreArgs = args as SitecoreEventArgs;
            if (sitecoreArgs == null) return;

            var arguments = new ItemRenamedEventArgs((Item)sitecoreArgs.Parameters[0], (string) sitecoreArgs.Parameters[1]);

            if (arguments.Item == null) return;

            if (arguments.Item.Name.Equals(arguments.OldName)) return;

            try
            {
                //Reset Caches
                ResetDynamicSitesCache(arguments.Item);
            }
            catch (NullReferenceException)
            {
                //Do nothing. 
            }
            catch (Exception e)
            {
                Log.Error($"[DynamicSites] Error: {e.Message} \r\n Stack: {e.StackTrace}", e);
            }
        }

        [UsedImplicitly]
        internal void OnItemSaved(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");

            var sitecoreArgs = args as SitecoreEventArgs;
            if (sitecoreArgs == null) return;

            var arguments = new ItemSavedEventArgs((Item)sitecoreArgs.Parameters[0], (ItemChanges)sitecoreArgs.Parameters[1]);
            
            var item = arguments.Item;
            if (item == null) return;

            //Is Module Disabled at the config level?
            if (DynamicSiteSettings.Disabled) return;

            try
            {
                //Skip if current database doesn't keep content items.
                if (!Context.Database.HasContentItem) return;

                //Reset Cache if Item is Dynamic Site.
                ResetDynamicSitesCache(item);

                //If Item being saved is the Dynamic Site Settings Item, Make Updates
                //Return otherwise.
                if (item.TemplateID != new ID(DynamicSiteSettingsItem.TemplateId)) return;

                //Get ItemChanges
                var itemChanges = arguments.Changes;

                //Do Base Template Updates to Activate Dynamic Sites
                DoBaseTemplateUpdates(item, itemChanges);
            }
            catch (NullReferenceException)
            {
                //Do nothing. 
            }
            catch (Exception e)
            {
                Log.Error($"[DynamicSites] Error: {e.Message} \r\n Stack: {e.StackTrace}",e);
            }
        }

        private static void DoBaseTemplateUpdates([NotNull] Item item, ItemChanges itemChanges)
        {
            var siteSettings = (DynamicSiteSettingsItem)item;

            if (itemChanges == null ||
                !itemChanges.IsFieldModified(siteSettings.SiteDefinitionTemplate.Field.InnerField.ID))
            {
                DynamicSiteManager.PublishItemChanges(item);
                return;
            }

            var changedField = itemChanges.FieldChanges[siteSettings.SiteDefinitionTemplate.Field.InnerField.ID];
            var oldValue = changedField.OriginalValue;
            var newValue = changedField.Value;

            if (oldValue.Equals(newValue)) return;

            if (newValue.IsNullOrEmpty())
            {
                if (item.Fields[changedField.FieldID].ContainsStandardValue)
                {
                    newValue = item.Fields[changedField.FieldID].GetStandardValue();
                }
            }

            if (!newValue.IsNullOrEmpty())
            {
                var newTemplateItem = DynamicSiteSettings.GetCurrentDatabase.GetTemplate(new ID(newValue));

                if (newTemplateItem != null)
                    DynamicSiteManager.AddBaseTemplate(newTemplateItem);
            }

            if (oldValue.IsNullOrEmpty()) return;
            var oldTemplateItem = DynamicSiteSettings.GetCurrentDatabase.GetTemplate(new ID(oldValue));

            if (oldTemplateItem != null)
                DynamicSiteManager.RemoveBaseTemplate(oldTemplateItem);
        }

        private void ResetDynamicSitesCache([NotNull] Item item)
        {
            //Is Module Disabled at the config level?
            if (DynamicSiteSettings.Disabled) return;

            //If Item being deleted is a Dynamic Site, clear the Dynamic Site cache.
            if (!DynamicSiteManager.HasBaseTemplate(item)) return;

            DynamicSiteManager.PublishItemChanges(item);
            DynamicSiteManager.ClearCache();
        }
    }
}
