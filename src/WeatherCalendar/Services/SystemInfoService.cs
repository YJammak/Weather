using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System.Reactive.Linq;
using WeatherCalendar.Models;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.Services;

public class SystemInfoService : ReactiveObject
{
    /// <summary>
    /// CPU 使用率
    /// </summary>
    [ObservableAsProperty]
    public float CpuLoad { get; }

    /// <summary>
    /// 物理内存
    /// </summary>
    [Reactive]
    public long PhysicalMemory { get; set; }

    /// <summary>
    /// 可用内存
    /// </summary>
    [ObservableAsProperty]
    public long AvailableMemory { get; }

    /// <summary>
    /// 内存使用率
    /// </summary>
    [ObservableAsProperty]
    public float MemoryLoad { get; }

    /// <summary>
    /// 网络速度
    /// </summary>
    [ObservableAsProperty]
    public NetWorkInfo NetWorkInfo { get; }

    private static readonly PerformanceCounter CpuLoadCounter = new("Processor", "% Processor Time", "_Total");

    private static IEnumerable<NetworkInterface> NetworkInterfaces => NetworkInterface.GetAllNetworkInterfaces();

    private long LastTotalSend { get; set; }

    private long LastTotalReceived { get; set; }


    public SystemInfoService()
    {
        var appService = Locator.Current.GetService<AppService>();

        var mc = new ManagementClass("Win32_ComputerSystem");
        var moc = mc.GetInstances();
        foreach (var mo in moc)
        {
            if (mo["TotalPhysicalMemory"] != null)
            {
                PhysicalMemory = long.Parse(mo["TotalPhysicalMemory"].ToString()!);
            }
        }

        appService
            .TimerPerSecond
            .Select(_ => Update())
            .ToPropertyEx(this, monitor => monitor.NetWorkInfo);

        appService
            .TimerPerSecond
            .Select(_ => CpuLoadCounter.NextValue())
            .ToPropertyEx(this, service => service.CpuLoad);

        appService
            .TimerPerSecond
            .Select(_ =>
            {
                long availableBytes = 0;
                try
                {
                    var mos = new ManagementClass("Win32_OperatingSystem");
                    foreach (var mo in mos.GetInstances())
                    {
                        availableBytes = 1024 * long.Parse(mo["FreePhysicalMemory"]!.ToString() ?? "0");
                    }
                }
                catch
                {
                    //
                }

                return availableBytes;
            })
            .ToPropertyEx(this, service => service.AvailableMemory);

        this.WhenAnyValue(x => x.AvailableMemory)
            .Select(available => (PhysicalMemory - available) / (float)PhysicalMemory * 100)
            .ToPropertyEx(this, service => service.MemoryLoad);
    }

    private NetWorkInfo Update()
    {
        long tempSent = 0;
        long tempReceived = 0;
        foreach (var networkInterface in NetworkInterfaces)
        {
            tempSent += networkInterface.GetIPStatistics().BytesSent / 1024;
            tempReceived += networkInterface.GetIPStatistics().BytesReceived / 1024;
        }

        LastTotalSend = NetWorkInfo?.TotalSend ?? 0;
        LastTotalReceived = NetWorkInfo?.TotalReceived ?? 0;

        if (NetWorkInfo == null)
            return new NetWorkInfo(0, 0, tempSent, tempReceived);

        var totalSend = tempSent;
        var totalReceived = tempReceived;
        var sentSpeed = totalSend - LastTotalSend;
        var receivedSpeed = totalReceived - LastTotalReceived;

        return new NetWorkInfo(sentSpeed, receivedSpeed, totalSend, totalReceived);
    }
}
