﻿using System;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.SharedSource.DynamicSites.Items.ModuleSettings;
using Sitecore.SharedSource.DynamicSites.Utilities;
using Sitecore.StringExtensions;

namespace Sitecore.SharedSource.DynamicSites.Events
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
        /// TODO: Add Rename Event

        [UsedImplicitly]
        internal void OnItemDeleted(object sender, EventArgs args)
        {
            var arguments = args as ItemDeletedEventArgs;
            if (arguments == null) return;
            if (arguments.Item == null) return;

            //Reset Caches
            ResetDynamicSitesCache(arguments.Item);
        }

        [UsedImplicitly]
        internal void OnItemRenamed(object sender, EventArgs args)
        {
            var arguments = args as ItemRenamedEventArgs;
            if (arguments == null) return;
            if (arguments.Item == null) return;

            //Reset Caches
            ResetDynamicSitesCache(arguments.Item);
        }

        [UsedImplicitly]
        internal void OnItemSaved(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            var obj = Event.ExtractParameter(args, 0) as Item;
            if (obj == null) return;

            //Is Module Disabled at the config level?
            if (DynamicSiteSettings.Disabled) return;
            
            //Reset Cache if Item is Dynamic Site.
            ResetDynamicSitesCache(obj);
            
            //If Item being saved is the Dynamic Site Settings Item, Make Updates
            //Return otherwise.
            if (obj.TemplateID != new ID(DynamicSiteSettingsItem.TemplateId)) return;

            //Get ItemChanges
            var itemChanges = Event.ExtractParameter(args, 1) as ItemChanges;

            //Do Base Template Updates to Activate Dynamic Sites
            DoBaseTemplateUpdates(obj, itemChanges);
        }

        private void DoBaseTemplateUpdates([NotNull] Item item, ItemChanges itemChanges)
        {
            var siteSettings = (DynamicSiteSettingsItem)item;

            if (itemChanges == null || !itemChanges.IsFieldModified(siteSettings.SiteDefinitionTemplate.Field.InnerField.ID))
                return;

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
            if (DynamicSiteManager.HasBaseTemplate(item))
            {
                DynamicSiteManager.ClearCache();
            }
        }
    }
}