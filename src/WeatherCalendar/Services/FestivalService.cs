using Splat;
using System;
using System.Linq;
using WeatherCalendar.Models;
using WeatherCalendar.Utils;

namespace WeatherCalendar.Services;

/// <summary>
/// 节日服务
/// </summary>
public class FestivalService
{
    /// <summary>
    /// 农历节日
    /// </summary>
    public LunarFestival[] LunarFestivals { get; set; }

    /// <summary>
    /// 公历节日
    /// </summary>
    public Festival[] Festivals { get; set; }

    private readonly CalendarService _calendarService;

    public FestivalService()
    {
        _calendarService = Locator.Current.GetService<CalendarService>();
    }

    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="file"></param>
    public void Load(string file)
    {
        try
        {
            var festival = JsonHelper.LoadFromFileToObject<FestivalService>(file);
            LunarFestivals = festival.LunarFestivals;
            Festivals = festival.Festivals;
        }
        catch
        {
            LunarFestivals = Array.Empty<LunarFestival>();
            Festivals = Array.Empty<Festival>();
        }
    }

    /// <summary>
    /// 获取指定日期的农历节日
    /// </summary>
    /// <returns></returns>
    public string GetLunarFestival(DateTime date)
    {
        if (date.Year < 1901)
            return null;

        var lunarMonthName = _calendarService.GetLunarMonthName(date);
        var lunarDayName = _calendarService.GetLunarDayName(date);

        if (string.IsNullOrWhiteSpace(lunarMonthName) ||
            string.IsNullOrWhiteSpace(lunarDayName))
            return null;

        var festival =
            LunarFestivals
                .FirstOrDefault(
                    f =>
                        f.Month == lunarMonthName &&
                        f.Day == lunarDayName);

        if (festival == null)
        {
            var nextDay = date.Date.AddDays(1);
            lunarMonthName = _calendarService.GetLunarMonthName(nextDay);
            lunarDayName = _calendarService.GetLunarDayName(nextDay);

            if (lunarMonthName == "正月" && lunarDayName == "初一")
                return "除夕";
        }

        return festival?.Name;
    }

    /// <summary>
    /// 获取指定日期的公历节日
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetFestival(DateTime date)
    {
        var festival =
            Festivals
                .FirstOrDefault(
                    f =>
                        f.Month == date.Month &&
                        f.Day == date.Day);

        return festival?.Name;
    }
}
