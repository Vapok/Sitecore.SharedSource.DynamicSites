using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.StringExtensions;

namespace Sitecore.SharedSource.DynamicSites.Pipelines.GetLookupSourceValueLists
{
    internal class DelimitedListSource
    {
        [UsedImplicitly]
        public void Process(GetLookupSourceValueListsArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.Source.IsNullOrEmpty()) return;

            var values = new List<string>(args.Source.Split('|').Select(x => x.Trim()));
            if (values.Count > 0)
                args.Result.AddRange(values.ToArray());
        }
    }
}
