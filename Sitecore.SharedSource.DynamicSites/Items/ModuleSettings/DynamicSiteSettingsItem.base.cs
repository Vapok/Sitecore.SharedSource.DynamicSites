using Sitecore.Data.Items;
using Sitecore.SharedSource.DynamicSites.Fields.LinkTypes;
using Sitecore.SharedSource.DynamicSites.Fields.SimpleTypes;

namespace Sitecore.SharedSource.DynamicSites.Items.ModuleSettings
{
internal partial class DynamicSiteSettingsItem : CustomItem
{

public static readonly string TemplateId = "{FAA42375-D5C7-4057-A386-80E396078B36}";


#region Boilerplate CustomItem Code

public DynamicSiteSettingsItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator DynamicSiteSettingsItem(Item innerItem)
{
	return innerItem != null ? new DynamicSiteSettingsItem(innerItem) : null;
}

public static implicit operator Item(DynamicSiteSettingsItem customItem)
{
	return customItem != null ? customItem.InnerItem : null;
}

#endregion //Boilerplate CustomItem Code


#region Field Instance Methods


public CustomTextField DefaultHostname
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Default Hostname"]);
	}
}


public CustomLookupField SitesFolder
{
	get
	{
		return new CustomLookupField(InnerItem, InnerItem.Fields["Sites Folder"]);
	}
}


public CustomLookupField DefaultStartItem
{
	get
	{
		return new CustomLookupField(InnerItem, InnerItem.Fields["Default Start Item"]);
	}
}


public CustomLookupField SiteDefinitionTemplate
{
	get
	{
		return new CustomLookupField(InnerItem, InnerItem.Fields["Site Definition Template"]);
	}
}


#endregion //Field Instance Methods
}
}