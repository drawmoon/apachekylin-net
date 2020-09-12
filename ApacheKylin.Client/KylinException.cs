using System.Data.Common;

namespace ApacheKylin.Client
{
    public class KylinException : DbException
    {
        public KylinException(string message) : base(message)
        {
        }
    }
}