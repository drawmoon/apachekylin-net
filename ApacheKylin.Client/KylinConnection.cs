using System;
using System.Data;
using System.Data.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApacheKylin.Client
{
    public class KylinConnection : DbConnection
    {
        private HttpClient? _client;
        private KylinConnectionSetting? _kylinConnectionSetting;
        private ConnectionState _state;

        public KylinConnection()
        {
        }
        
        public KylinConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }
        
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new System.NotImplementedException();
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw new System.NotImplementedException();
        }

        public override void Close() => Dispose(true);

        private bool _disposedValue;
        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _client?.Dispose();
                }

                _state = ConnectionState.Closed;
                _disposedValue = true;
            }
            
            base.Dispose(disposing);
        }

        public override void Open() => OpenAsync().GetAwaiter().GetResult();

        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            try
            {
                _client = new HttpClient
                {
                    BaseAddress = BuildUri()
                };

                _state = ConnectionState.Connecting;
                
                // Test connection
                var response = await PostAsync("/kylin/api/user/authentication", null, cancellationToken);
                response.EnsureSuccessStatusCode();

                _state = ConnectionState.Open;
            }
            catch (Exception)
            {
                _state = ConnectionState.Broken;
                throw;
            }
        }

        private Uri BuildUri()
        {
            if (_kylinConnectionSetting == null)
            {
                throw new KylinException("Connection string is null.");
            }

            return new Uri($"{_kylinConnectionSetting.Host}:{_kylinConnectionSetting.Port}");
        }

        internal async Task<HttpResponseMessage> ListQueryableTablesAsync(CancellationToken token)
        {
            return await GetAsync($"/kylin/api/tables_and_columns?project={Project}", token);
        }

        internal async Task<HttpResponseMessage> PostSqlQueryAsync(string sql, CancellationToken cancellationToken)
        {
            var kylinQuery = new KylinQuery(sql)
            {
                Project = Project
            };

            return await PostBulkDataQueryAsync(kylinQuery, cancellationToken);
        }

        internal async Task<HttpResponseMessage> PostBulkDataQueryAsync(KylinQuery kylinQuery,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(kylinQuery.Sql))
            {
                throw new KylinException("Command text is null.");
            }

            kylinQuery.Project = Project;

            var queryBody = JsonConvert.SerializeObject(kylinQuery);
            
            var content = new StringContent(queryBody, Encoding.UTF8, MediaTypeNames.Application.Json);

            return await PostAsync("/kylin/api/query", content, cancellationToken);
        }

        private async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken token)
        {
            if (_disposedValue || _client == null)
            {
                throw new KylinException("Closed state on connection");
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            AddDefaultRequestHeaders(request.Headers);

            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
            return await HandleError(response);
        }

        private async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent? content,
            CancellationToken cancellationToken)
        {
            if (_disposedValue || _client == null)
            {
                throw new KylinException("Closed state on connection.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            AddDefaultRequestHeaders(request.Headers);

            request.Content = content;

            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            return await HandleError(response);
        }

        private void AddDefaultRequestHeaders(HttpRequestHeaders requestHeaders)
        {
            var basicString = Convert.ToBase64String(
#nullable disable
                Encoding.UTF8.GetBytes($"{_kylinConnectionSetting.User}:{_kylinConnectionSetting.Password}"));
#nullable enable

            requestHeaders.Add("Accept", MediaTypeNames.Application.Json);
            requestHeaders.Add("Authorization", $"Basic {basicString}");
        }

        private static async Task<HttpResponseMessage> HandleError(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new KylinException(errorMessage);
            }

            return response;
        }

        public sealed override string? ConnectionString
        {
            get
            {
                return _kylinConnectionSetting?.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _kylinConnectionSetting = new KylinConnectionSetting(value); 
                }
            }
        }
        public string Project
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_kylinConnectionSetting?.Project))
                {
                    throw new KylinException("Project name should not be empty.");
                }

                return _kylinConnectionSetting.Project;
            }
        }
        public override string? Database => _kylinConnectionSetting?.Project;
        public override ConnectionState State => _state;
        public override string? DataSource => null;
        public override string? ServerVersion => null;

        protected override DbCommand CreateDbCommand() => new KylinCommand(this);
    }
}