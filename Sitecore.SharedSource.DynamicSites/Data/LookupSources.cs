using System;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Pipelines;
using Sitecore.SharedSource.DynamicSites.Pipelines.GetLookupSourceValueLists;

namespace Sitecore.SharedSource.DynamicSites.Data
{
    public static class LookupSources
    {
        /// <summary>
        /// Gets a list of string values from a data source.
        /// 
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The items.
        /// </returns>
        /// <contract><requires name="source" condition="not null"/><ensures condition="not null"/></contract><exception cref="T:Sitecore.Exceptions.LookupSourceException"><c>LookupSourceException</c>.</exception>
        public static string[] GetValues(string source)
        {
            Assert.ArgumentNotNull(source, "source");
            var lookupSourceValuesArgs = new GetLookupSourceValueListsArgs {Source = source};

            try
            {
                using (new LongRunningOperationWatcher(1000, "getLookupSourceValueLists pipeline[source={0}]",source))
                    CorePipeline.Run("getLookupSourceValueLists", lookupSourceValuesArgs);
            }
            catch (Exception ex)
            {
                throw new LookupSourceException(source, ex);
            }

            var output = new string[lookupSourceValuesArgs.Result.Count];
            lookupSourceValuesArgs.Result.CopyTo(output,0);
            return output;
        }

    }
}
