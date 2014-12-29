using System;
using Sitecore.Data;
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
        [UsedImplicitly]
        internal void OnItemSaved(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");

            //Is Module Disabled at the config level?
            if (DynamicSiteSettings.Disabled) return;
            
            var obj = Event.ExtractParameter(args, 0) as Item;

            //If Item being saved is a Dynamic Site, clear the Dynamic Site cache.
            if (obj != null && DynamicSiteManager.HasBaseTemplate(obj))
            {
                DynamicSiteSettings.ClearCache();
            }

            //If Item being saved is the Dynamic Site Settings Item, Make Updates
            //Return otherwise.
            if (obj == null || obj.TemplateID != new ID(DynamicSiteSettingsItem.TemplateId)) return;

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
    }
}
