using Newtonsoft.Json;

namespace ApacheKylin.Client
{
    [JsonObject]
    public class KylinQuery
    {
        [JsonConstructor]
        public KylinQuery(string sql)
        {
            Sql = sql;
        }
        
        [JsonProperty("sql")]
        public string Sql { get; set; }

        [JsonProperty("project")]
        public string? Project { get; set; }

        [JsonProperty("offset")]
        public int? Offset { get; set; }

        [JsonProperty("limit")]
        public int? Limit { get; set; }

        [JsonProperty("acceptPartial")]
        public bool AcceptPartial { get; set; } = false;
    }
}