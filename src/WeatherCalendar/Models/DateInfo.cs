using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Reactive.Linq;
using WeatherCalendar.Services;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.Models;

public class DateInfo : ReactiveObject
{
    /// <summary>
    /// 公历日期
    /// </summary>
    [Reactive]
    public DateTime Date { get; set; }

    /// <summary>
    /// 干支年（正月）
    /// </summary>
    [ObservableAsProperty]
    public string StemsAndBranchesYearNameOfFirstMonth { get; }

    /// <summary>
    /// 干支年（立春）
    /// </summary>
    [ObservableAsProperty]
    public string StemsAndBranchesYearNameOfSpringBegins { get; }

    /// <summary>
    /// 生肖（正月）
    /// </summary>
    [ObservableAsProperty]
    public string ChineseZodiacOfFirstMonth { get; }

    /// <summary>
    /// 生肖（立春）
    /// </summary>
    [ObservableAsProperty]
    public string ChineseZodiacOfSpringBegins { get; }

    /// <summary>
    /// 农历月
    /// </summary>
    [ObservableAsProperty]
    public string LunarMonthName { get; }

    /// <summary>
    /// 干支月
    /// </summary>
    [ObservableAsProperty]
    public string StemsAndBranchesMonthName { get; }

    /// <summary>
    /// 农历闰月（‘闰’或空）
    /// </summary>
    [ObservableAsProperty]
    public string LunarLeapMonthFlag { get; }

    /// <summary>
    /// 农历月大小
    /// </summary>
    [ObservableAsProperty]
    public string LunarMonthSizeFlag { get; }

    /// <summary>
    /// 农历月信息
    /// </summary>
    [ObservableAsProperty]
    public string LunarMonthInfo { get; }

    /// <summary>
    /// 农历日
    /// </summary>
    [ObservableAsProperty]
    public string LunarDayName { get; }

    /// <summary>
    /// 干支日
    /// </summary>
    [ObservableAsProperty]
    public string StemsAndBranchesDayName { get; }

    /// <summary>
    /// 节气
    /// </summary>
    [ObservableAsProperty]
    public string SolarTerm { get; }

    /// <summary>
    /// 三九或三伏
    /// </summary>
    [ObservableAsProperty]
    public string ShuJiuOrDogDays { get; }

    /// <summary>
    /// 数九详情
    /// </summary>
    [ObservableAsProperty]
    public string ShuJiuDetail { get; }

    /// <summary>
    /// 三伏详情
    /// </summary>
    [ObservableAsProperty]
    public string DogDaysDetail { get; }

    /// <summary>
    /// 中国节假日
    /// </summary>
    [ObservableAsProperty]
    public string ChineseFestival { get; }

    /// <summary>
    /// 节假日
    /// </summary>
    [ObservableAsProperty]
    public string Festival { get; }

    public DateInfo()
    {
        var calendarService = Locator.Current.GetService<CalendarService>();

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetStemsAndBranchesYearNameOfFirstMonth)
            .ToPropertyEx(this, info => info.StemsAndBranchesYearNameOfFirstMonth);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetStemsAndBranchesYearNameOfSpringBegins)
            .ToPropertyEx(this, info => info.StemsAndBranchesYearNameOfSpringBegins);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetChineseZodiacOfFirstMonth)
            .ToPropertyEx(this, info => info.ChineseZodiacOfFirstMonth);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetChineseZodiacOfSpringBegins)
            .ToPropertyEx(this, info => info.ChineseZodiacOfSpringBegins);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetLunarMonthName)
            .ToPropertyEx(this, info => info.LunarMonthName);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetStemsAndBranchesMonthName)
            .ToPropertyEx(this, info => info.StemsAndBranchesMonthName);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetLunarLeapMonthFlag)
            .ToPropertyEx(this, info => info.LunarLeapMonthFlag);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetLunarMonthSizeFlag)
            .ToPropertyEx(this, info => info.LunarMonthSizeFlag);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetLunarMonthInfo)
            .ToPropertyEx(this, info => info.LunarMonthInfo);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetLunarDayName)
            .ToPropertyEx(this, info => info.LunarDayName);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetStemsAndBranchesDayName)
            .ToPropertyEx(this, info => info.StemsAndBranchesDayName);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetSolarTerm)
            .ToPropertyEx(this, info => info.SolarTerm);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetShuJiuOrDogDays)
            .ToPropertyEx(this, info => info.ShuJiuOrDogDays);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetShuJiuDetail)
            .ToPropertyEx(this, info => info.ShuJiuDetail);

        this.WhenAnyValue(x => x.Date)
            .Select(calendarService.GetDogDaysDetail)
            .ToPropertyEx(this, info => info.DogDaysDetail);

        var festivalService = Locator.Current.GetService<FestivalService>();

        this.WhenAnyValue(x => x.Date)
            .Select(festivalService.GetLunarFestival)
            .ToPropertyEx(this, info => info.ChineseFestival);

        this.WhenAnyValue(x => x.Date)
            .Select(festivalService.GetFestival)
            .ToPropertyEx(this, info => info.Festival);
    }
}
