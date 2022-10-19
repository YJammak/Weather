namespace WeatherCalendar.Models;

/// <summary>
/// 提醒周期
/// </summary>
public enum NoticeCycle
{
    /// <summary>
    /// 无
    /// </summary>
    None,

    /// <summary>
    /// 每天
    /// </summary>
    PerDay,

    /// <summary>
    /// 每周
    /// </summary>
    PerWeek,

    /// <summary>
    /// 每月
    /// </summary>
    PerMonth,

    /// <summary>
    /// 每年
    /// </summary>
    PerYear,

    /// <summary>
    /// 工作日
    /// </summary>
    WorkDay,

    /// <summary>
    /// 休息日
    /// </summary>
    RestDay
}

/// <summary>
/// 提醒倒计时
/// </summary>
public enum NoticeCountdown
{
    /// <summary>
    /// 无
    /// </summary>
    None,

    /// <summary>
    /// 总是
    /// </summary>
    Always,

    /// <summary>
    /// 自定义
    /// </summary>
    Custom
}

/// <summary>
/// 提醒
/// </summary>
public class Notice
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 颜色
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// 周期
    /// </summary>
    public NoticeCycle Cycle { get; set; }

    /// <summary>
    /// 周期信息
    /// </summary>
    public string CycleContent { get; set; }

    /// <summary>
    /// 倒计时
    /// </summary>
    public NoticeCountdown Countdown { get; set; }

    /// <summary>
    /// 倒计时提前天数
    /// </summary>
    public int CountdownDays { get; set; }

    /// <summary>
    /// 详情
    /// </summary>
    public string Detail { get; set; }
}
