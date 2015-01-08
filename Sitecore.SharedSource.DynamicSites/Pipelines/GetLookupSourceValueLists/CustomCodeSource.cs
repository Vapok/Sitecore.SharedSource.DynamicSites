using System;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Diagnostics;

namespace Sitecore.SharedSource.DynamicSites.Pipelines.GetLookupSourceValueLists
{
    internal class CustomCodeSource
    {
        [UsedImplicitly]
        public void Process(GetLookupSourceValueListsArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (!args.Source.StartsWith("code:")) return;

            var values = RunEnumeration(args.Source);
            if (values != null && values.Length > 0)
                args.Result.AddRange(values);
            args.AbortPipeline();
        }

        private static string[] RunEnumeration(string templateSource)
        {
            templateSource = templateSource.Replace("code:", string.Empty);
            var valueList = ReflectionUtility.CreateInstance(Type.GetType(templateSource)) as IValueList;
            return valueList == null ? new string[0] : valueList.ValueListQuery();
        }

    }
}
