using Newtonsoft.Json;

namespace AKS.Three.Tier.App.Frontend.Shared
{
    public class APIEnvInfosDTO
    {
        [JsonProperty("totalAvailableMemory", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? TotalAvailableMemory { get; set; }

        [JsonProperty("memoryUsage", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? MemoryUsage { get; set; }

        [JsonProperty("memoryLimit", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? MemoryLimit { get; set; }

        [JsonProperty("cgroup", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Cgroup { get; set; }

        [JsonProperty("usage", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? Usage { get; set; }

        [JsonProperty("limit", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? Limit { get; set; }

        [JsonProperty("hostName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? HostName { get; set; }

        [JsonProperty("ipList", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<string> IpList { get; set; } = new List<string>();

        [JsonProperty("frameworkDescription", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? FrameworkDescription { get; set; }

        [JsonProperty("osDescription", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? OsDescription { get; set; }

        [JsonProperty("osArchitecture", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? OsArchitecture { get; set; }

        [JsonProperty("processorCount", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? ProcessorCount { get; set; }

        [JsonProperty("containerized", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? Containerized { get; set; }
        [Newtonsoft.Json.JsonProperty("dbEntities", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<ClientDbEntity>? DbEntities { get; set; }
    }
}