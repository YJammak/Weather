using System;

namespace WeatherCalendar.Models;

/// <summary>
/// 假期
/// </summary>
public class Holiday
{
    /// <summary>
    /// 年
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 休息日期
    /// </summary>
    public DateTime[] RestDates { get; set; }

    /// <summary>
    /// 工作日期
    /// </summary>
    public DateTime[] WorkDates { get; set; }

    public override string ToString() => Name;
}
