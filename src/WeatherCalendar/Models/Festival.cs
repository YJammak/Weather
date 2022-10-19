namespace WeatherCalendar.Models;

/// <summary>
/// 公历节日
/// </summary>
public class Festival
{
    /// <summary>
    /// 月份
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// 日期
    /// </summary>
    public int Day { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    public override string ToString() => Name;
}
