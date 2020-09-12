using System.Data;
using Xunit;

namespace ApacheKylin.Client.Tests
{
    public class SimpleTests
    {
        [Fact]
        public void Test1()
        {
            var connectionString = "http://localhost:7070/DEFAULT,user=admin,password=KYLIN";

            var kylinConnectionSetting = new KylinConnectionSetting(connectionString);

            Assert.Equal("http://localhost", kylinConnectionSetting.Host);
            Assert.Equal((ushort)7070, kylinConnectionSetting.Port);
            Assert.Equal("admin", kylinConnectionSetting.User);
            Assert.Equal("KYLIN", kylinConnectionSetting.Password);
            Assert.Equal("test_pro", kylinConnectionSetting.Project);
        }

        private KylinConnection CreateDefaultKylinConnection(string connectionString = "http://localhost:7070/DEFAULT,user=admin,password=KYLIN")
        {
            var kylinConnection = new KylinConnection(connectionString);
            kylinConnection.Open();

            return kylinConnection;
        }

        [Theory]
        [InlineData("show tables")]
        [InlineData("show columns")]
        [InlineData("select * from TEST_KYLIN_FACT")]
        public void Test2(string sql)
        {
            using var kylinConnection = CreateDefaultKylinConnection();

            Assert.Equal(ConnectionState.Open, kylinConnection.State);

            using var kylinCommand = kylinConnection.CreateCommand();
            kylinCommand.CommandText = sql;

            using var dataAdapter = KylinProviderFactory.Instance.CreateDataAdapter();
            dataAdapter.SelectCommand = kylinCommand;

            var dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            var dataTable = dataSet.Tables[0];

            Assert.NotNull(dataTable);
        }
    }
}