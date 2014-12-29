
using System.Collections.Generic;
using System.Linq;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.SharedSource.DynamicSites.Utilities;
using Sitecore.Sites;
using Sitecore.Web;

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
            var site = ResolveDynamicSiteContext();
            UpdatePaths(args, site);
            Context.Site = site;
        }

        /// <summary>
        /// Resolves the Dynamic Site context.
        /// </summary>
        /// <returns/>
        private SiteContext ResolveDynamicSiteContext()
        {
            var requestUri = WebUtil.GetRequestUri();
            var siteContext = GetSiteContext(requestUri.Host);
            return siteContext;
        }

        private SiteContext GetSiteContext(string host)
        {
            ISet<Item> dynamicSiteItems;
            Item dynamicItem = null;

            var sites = DynamicSiteSettings.Sites;
            if (sites == null) return Context.Site;

            if (sites.TryGetValue(host, out dynamicSiteItems))
            {
                dynamicItem = dynamicSiteItems.FirstOrDefault(item => item != null);
            }

            return CreateSiteContext(dynamicItem ?? DynamicSiteSettings.GetSettingsItem.DefaultStartItem);
        }

        private SiteContext CreateSiteContext(Item item)
        {
            var siteInfoDictionary = new StringDictionary();

            foreach (string key in Context.Site.SiteInfo.Properties.Keys)
            {
                siteInfoDictionary.Add(key, Context.Site.SiteInfo.Properties[key]);
            }

            siteInfoDictionary["startItem"] = string.Format("/{0}", item.Name);
            siteInfoDictionary["rootPath"] = item.Parent.Paths.FullPath;

            var newInfo = SiteInfo.Create(siteInfoDictionary);
            var newSite = new SiteContext(newInfo);
            return newSite;
   
        }

    }
}
