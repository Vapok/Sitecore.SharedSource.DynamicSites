using Sitecore.Data.Items;
using Sitecore.Foundation.Multisite.DynamicSites.Fields.LinkTypes;
using Sitecore.Foundation.Multisite.DynamicSites.Fields.ListTypes;
using Sitecore.Foundation.Multisite.DynamicSites.Fields.SimpleTypes;

namespace Sitecore.Foundation.Multisite.DynamicSites.Items.BaseTemplates
{
internal class DynamicSiteDefinitionBaseItem : CustomItem
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


public CustomCheckboxField SiteEnabled
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["Site Enabled"]);
	}
}


public CustomTextField Hostname
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Hostname"]);
	}
}


//Could not find Field Type for Properties


public CustomLookupField HomeItem
{
	get
	{
		return new CustomLookupField(InnerItem, InnerItem.Fields["Home Item"]);
	}
}


public CustomDroplistField Language
{
	get
	{
        return new CustomDroplistField(InnerItem, InnerItem.Fields["Language"]);
	}
}


public CustomDroplistField TargetHostName
{
	get
	{
        return new CustomDroplistField(InnerItem, InnerItem.Fields["Target Host Name"]);
	}
}


public CustomTextField Port
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Port"]);
	}
}


public CustomDroplistField DatabaseName
{
	get
	{
        return new CustomDroplistField(InnerItem, InnerItem.Fields["Database"]);
	}
}


public CustomCheckboxField CacheHtml
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["Cache Html"]);
	}
}


public CustomCheckboxField CacheMedia
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["Cache Media"]);
	}
}


public CustomCheckboxField EnableDebugger
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["Enable Debugger"]);
	}
}


public CustomCheckboxField EnableAnalytics
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["Enable Analytics"]);
	}
}


public CustomDroplistField Inherit
{
    get
    {
        return new CustomDroplistField(InnerItem, InnerItem.Fields["Inherit"]);
    }
}

public CustomNameValueListField Properties
{
    get
    {
        return new CustomNameValueListField(InnerItem, InnerItem.Fields["Properties"]);
    }
}


#endregion //Field Instance Methods
}
}