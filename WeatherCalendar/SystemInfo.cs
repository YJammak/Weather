using System;
using System.Diagnostics;
using System.Management;
using Network;

namespace WeatherCalendar
{
    public class NetWorkSpeedInfo
    {
        public double DownLoadSpeed { get; set; }

        public double UpLoadSpeed { get; set; }
    }


    public class SystemInfo
    {
        private static readonly PerformanceCounter cpuLoadCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        /// <summary>
        /// CPU 使用率
        /// </summary>
        public static float CpuLoad => cpuLoadCounter.NextValue();

        /// <summary>
        /// 物理内存
        /// </summary>
        public static long PhysicalMemory { get; }

        /// <summary>
        /// 可用内存
        /// </summary>
        public static long AvailableMemory
        {
            get
            {
                long availablebytes = 0;
                try
                {
                    var mos = new ManagementClass("Win32_OperatingSystem");
                    foreach (var mo in mos.GetInstances())
                    {
                        if (mo["FreePhysicalMemory"] != null)
                        {
                            availablebytes = 1024 * long.Parse(mo["FreePhysicalMemory"].ToString());
                        }
                    }
                }
                catch
                {
                    //
                }
                return availablebytes;
            }
        }

        /// <summary>
        /// 内存使用率
        /// </summary>
        public static float MemoryLoad
        {
            get
            {
                var available = AvailableMemory;

                return (PhysicalMemory - available) / (float)PhysicalMemory * 100;
            }
        }

        public static NetWorkSpeedInfo NetWpSpeed
        {
            get
            {
                var info = new NetWorkSpeedInfo();

                info.DownLoadSpeed += networkMonitor.ReceivedSpeed;
                info.UpLoadSpeed += networkMonitor.SentSpeed;

                return info;
            }
        }

        private static readonly NetworkMonitor networkMonitor = new NetworkMonitor();
        static SystemInfo()
        {
            var mc = new ManagementClass("Win32_ComputerSystem");
            var moc = mc.GetInstances();
            foreach (var mo in moc)
            {
                if (mo["TotalPhysicalMemory"] != null)
                {
                    PhysicalMemory = long.Parse(mo["TotalPhysicalMemory"].ToString());
                }
            }
            networkMonitor.StartMonitor();
        }
    }
}
