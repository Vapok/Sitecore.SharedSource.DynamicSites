using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.SharedSource.DynamicSites.Fields.ListTypes
{
	internal class CustomDroplistField : BaseCustomField<ValueLookupField>
	{
        public CustomDroplistField(Item item, ValueLookupField field)
			: base(item, field)
		{
		}

	    public string Value
	    {
	        get
	        {
                if (field == null) return string.Empty;
                return item.Fields[field.InnerField.Name] == null ? string.Empty : field.Value;
	        }
	    }



	}
}
