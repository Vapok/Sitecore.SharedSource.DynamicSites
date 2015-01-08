using System.Collections.Specialized;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using StringDictionary = Sitecore.Collections.StringDictionary;

namespace Sitecore.SharedSource.DynamicSites.Fields.ListTypes
{
    internal class CustomNameValueListField : BaseCustomField<NameValueListField>
    {
        public CustomNameValueListField(Item item, NameValueListField field) : base(item, field)
        {
        }

        public static implicit operator NameValueCollection(CustomNameValueListField nameValueListField)
        {
            return nameValueListField.NameValues;
        }

        public NameValueCollection NameValues
        {
            get
            {
                if (field == null) return new NameValueCollection();
                if (item.Fields[field.InnerField.Name] == null) return new NameValueCollection();
                return ((NameValueListField)item.Fields[field.InnerField.Name]).NameValues;
            }
        }

        public StringDictionary ToStringDictionary
        {
            get
            {
                if (field == null) return new StringDictionary();
                if (item.Fields[field.InnerField.Name] == null) return new StringDictionary();

                var dictionary = new StringDictionary();
                foreach (var key in NameValues.AllKeys)
                {
                    dictionary.Add(key,NameValues[key]);
                }
                return dictionary;
            }
        }

    }
}
