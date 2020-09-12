using System.Data.Common;

namespace ApacheKylin.Client
{
    public class KylinProviderFactory : DbProviderFactory
    {
        public static readonly KylinProviderFactory Instance = new KylinProviderFactory();

        public override bool CanCreateDataAdapter => true;

        public override DbConnection CreateConnection() => new KylinConnection();

        public override DbCommand CreateCommand() => new KylinCommand();

        public override DbParameter CreateParameter() => new KylinParameter();

        public override DbDataAdapter CreateDataAdapter() => new KylinDataAdapter();
    }
}