using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HasPresentation;
using Sitecore.Publishing;
using Sitecore.SharedSource.DynamicSites.Utilities;
using Sitecore.Shell.DeviceSimulation;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Sites;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.SharedSource.DynamicSites.Commands
{
    [UsedImplicitly]
    public class DynamicPreviewItem :   PreviewItem
    {
        /// <summary>
        /// Runs the specified args.
        /// 
        /// </summary>
        /// <param name="args">The arguments.</param>
        [UsedImplicitly]
        protected new void Run(ClientPipelineArgs args)
        {
            var contentItem = Database.GetItem(ItemUri.Parse(args.Parameters["uri"]));
            if (contentItem == null)
            {
                SheerResponse.Alert("Item not found.");
            }
            else
            {
                var itemId = contentItem.ID.ToString();
                if (args.IsPostBack)
                {
                    if (args.Result != "yes")
                        return;
                    var startItem = Context.ContentDatabase.GetItem(Context.Site.StartPath);
                    if (startItem == null)
                    {
                        SheerResponse.Alert("Start item not found.");
                        return;
                    }
                    if (!HasPresentationPipeline.Run(startItem))
                    {
                        SheerResponse.Alert("The start item cannot be previewed because it has no layout for the current device.\n\nPreview cannot be started.");
                        return;
                    }
                    itemId = startItem.ID.ToString();
                }
                else if (!HasPresentationPipeline.Run(contentItem))
                {
                    SheerResponse.Confirm("The current item cannot be previewed because it has no layout for the current device.\n\nDo you want to preview the start Web page instead?");
                    args.WaitForPostBack();
                    return;
                }
                SheerResponse.CheckModified(false);

                //Dynamically Speaking, we want the site of the item as it relates to a home Item.
                //If not found, fallback to default functionalty.
                var site = DynamicSiteManager.GetSiteContextByContentItem(contentItem) ??
                           Factory.GetSite(Settings.Preview.DefaultSite);
                Assert.IsNotNull(site, "Site \"{0}\" not found", Settings.Preview.DefaultSite);
                WebUtil.SetCookieValue(site.GetCookieKey("sc_date"), string.Empty);
                PreviewManager.StoreShellUser(Settings.Preview.AsAnonymous);
                var webSiteUrl = SiteContext.GetWebSiteUrl();
                webSiteUrl["sc_itemid"] = itemId;
                webSiteUrl["sc_mode"] = "preview";
                webSiteUrl["sc_lang"] = contentItem.Language.ToString();
                DeviceSimulationUtil.DeacitvateSimulators();
                SheerResponse.Eval("window.open('" + webSiteUrl + "', '_blank')");
            }
        }

    }
}
