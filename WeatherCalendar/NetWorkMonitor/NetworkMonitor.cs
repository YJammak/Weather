using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Network
{
	public class NetworkMonitor
	{
		private Timer timer;

	    public NetworkInterface[] NetworkInterfaces => NetworkInterface.GetAllNetworkInterfaces();


        private IEnumerable<NetworkInterface> monitorInterfaces;

        public long SentSpeed { get; set; }

        public long ReceivedSpeed { get; set; }

        public long TotalSend { get; set; }

        public long TotalReceived { get; set; }

	    public NetworkMonitor()
	    {
	    }

	    private long lastTotalSend;
	    private long lastTotalReceived;

	    private bool IsMonitorAll = false;

        public void StartMonitor(IEnumerable<NetworkInterface> interfaces = null)
        {
            monitorInterfaces = interfaces ?? NetworkInterfaces;

            IsMonitorAll = interfaces == null;

            lastTotalSend = 0;
            lastTotalReceived = 0;
            SentSpeed = 0;
            ReceivedSpeed = 0;
            foreach (var networkInterface in monitorInterfaces)
            {
                lastTotalSend += networkInterface.GetIPv4Statistics().BytesSent / 1024;
                lastTotalReceived += networkInterface.GetIPv4Statistics().BytesReceived / 1024;
            }
            TotalSend = lastTotalSend;
            TotalReceived = lastTotalReceived;

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += (sender, args) =>
            {
                if (IsMonitorAll)
                    monitorInterfaces = NetworkInterfaces;

                if (monitorInterfaces == null)
                    return;

                long tempSent = 0;
                long tempReceived = 0;
                foreach (var networkInterface in monitorInterfaces)
                {
                    tempSent += networkInterface.GetIPStatistics().BytesSent / 1024;
                    tempReceived += networkInterface.GetIPStatistics().BytesReceived / 1024;
                }

                lastTotalSend = TotalSend;
                lastTotalReceived = TotalReceived;

                TotalSend = tempSent;
                TotalReceived = tempReceived;

                SentSpeed = TotalSend - lastTotalSend;
                ReceivedSpeed = TotalReceived - lastTotalReceived;
            };
            timer.Start();
        }

        public void StopMonitor()
	    {
            timer.Stop();
        }
    }
}
