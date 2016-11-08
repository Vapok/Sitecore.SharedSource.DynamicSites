using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.SharedSource.ValueListField.Pipelines.GetLookupSourceValueLists;

namespace Sitecore.Foundation.Multisite.DynamicSites.Fields.Datasources
{
    [UsedImplicitly]
    public class Databases : IValueList
    {
        public string[] ValueListQuery()
        {
            return (from databaseName in Factory.GetDatabaseNames() let db = Database.GetDatabase(databaseName) where db.HasContentItem select databaseName).ToArray();
        }
    }
}