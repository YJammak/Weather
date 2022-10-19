namespace WeatherCalendar.Models;

public class NetWorkInfo
{
    /// <summary>
    /// 发送速度
    /// </summary>
    public long SentSpeed { get; }

    /// <summary>
    /// 接收速度
    /// </summary>
    public long ReceivedSpeed { get; }

    /// <summary>
    /// 总发送量
    /// </summary>
    public long TotalSend { get; }

    /// <summary>
    /// 总接收量
    /// </summary>
    public long TotalReceived { get; }

    public NetWorkInfo(long sentSpeed, long receivedSpeed, long totalSend, long totalReceived)
    {
        SentSpeed = sentSpeed;
        ReceivedSpeed = receivedSpeed;
        TotalSend = totalSend;
        TotalReceived = totalReceived;
    }
}
