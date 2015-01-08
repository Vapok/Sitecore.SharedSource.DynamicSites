using System.Linq;
using Sitecore.Buckets.FieldTypes;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SharedSource.DynamicSites.Pipelines.GetLookupSourceValueLists;

namespace Sitecore.SharedSource.DynamicSites.Fields.Datasources
{
    [UsedImplicitly]
    public class Databases : IDataSource, IValueList
    {
        public Item[] ListQuery(Item item)
        {
            return (from databaseName in Factory.GetDatabaseNames() select Database.GetDatabase(databaseName) into db where db.HasContentItem select db.SitecoreItem).ToArray();
        }

        public string[] ValueListQuery()
        {
            return (from databaseName in Factory.GetDatabaseNames() let db = Database.GetDatabase(databaseName) where db.HasContentItem select databaseName).ToArray();
        }
    }
}
