namespace WeatherCalendar.Models;

/// <summary>
/// 农历节日
/// </summary>
public class LunarFestival
{
    /// <summary>
    /// 月份(正月，二月...)
    /// </summary>
    public string Month { get; set; }

    /// <summary>
    /// 日期（初一、初二...）
    /// </summary>
    public string Day { get; set; }

    /// <summary>
    /// 节日名称
    /// </summary>
    public string Name { get; set; }

    public override string ToString() => Name;
}
