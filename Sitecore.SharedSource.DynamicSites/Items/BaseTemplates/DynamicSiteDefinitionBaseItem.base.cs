using Sitecore.Data.Items;
using Sitecore.SharedSource.DynamicSites.Fields.LinkTypes;
using Sitecore.SharedSource.DynamicSites.Fields.SimpleTypes;

namespace Sitecore.SharedSource.DynamicSites.Items.BaseTemplates
{
internal partial class DynamicSiteDefinitionBaseItem : CustomItem
{

public static readonly string TemplateId = "{6A5CCD86-C5E0-45DE-BA64-4296481F2DE3}";


#region Boilerplate CustomItem Code

public DynamicSiteDefinitionBaseItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator DynamicSiteDefinitionBaseItem(Item innerItem)
{
	return innerItem != null ? new DynamicSiteDefinitionBaseItem(innerItem) : null;
}

public static implicit operator Item(DynamicSiteDefinitionBaseItem customItem)
{
	return customItem != null ? customItem.InnerItem : null;
}

#endregion //Boilerplate CustomItem Code


#region Field Instance Methods


public CustomTextField Hostname
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Hostname"]);
	}
}


public CustomLookupField HomeItem
{
	get
	{
		return new CustomLookupField(InnerItem, InnerItem.Fields["Home Item"]);
	}
}


#endregion //Field Instance Methods
}
}