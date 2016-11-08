using Sitecore.Diagnostics;
using Sitecore.Foundation.Multisite.DynamicSites.Utilities;
using Sitecore.Pipelines;

namespace Sitecore.Foundation.Multisite.DynamicSites.Pipelines.Loader
{
    [UsedImplicitly]
    public class InitializeDynamicSites
    {
        /// <summary>
        /// Runs the processor.
        /// 
        /// </summary>
        /// <param name="args">The arguments.</param>
        [UsedImplicitly]
        public void Process(PipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            //Is Module Disabled at the config level?
            if (DynamicSiteSettings.Disabled) return;

            //Is Module Initialized, if not, Initialize. Exit on failure.
            if (!DynamicSiteSettings.IsInitialized)
                DynamicSiteManager.InitializeSettings();
        }

    }
}
