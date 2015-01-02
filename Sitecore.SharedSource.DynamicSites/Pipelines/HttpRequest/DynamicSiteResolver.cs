
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.SharedSource.DynamicSites.Utilities;

namespace Sitecore.SharedSource.DynamicSites.Pipelines.HttpRequest
{
    /// <summary>
    /// Allows for the creation of Dynamic Sites generated as Sitecore Items
    /// This Processor works in conjunction with the SiteResolver processor.
    /// </summary>
    [UsedImplicitly]
    internal sealed class DynamicSiteResolver : SiteResolver
    {
        /// <summary>
        /// Runs the Dynamic Site Resolver Processor
        /// Located in httpBeginRequest Pipeline.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            
            //Is Module Disabled at the config level?
            if (DynamicSiteSettings.Disabled) return;

            if (Context.Site == null) return;
            if (!Context.Site.Name.Equals(DynamicSiteSettings.SiteName)) return;
            
            //Is Module Initialized, if not, Initialize. Exit on failure.
            if (!DynamicSiteSettings.IsInitialized)
                if (!DynamicSiteManager.InitializeSettings())
                    return;
            
            //Resolve Dynamic Site
            var site = ResolveSiteContext(args);
            //var site = ResolveDynamicSiteContext();
            UpdatePaths(args, site);
            //UpdatePaths(args, site);
            Context.Site = site;
        }
    }
}
