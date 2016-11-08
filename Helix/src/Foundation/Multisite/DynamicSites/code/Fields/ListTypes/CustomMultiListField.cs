using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Foundation.Multisite.DynamicSites.Fields.ListTypes
{
	internal class CustomMultiListField : BaseCustomField<MultilistField>
	{
		public CustomMultiListField(Item item, MultilistField field)
			: base(item, field)
		{
		}

		public static implicit operator List<Item>(CustomMultiListField multilistField)
		{
			return multilistField.ListItems;
		}

	    private List<Item> ListItems
		{
			get
			{
				if (field == null) return new List<Item>();

				return item.Fields[field.InnerField.Name] == null ? new List<Item>() : 
                    ((MultilistField)item.Fields[field.InnerField.Name]).GetItems().ToList();
			}
		}

		/// <summary>
		/// Returns the ID values of the field as a list of strings
		/// </summary>
		public List<string> Ids
		{
			get
			{
				if (field == null)
				{
					return new List<string>();
				}

				if (item.Fields[field.InnerField.Name] == null)
				{
					return new List<string>();
				}

				return string.IsNullOrEmpty(Raw) ? new List<string>() : Raw.Split('|').Where(id => !string.IsNullOrEmpty(id)).ToList();
			}
		}
	}
}
