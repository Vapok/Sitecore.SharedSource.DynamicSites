using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.SharedSource.DynamicSites.Fields.SimpleTypes
{
	internal partial class CustomTextField : BaseCustomField<TextField>
	{
		public CustomTextField(Item item, TextField field)
			: base(item, field)
		{
		}

		public static implicit operator string(CustomTextField textField)
		{
			return ((textField != null) ? textField.Text : null);
		}

		public string Text
		{
			get { return Raw; }
		}
	}
}
