using SharpSxwnl;
using System;
using System.Collections.Concurrent;
using WeatherCalendar.Utils;

namespace WeatherCalendar.Services;

public class CalendarService
{
    private static readonly ConcurrentDictionary<DateTime, Lunar> LunarDictionary = new();

    /// <summary>
    /// 获取干支年名称（正月）
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetStemsAndBranchesYearNameOfFirstMonth(DateTime date) => GetOb(date)?.Lyear3;

    /// <summary>
    /// 获取干支年名称（立春）
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetStemsAndBranchesYearNameOfSpringBegins(DateTime date) => GetOb(date)?.Lyear2;

    /// <summary>
    /// 获取生肖（正月）
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetChineseZodiacOfFirstMonth(DateTime date) => GetOb(date)?.LShX2;

    /// <summary>
    /// 获取生肖（立春）
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetChineseZodiacOfSpringBegins(DateTime date) => GetOb(date)?.LShX1;

    /// <summary>
    /// 获取农历月名称
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetLunarMonthName(DateTime date) => GetOb(date)?.Lmc;

    /// <summary>
    /// 获取农历闰月标志（闰或空字符串）
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetLunarLeapMonthFlag(DateTime date) => GetOb(date)?.Lleap;

    /// <summary>
    /// 获取干支月名称
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetStemsAndBranchesMonthName(DateTime date) => GetOb(date)?.Lmonth2;

    /// <summary>
    /// 获取农历月大小标志
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetLunarMonthSizeFlag(DateTime date) => GetOb(date)?.Ldns;

    /// <summary>
    /// 获取农历月信息
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetLunarMonthInfo(DateTime date) => GetOb(date)?.LMouthInfo;

    /// <summary>
    /// 获取农历日名称（初一、初二等）
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetLunarDayName(DateTime date) => GetOb(date)?.Ldc;

    /// <summary>
    /// 获取干支日名称
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetStemsAndBranchesDayName(DateTime date) => GetOb(date)?.Lday2;

    /// <summary>
    /// 获取二十四节气
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetSolarTerm(DateTime date) => GetOb(date)?.Ljq;

    /// <summary>
    /// 获取三九或三伏
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetShuJiuOrDogDays(DateTime date) => ChineseAlmanac.GetSanFuShuJiuString(date);

    /// <summary>
    /// 获取数九详情
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetShuJiuDetail(DateTime date) => ChineseAlmanac.GetShuJiuInfo(date);

    /// <summary>
    /// 获取三伏详情
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetDogDaysDetail(DateTime date) => ChineseAlmanac.GetSanFuInfo(date);

    private OB GetOb(DateTime date)
    {
        if (date == DateTime.MinValue)
            return null;

        var monthDate = new DateTime(date.Year, date.Month, 1);

        if (!LunarDictionary.TryGetValue(monthDate, out var lunar))
        {
            lunar = new Lunar(monthDate);
            LunarDictionary.TryAdd(monthDate, lunar);
        }

        return lunar.GetOBOfDay(date);
    }
}
