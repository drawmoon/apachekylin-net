using System.Data;
using System.Data.Common;

namespace ApacheKylin.Client
{
    public class KylinParameter : DbParameter
    {
        public override void ResetDbType()
        {
            throw new System.NotImplementedException();
        }

        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string? ParameterName { get; set; }
        public override string? SourceColumn { get; set; }
        public override object? Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }
    }
}