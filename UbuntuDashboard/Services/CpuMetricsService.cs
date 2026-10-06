using System;
using System.IO;
using System.Threading.Tasks;
using UbuntuDashboard.Models;

namespace UbuntuDashboard.Services
{
    public class CpuMetricsService
    {
        public class CoreUsageInfo
        {
            public int CoreId { get; set; }
            public double UsagePercentage { get; set; }
        }

        public async Task<ServerStatsModel> GetServerStatsAsync()
        {
            var stats = new ServerStatsModel();

            if (!OperatingSystem.IsLinux())
            {
                stats.CpuTotalUsage = 45.2;
                stats.RamUsedGB = 0.7;
                stats.RamTotalGB = 18.9;
                stats.UptimeText = "48 minutes";
                stats.CoresUsage = new List<CoreUsageInfo>
            {
                new CoreUsageInfo { CoreId = 0, UsagePercentage = 35.0 },
                new CoreUsageInfo { CoreId = 1, UsagePercentage = 55.4 },
                new CoreUsageInfo { CoreId = 2, UsagePercentage = 20.1 },
                new CoreUsageInfo { CoreId = 3, UsagePercentage = 70.8 }
            };
                return stats;
            }

            try
            {
                var (snapshot1Total, snapshot1Cores) = ReadCpuStats();
                await Task.Delay(200);
                var (snapshot2Total, snapshot2Cores) = ReadCpuStats();

                stats.CpuTotalUsage = CalculateCpuPercentage(snapshot1Total, snapshot2Total);

                for (int i = 0; i < snapshot2Cores.Count; i++)
                {
                    if (i < snapshot1Cores.Count)
                    {
                        double coreUsage = CalculateCpuPercentage(snapshot1Cores[i], snapshot2Cores[i]);
                        stats.CoresUsage.Add(new CoreUsageInfo
                        {
                            CoreId = i,
                            UsagePercentage = coreUsage
                        });
                    }
                }

                ReadRamInfo(stats);

                ReadUptimeInfo(stats);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd odczytu danych systemowych: {ex.Message}");
            }

            return stats;
        }

        private (CpuRawData total, List<CpuRawData> cores) ReadCpuStats()
        {
            string[] lines = File.ReadAllLines("/proc/stat");

            CpuRawData total = ParseCpuLine(lines[0]);
            List<CpuRawData> cores = new();

            foreach (var line in lines.Skip(1))
            {
                if (line.StartsWith("cpu") && char.IsDigit(line[3]))
                {
                    cores.Add(ParseCpuLine(line));
                }
                else
                {
                    break;
                }
            }

            return (total, cores);
        }

        private struct CpuRawData
        {
            public long Idle;
            public long Total;
        }

        private CpuRawData ParseCpuLine(string line)
        {
            string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            long user = long.Parse(tokens[1]);
            long nice = long.Parse(tokens[2]);
            long system = long.Parse(tokens[3]);
            long idle = long.Parse(tokens[4]);
            long iowait = long.Parse(tokens[5]);
            long irq = long.Parse(tokens[6]);
            long softirq = long.Parse(tokens[7]);

            long totalIdle = idle + iowait;
            long totalSystem = user + nice + system + totalIdle + irq + softirq;

            return new CpuRawData { Idle = totalIdle, Total = totalSystem };
        }

        private double CalculateCpuPercentage(CpuRawData snapshot1, CpuRawData snapshot2)
        {
            double idleDelta = snapshot2.Idle - snapshot1.Idle;
            double totalDelta = snapshot2.Total - snapshot1.Total;

            if (totalDelta == 0) return 0;

            double usage = (1.0 - (idleDelta / totalDelta)) * 100.0;
            return Math.Round(usage, 1);
        }

        private void ReadRamInfo(ServerStatsModel stats)
        {
            string[] lines = File.ReadAllLines("/proc/meminfo");
            double totalKb = 0;
            double availableKb = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("MemTotal:"))
                    totalKb = ParseKbValue(line);
                else if (line.StartsWith("MemAvailable:"))
                    availableKb = ParseKbValue(line);
            }

            double totalGb = totalKb / 1024.0 / 1024.0;
            double usedGb = (totalKb - availableKb) / 1024.0 / 1024.0;

            stats.RamTotalGB = Math.Round(totalGb, 1);
            stats.RamUsedGB = Math.Round(usedGb, 1);
        }

        private double ParseKbValue(string line)
        {
            string parts = new string(line.Where(char.IsDigit).ToArray());
            return double.TryParse(parts, out double result) ? result : 0;
        }

        private void ReadUptimeInfo(ServerStatsModel stats)
        {
            string content = File.ReadAllText("/proc/uptime");
            string firstValue = content.Split(' ')[0];

            if (double.TryParse(firstValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double uptimeSeconds))
            {
                TimeSpan time = TimeSpan.FromSeconds(uptimeSeconds);
                if (time.TotalHours >= 1)
                {
                    stats.UptimeText = $"{(int)time.TotalHours} hours {time.Minutes} mins";
                }
                else
                {
                    stats.UptimeText = $"{time.Minutes} minutes";
                }
            }
        }
    }
}