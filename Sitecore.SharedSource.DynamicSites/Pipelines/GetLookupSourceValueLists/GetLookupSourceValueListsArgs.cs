
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Sitecore.SharedSource.DynamicSites.Pipelines.GetLookupSourceValueLists
{
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public class GetLookupSourceValueListsArgs : PipelineArgs
    {
        private readonly StringCollection _result = new StringCollection();
        private string _source;

        public StringCollection Result
        {
            get
            {
                return _result;
            }
        }

        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                _source = value;
            }
        }
    }
}
