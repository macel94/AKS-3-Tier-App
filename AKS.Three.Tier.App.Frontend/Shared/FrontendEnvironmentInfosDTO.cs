using System.Net;
using System.Runtime.InteropServices;

namespace AKS.Three.Tier.App.Frontend.Shared
{
    public class FrontendEnvironmentInfosDTO
    {
        const long Mebi = 1024 * 1024;
        const long Gibi = Mebi * 1024;
        public string TotalAvailableMemory { get; set; } = string.Empty;
        public string MemoryUsage { get; set; } = string.Empty;
        public string MemoryLimit { get; set; } = string.Empty;
        public bool Cgroup { get; set; }
        public string Usage { get; set; } = string.Empty;
        public string Limit { get; set; } = string.Empty;
        public string HostName { get; set; } = string.Empty;
        public IEnumerable<string> IpList { get; set; } = Array.Empty<string>();
        public string FrameworkDescription { get; set; } = RuntimeInformation.FrameworkDescription;
        public string OSDescription { get; set; } = RuntimeInformation.OSDescription;
        public string OSArchitecture { get; set; } = RuntimeInformation.OSArchitecture.ToString();
        public string ProcessorCount { get; set; } = Environment.ProcessorCount.ToString();
        public string Containerized { get; set; } = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") is null ? "false" : "true";
        public FrontendEnvironmentInfosDTO()
        {
            HostName = Dns.GetHostName();
            GCMemoryInfo gcInfo = GC.GetGCMemoryInfo();
            TotalAvailableMemory = GetInBestUnit(gcInfo.TotalAvailableMemoryBytes);

            Cgroup = RuntimeInformation.OSDescription.StartsWith("Linux") && Directory.Exists("/sys/fs/cgroup/memory");
            if (Cgroup)
            {
                Usage = File.ReadAllLines("/sys/fs/cgroup/memory/memory.usage_in_bytes")[0];
                Limit = File.ReadAllLines("/sys/fs/cgroup/memory/memory.limit_in_bytes")[0];
                MemoryUsage = GetInBestUnit(long.Parse(Usage));
                MemoryLimit = GetInBestUnit(long.Parse(Limit));
            }
        }

        public async Task GetIpInfosAsync()
        {
            IPAddress[] ips = await Dns.GetHostAddressesAsync(HostName);
            IpList = ips.Select(x => x.ToString());
        }

        string GetInBestUnit(long size)
        {
            if (size < Mebi)
            {
                return $"{size} bytes";
            }
            else if (size < Gibi)
            {
                decimal mebibytes = decimal.Divide(size, Mebi);
                return $"{mebibytes:F} MiB";
            }
            else
            {
                decimal gibibytes = decimal.Divide(size, Gibi);
                return $"{gibibytes:F} GiB";
            }
        }
    }
}