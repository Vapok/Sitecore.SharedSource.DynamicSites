using System;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.SharedSource.DynamicSites.Events
{
    //There's a bug in Sitecore 7.5 where the ItemRenamedEventArgs do not inherit from EventArgs.
    //Makes it unable to cast class.
    public class ItemRenamedEventArgs : EventArgs
    {
        private Item m_item;
        private string m_oldName;

        public Item Item
        {
            get
            {
                return m_item;
            }
        }

        public string OldName
        {
            get
            {
                return m_oldName;
            }
        }

        public ItemRenamedEventArgs(Item item, string oldName)
        {
            Error.AssertObject(item, "item");
            Error.AssertString(oldName, "oldName", false);
            m_item = item;
            m_oldName = oldName;
        }
    }
}
