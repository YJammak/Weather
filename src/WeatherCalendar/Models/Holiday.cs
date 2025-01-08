using System;
using System.Collections.Generic;

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
    ///     休息日期
    /// </summary>
    public DateTime[] RestDates { get; set; }

    /// <summary>
    ///     工作日期
    /// </summary>
    public DateTime[] WorkDates { get; set; }

    public override string ToString()
    {
        return $"{Year} - {Name}";
    }

    public static bool Equals(Holiday one, Holiday two)
    {
        if (one == null || two == null)
            return false;

        if (one.Year != two.Year)
            return false;

        if (one.Name != two.Name)
            return false;

        if (one.RestDates?.Length != two.RestDates?.Length)
            return false;

        if (one.WorkDates?.Length != two.WorkDates?.Length)
            return false;

        return true;
    }

    public static Holiday Combine(Holiday one, Holiday two)
    {
        var holiday = new Holiday
        {
            Year = one.Year,
            Name = one.Name
        };

        var rest = new List<DateTime>();
        if (one.RestDates != null)
            rest.AddRange(one.RestDates);
        if (two.RestDates != null)
            foreach (var date in two.RestDates)
                if (!rest.Contains(date))
                    rest.Add(date);

        var work = new List<DateTime>();
        if (one.WorkDates != null)
            work.AddRange(one.WorkDates);
        if (two.WorkDates != null)
            foreach (var date in two.WorkDates)
                if (!work.Contains(date))
                    work.Add(date);

        holiday.RestDates = rest.ToArray();
        holiday.WorkDates = work.ToArray();

        return holiday;
    }
}
