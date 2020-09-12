using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ApacheKylin.Client
{
    public class KylinMetaTableReader : KylinDataReader
    {
#nullable disable
        private KylinMetaColumn[] _columns;
        private KylinMetaTableRecord[] _metaTableRecords;
#nullable enable

        public KylinMetaTableReader(HttpResponseMessage response) : base(response)
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
#nullable disable
            return _columns[ordinal].Name;
#nullable enable
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

            var metaTableRecords = _jsonSerializer.Deserialize<KylinMetaTableRecord[]>(_jsonTextReader);

            AssertEqual(JsonToken.EndArray, _jsonTextReader.TokenType);

            var properties = typeof(KylinMetaTableRecord).GetProperties();

            var columns = new KylinMetaColumn[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                columns[i] = new KylinMetaColumn(property.Name, property.PropertyType);
            }

            _metaTableRecords = metaTableRecords;
            _columns = columns;
        }

        public override bool Read()
        {
            if (_recordsAffected >= _metaTableRecords.Length)
            {
                return false;
            }

            var currentTableRecord = _metaTableRecords[_recordsAffected];

            var type = typeof(KylinMetaTableRecord);
            var properties = type.GetProperties();

            var records = new object[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                var value = property.GetValue(currentTableRecord, null);

#nullable disable
                records[i] = Convert.ChangeType(value, _columns[i].Type);
#nullable enable
            }

            _records = records;
            _recordsAffected++;
            return true;
        }

        [JsonObject]
        private class KylinMetaTableRecord
        {
            [JsonProperty("table_NAME")]
            public string? Name { get; set; }

            [JsonProperty("remarks")]
            public string? Remarks { get; set; }
        }
    }
}
