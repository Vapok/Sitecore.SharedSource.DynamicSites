using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Foundation.Multisite.DynamicSites.Fields.SimpleTypes
{
	internal class CustomCheckboxField : BaseCustomField<CheckboxField>
	{
		public CustomCheckboxField(Item item, CheckboxField field)
			: base(item, field)
		{
		}

		public static implicit operator bool(CustomCheckboxField dateField)
		{
			return dateField.Checked;
		}

		public bool Checked
		{
			get
			{
				if (field == null) return false;
				if (item.Fields[field.InnerField.Name] == null) return false;
				return ((CheckboxField)item.Fields[field.InnerField.Name]).Checked;
			}
		}
	}
}
