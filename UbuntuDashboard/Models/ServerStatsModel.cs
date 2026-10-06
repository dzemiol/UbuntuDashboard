using UbuntuDashboard.Services;

namespace UbuntuDashboard.Models
{
    public class ServerStatsModel : CpuMetricsService
    {
        public double CpuTotalUsage { get; set; }
        public List<CoreUsageInfo> CoresUsage { get; set; } = new();
        public double RamUsedGB { get; set; }
        public double RamTotalGB { get; set; }
        public string UptimeText { get; set; } = "0 mins";
    }
}
