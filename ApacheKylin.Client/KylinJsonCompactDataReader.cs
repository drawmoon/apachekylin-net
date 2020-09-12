using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace ApacheKylin.Client
{
    public class KylinJsonCompactDataReader : KylinDataReader
    {
#nullable disable
        private KylinColumnRecord[] _columns;
#nullable enable

        public KylinJsonCompactDataReader(HttpResponseMessage response) : base(response)
        {
        }

        public override string GetDataTypeName(int ordinal) => _columns[ordinal].Type.ToString();

        public override Type GetFieldType(int ordinal) => _columns[ordinal].ClrType;

#nullable disable
        public override string GetName(int ordinal) => _columns[ordinal].Name;
#nullable enable

        public override int GetOrdinal(string name)
        {
            var index = Array.FindIndex(_columns, p => p.Name != null && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (index == -1)
            {
                throw new IndexOutOfRangeException();
            }

            return index;
        }

        public override int FieldCount => _columns.Length;

        protected override void ReadHeaders()
        {
            AssertEqual(true, _jsonTextReader.Read());
            AssertEqual(JsonToken.StartObject, _jsonTextReader.TokenType);

            AssertEqual(true, _jsonTextReader.Read());
            AssertEqual(JsonToken.PropertyName, _jsonTextReader.TokenType);
            AssertEqual("columnMetas", _jsonTextReader.Value);

            AssertEqual(true, _jsonTextReader.Read());
            AssertEqual(JsonToken.StartArray, _jsonTextReader.TokenType);

            _columns = _jsonSerializer.Deserialize<KylinColumnRecord[]>(_jsonTextReader);
            AssertEqual(JsonToken.EndArray, _jsonTextReader.TokenType);

            AssertEqual(true, _jsonTextReader.Read());
            AssertEqual(JsonToken.PropertyName, _jsonTextReader.TokenType);
            AssertEqual("results", _jsonTextReader.Value);

            AssertEqual(true, _jsonTextReader.Read());
            _hasMore = _jsonTextReader.TokenType == JsonToken.StartArray;

            AssertEqual(true, _jsonTextReader.Read());
        }

        public override bool Read()
        {
            if (!_hasMore)
            {
                return _hasRows = false;
            }

            AssertEqual(JsonToken.StartArray, _jsonTextReader.TokenType);

            var records = _jsonSerializer.Deserialize<object[]>(_jsonTextReader);

            if (records != null && records.Length > 0)
            {
                for (var i = 0; i < _columns.Length; i++)
                {
                    records[i] = ConvertTo(records[i], _columns[i].ClrType);
                }

                _records = records;
                _recordsAffected = _records.Length;

                AssertEqual(JsonToken.EndArray, _jsonTextReader.TokenType);

                _hasRows = true;
            }
            else
            {
                _hasRows = false;
            }

            AssertEqual(true, _jsonTextReader.Read());
            _hasMore = _jsonTextReader.TokenType == JsonToken.StartArray;
            return _hasRows;
        }

        [JsonObject]
        private class KylinColumnRecord
        {
            [JsonProperty("label")]
            public string? Label { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("type")]
            public KylinType Type { get; set; }

            [JsonIgnore]
            public Type ClrType
            {
                get
                {
                    switch (Type)
                    {
                        case KylinType.ODBC_TinyInt:
                        case KylinType.ODBC_BigInt:
                        case KylinType.ODBC_Integer:
                        case KylinType.ODBC_SmallInt:
                            return typeof(int);
                        case KylinType.ODBC_Numeric:
                        case KylinType.ODBC_Decimal:
                        case KylinType.ODBC_Float:
                        case KylinType.ODBC_Real:
                        case KylinType.ODBC_Double:
                            return typeof(double);
                        case KylinType.ODBC_DateTime:
                        case KylinType.ODBC_Interval:
                        case KylinType.ODBC_DateTimeOffset:
                        case KylinType.ODBC_Type_Date:
                        case KylinType.ODBC_Type_Time:
                        case KylinType.ODBC_Type_Timestamp:
                            return typeof(DateTime);
                        case KylinType.ODBC_Bit:
                            return typeof(bool);
                        case KylinType.ODBC_LongVarBinary:
                        case KylinType.ODBC_VarBinary:
                        case KylinType.ODBC_Binary:
                        case KylinType.ODBC_LongVarChar:
                        case KylinType.ODBC_Char:
                        case KylinType.ODBC_VarChar:
                        case KylinType.ODBC_Interval_Year:
                        case KylinType.ODBC_Interval_Month:
                        case KylinType.ODBC_Interval_Day:
                        case KylinType.ODBC_Interval_Hour:
                        case KylinType.ODBC_Interval_Minute:
                        case KylinType.ODBC_Interval_Second:
                        case KylinType.ODBC_Interval_Year_To_Month:
                        case KylinType.ODBC_Interval_Day_To_Hour:
                        case KylinType.ODBC_Interval_Day_To_Minute:
                        case KylinType.ODBC_Interval_Day_To_Second:
                        case KylinType.ODBC_Interval_Hour_To_Minute:
                        case KylinType.ODBC_Interval_Hour_To_Second:
                        case KylinType.ODBC_Interval_Minute_To_Second:
                        case KylinType.ODBC_Guid:
                        case KylinType.ODBC_WLongVarChar:
                        case KylinType.ODBC_WVarChar:
                        case KylinType.ODBC_WChar:
                            return typeof(string);
                        default:
                            return typeof(string);
                    }
                }
            }
        }

        private static object ConvertTo(object? val, Type type)
        {
            if (val == null)
            {
                return DBNull.Value;
            }

            switch (type.Name)
            {
                case "int":
                    return int.TryParse((string)val, out var i) ? i : val;
                case "double":
                    return double.TryParse((string)val, out var d) ? d : val;
                case "DateTime":
                    return DateTime.TryParse((string)val, out var dt) ? dt : val;
                case "bool":
                    return bool.TryParse((string)val, out var b) ? b : val;
                case "string":
#nullable disable
                    return val.ToString();
#nullable enable
            }

#nullable disable
            return val;
#nullable enable
        }
    }
}
