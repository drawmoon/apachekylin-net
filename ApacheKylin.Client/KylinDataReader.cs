using System;
using System.Collections;
using System.Data.Common;
using System.IO;
using System.Net.Http;
using ApacheKylin.Client.JsonConverters;
using Newtonsoft.Json;

namespace ApacheKylin.Client
{
    public abstract class KylinDataReader : DbDataReader
    {
        private readonly HttpResponseMessage _response;
        protected readonly JsonTextReader _jsonTextReader;
        protected readonly JsonSerializer _jsonSerializer = new JsonSerializer();
        protected int _recordsAffected;
        protected bool _hasRows;
        protected bool _hasMore;
        protected object[]? _records;

        public KylinDataReader(HttpResponseMessage response)
        {
            _response = response;
            var streamReader = new StreamReader(response.Content.ReadAsStreamAsync().GetAwaiter().GetResult());
            _jsonTextReader = new JsonTextReader(streamReader) {SupportMultipleContent = true, CloseInput = false};
            _jsonSerializer.Converters.Add(new DatabaseValueConverter());
            _jsonSerializer.DateFormatString = string.Empty;

            ReadHeaders();
        }

        public override bool GetBoolean(int ordinal) => (bool) GetValue(ordinal);

        public override byte GetByte(int ordinal) => (byte) GetValue(ordinal);

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal) => (char) GetValue(ordinal);

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public abstract override string GetDataTypeName(int ordinal);

        public override DateTime GetDateTime(int ordinal) => (DateTime) GetValue(ordinal);

        public override decimal GetDecimal(int ordinal) => (decimal) GetValue(ordinal);

        public override double GetDouble(int ordinal) => (double) GetValue(ordinal);

        public abstract override Type GetFieldType(int ordinal);

        public override float GetFloat(int ordinal) => (float) GetValue(ordinal);

        public override Guid GetGuid(int ordinal) => (Guid) GetValue(ordinal);

        public override short GetInt16(int ordinal) => (short) GetValue(ordinal);

        public override int GetInt32(int ordinal) => (int) GetValue(ordinal);

        public override long GetInt64(int ordinal) => (long) GetValue(ordinal);

        public abstract override string GetName(int ordinal);

        public abstract override int GetOrdinal(string name);

        public override string GetString(int ordinal) => (string) GetValue(ordinal);

        public override object GetValue(int ordinal)
        {
            if (_records == null)
            {
                throw new InvalidOperationException("Please read first.");
            }
            
            return _records[ordinal];
        }

        public override int GetValues(object[] values)
        {
            if (_records == null)
            {
                throw new InvalidOperationException("Please read first.");
            }
            
            _records.CopyTo(values, 0);
            return values.Length;
        }

        public override bool IsDBNull(int ordinal) => GetValue(ordinal) == DBNull.Value;

        public abstract override int FieldCount { get; }

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));

        public override int RecordsAffected => _recordsAffected;
        public override bool HasRows => _hasRows;
        public override bool IsClosed => _disposedValue;

        public override bool NextResult() => false;

        protected abstract void ReadHeaders();

        public abstract override bool Read();

        protected virtual void AssertEqual<T>(T expected, T actual)
        {
#nullable disable
            if (!expected.Equals(actual))
            {
                throw new KylinException($"{actual} not equal {expected}");
            }
#nullable enable
        }

        private bool _disposedValue;
        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _response.Dispose();
                }

                _disposedValue = true;
            }
        }

        public override int Depth => 0;

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}