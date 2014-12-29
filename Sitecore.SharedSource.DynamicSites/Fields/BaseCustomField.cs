using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.SharedSource.DynamicSites.Fields
{
    /// <summary>
    /// Borrowed from the Custom Item Generator package written originally by Gabe Boys
    /// Custom Item and Custom Field Code extracted andreduced for purposes of this module.
    /// </summary>
    internal abstract class BaseCustomField<T> where T : CustomField
    {
        protected T field;
        protected Item item;

        protected BaseCustomField(Item item, T field)
        {
            this.field = field;
            this.item = item;
        }

        public string Raw
        {
            get
            {
                if (field == null) return string.Empty;
                return field.Value;
            }
        }

        public T Field
        {
            get
            {
                return field;
            }
        }

    }
}
