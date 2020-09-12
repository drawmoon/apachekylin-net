using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ApacheKylin.Client
{
    public class KylinMetaColumnReader : KylinDataReader
    {
#nullable disable
        private KylinMetaColumnRecord[] _metaColumnRecords;
        private KylinMetaColumn[] _columns;
#nullable enable

        public KylinMetaColumnReader(HttpResponseMessage response) : base(response)
        {
        }

        public override string GetDataTypeName(int ordinal)
        {
            return _columns[ordinal].Type.Name;
        }

        public override Type GetFieldType(int ordinal)
        {
            return _columns[ordinal].Type;
        }

        public override string GetName(int ordinal)
        {
            return _columns[ordinal].Name;
        }

        public override int GetOrdinal(string name)
        {
            var index = Array.FindIndex(_columns, p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

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
            AssertEqual(JsonToken.StartArray, _jsonTextReader.TokenType);

            AssertEqual(true, _jsonTextReader.Read());
            var tbTag = _jsonTextReader.TokenType == JsonToken.StartObject;

            var metaColumnRecords = new List<KylinMetaColumnRecord>();

            while (tbTag)
            {
                AssertEqual(true, _jsonTextReader.Read());
                AssertEqual(JsonToken.PropertyName, _jsonTextReader.TokenType);
                AssertEqual("columns", _jsonTextReader.Value);

                AssertEqual(true, _jsonTextReader.Read());
                AssertEqual(JsonToken.StartArray, _jsonTextReader.TokenType);

                var metaColumnRecords2 = _jsonSerializer.Deserialize<KylinMetaColumnRecord[]>(_jsonTextReader);
                metaColumnRecords.AddRange(metaColumnRecords2);

                AssertEqual(JsonToken.EndArray, _jsonTextReader.TokenType);

                AssertEqual(true, _jsonTextReader.Read());
                var colTag = _jsonTextReader.TokenType == JsonToken.EndObject;

                while (!colTag)
                {
                    AssertEqual(true, _jsonTextReader.Read());
                    colTag = _jsonTextReader.TokenType == JsonToken.EndObject;
                }

                AssertEqual(true, _jsonTextReader.Read());
                tbTag = _jsonTextReader.TokenType == JsonToken.StartObject;
            }

            AssertEqual(JsonToken.EndArray, _jsonTextReader.TokenType);

            var properties = typeof(KylinMetaColumnRecord).GetProperties();

            var columns = new KylinMetaColumn[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                columns[i] = new KylinMetaColumn(property.Name, property.PropertyType);
            }

            _metaColumnRecords = metaColumnRecords.ToArray();
            _columns = columns;
        }

        public override bool Read()
        {
            if (_recordsAffected >= _metaColumnRecords.Length)
            {
                return false;
            }

            var currentColumnRecord = _metaColumnRecords[_recordsAffected];

            var type = typeof(KylinMetaColumnRecord);
            var properties = type.GetProperties();

            var records = new object[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                var value = property.GetValue(currentColumnRecord, null);

#nullable disable
                records[i] = Convert.ChangeType(value, _columns[i].Type);
#nullable enable
            }

            _records = records;
            _recordsAffected++;
            return true;
        }

        [JsonObject]
        private class KylinMetaColumnRecord
        {
            [JsonProperty("column_NAME")]
            public string? Name { get; set; }

            [JsonProperty("table_NAME")]
            public string? TableName { get; set; }

            [JsonProperty("type_NAME")]
            public string? DataType { get; set; }

            [JsonProperty("is_NULLABLE")]
            public string? Nullable { get; set; }

            [JsonProperty("remarks")]
            public string? Remarks { get; set; }
        }
    }
}
