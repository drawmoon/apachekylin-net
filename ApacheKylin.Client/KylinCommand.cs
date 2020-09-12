using System;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ApacheKylin.Client
{
    public class KylinCommand : DbCommand
    {
        private KylinConnection? _kylinConnection;
        private readonly KylinParameterCollection _kylinParameterCollection = new KylinParameterCollection();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public KylinCommand()
        {
        }
        
        public KylinCommand(KylinConnection kylinConnection)
        {
            _kylinConnection = kylinConnection;
        }

        public KylinCommand(KylinConnection kylinConnection, string commandText)
        {
            _kylinConnection = kylinConnection;
            CommandText = commandText;
        }
        
        public override void Cancel()
        {
            throw new System.NotImplementedException();
        }

        public override int ExecuteNonQuery()
        {
            throw new System.NotImplementedException();
        }

        public override object ExecuteScalar()
        {
            throw new System.NotImplementedException();
        }

        public override void Prepare()
        {
            throw new System.NotImplementedException();
        }

        public sealed override string? CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection? DbConnection
        {
            get => _kylinConnection;
            set => _kylinConnection = (KylinConnection?) value;
        }

        protected override DbParameterCollection DbParameterCollection => _kylinParameterCollection;
        protected override DbTransaction? DbTransaction { get; set; }
        public override bool DesignTimeVisible { get; set; }

        protected override DbParameter CreateDbParameter() => new KylinParameter();

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
            ExecuteDbDataReaderAsync(behavior, _cancellationTokenSource.Token).GetAwaiter().GetResult();

        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(CommandText) || _kylinConnection == null)
            {
                throw new KylinException("Command text is null.");
            }

            var regex = new Regex("^show (?<m>tables|columns);?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            if (regex.IsMatch(CommandText))
            {
                var val = regex.Match("tables").Groups["m"].Value;

                var response = await _kylinConnection.ListQueryableTablesAsync(cancellationToken);

                if (val == "tables")
                {
                    return new KylinMetaTableReader(response);
                }
                else
                {
                    return new KylinMetaColumnReader(response);
                }
            }
            else
            {
                var kylinQuery = new KylinQuery(CommandText);

                switch (behavior)
                {
                    case CommandBehavior.SingleRow:
                    case CommandBehavior.SingleResult:
                        kylinQuery.Limit = 1;
                        break;
                    case CommandBehavior.SchemaOnly:
                        kylinQuery.Limit = 0;
                        break;
                    case CommandBehavior.KeyInfo:
                    case CommandBehavior.Default:
                    case CommandBehavior.SequentialAccess:
                    case CommandBehavior.CloseConnection:
                        break;
                }

                var response = await _kylinConnection.PostBulkDataQueryAsync(kylinQuery, cancellationToken);

                return new KylinJsonCompactDataReader(response);
            }
        }
    }
}