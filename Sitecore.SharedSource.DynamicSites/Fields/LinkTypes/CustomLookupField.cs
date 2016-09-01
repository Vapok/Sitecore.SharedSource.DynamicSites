using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.SharedSource.DynamicSites.Fields.LinkTypes
{
    /// <summary>
    /// Borrowed from the Custom Item Generator package written originally by Gabe Boys
    /// Custom Item and Custom Field Code extracted andreduced for purposes of this module.
    /// </summary>
	internal class CustomLookupField : BaseCustomField<LookupField>
	{
		internal CustomLookupField(Item item, LookupField field) : base(item, field)
		{
		}

		public static implicit operator Item(CustomLookupField lookupField)
		{
			return lookupField.Item;
		}

		public Item Item => field?.TargetItem;
	}
}
