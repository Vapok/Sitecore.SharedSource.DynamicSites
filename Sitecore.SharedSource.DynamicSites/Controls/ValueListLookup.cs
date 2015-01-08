using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System.Web.UI;
using Sitecore.Shell.Applications.ContentEditor;

namespace Sitecore.SharedSource.DynamicSites.Controls
{
    [UsedImplicitly]
    public sealed class ValueListLookup : LookupEx
    {
        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");
            var values = GetValueList();
            
            output.Write("<select" + GetControlAttributes() + ">");
            output.Write("<option value=\"\"></option>");
            var valueExistsInList = false;
            foreach (var value in values)
            {
                if (IsSelected(value))
                    valueExistsInList = true;
                output.Write("<option value=\"" + value + "\"" + (IsSelected(value) ? " selected=\"selected\"" : string.Empty) + ">" + value + "</option>");
            }
            var valueNotInSelection = !string.IsNullOrEmpty(Value) && !valueExistsInList;
            if (valueNotInSelection)
            {
                output.Write("<optgroup label=\"" + Translate.Text("Value not in the selection list.") + "\">");
                output.Write("<option value=\"" + Value + "\" selected=\"selected\">" + Value + "</option>");
                output.Write("</optgroup>");
            }
            output.Write("</select>");

            if (valueNotInSelection)
                output.Write("<div style=\"color:#999999;padding:2px 0px 0px 0px\">{0}</div>", Translate.Text("The field contains a value that is not in the selection list."));
        }

        private bool IsSelected(string value)
        {
            Assert.ArgumentNotNull(value, "value");
            return Value == value;
        }

        private string[] GetValueList()
        {
            return Data.LookupSources.GetValues(Source);
        }
    }
}
