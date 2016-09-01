using System;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.SharedSource.DynamicSites.Events
{
    //There's a bug in Sitecore 7.5 where the ItemRenamedEventArgs do not inherit from EventArgs.
    //Makes it unable to cast class.
    public class ItemRenamedEventArgs : EventArgs
    {
        public Item Item { get; }

        public string OldName { get; }

        public ItemRenamedEventArgs(Item item, string oldName)
        {
            Error.AssertObject(item, "item");
            Error.AssertString(oldName, "oldName", false);
            Item = item;
            OldName = oldName;
        }
    }
}
