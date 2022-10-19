using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Reactive.Linq;
using Weather;
using WeatherCalendar.Models;
using WeatherCalendar.Services;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.ViewModels;

public class MainViewModel : CalendarBaseViewModel
{
    /// <summary>
    /// 日历
    /// </summary>
    public CalendarViewModel Calendar { get; }

    /// <summary>
    /// 上下班倒计时
    /// </summary>
    public WorkTimerViewModel WorkTimer { get; }

    /// <summary>
    /// 当前时间
    /// </summary>
    [ObservableAsProperty]
    public DateTime CurrentDateTime { get; }

    /// <summary>
    /// 干支年名称（正月）
    /// </summary>
    [ObservableAsProperty]
    public string StemsAndBranchesYearNameOfFirstMonth { get; }

    /// <summary>
    /// 干支年名称（立春）
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
    /// 干支月
    /// </summary>
    [ObservableAsProperty]
    public string StemsAndBranchesMonthName { get; }

    /// <summary>
    /// 干支日
    /// </summary>
    [ObservableAsProperty]
    public string StemsAndBranchesDayName { get; }

    /// <summary>
    /// 农历月信息
    /// </summary>
    [ObservableAsProperty]
    public string LunarMonthInfo { get; }

    /// <summary>
    /// 天气预报
    /// </summary>
    [ObservableAsProperty]
    public WeatherForecast Forecast { get; }

    /// <summary>
    /// 天气图片视图模型
    /// </summary>
    [ObservableAsProperty]
    public ReactiveObject WeatherImageViewModel { get; }

    /// <summary>
    /// 网络信息
    /// </summary>
    [ObservableAsProperty]
    public NetWorkInfo NetWorkInfo { get; }

    /// <summary>
    /// CPU使用率
    /// </summary>
    [ObservableAsProperty]
    public float CpuLoad { get; }

    /// <summary>
    /// 内存使用率
    /// </summary>
    [ObservableAsProperty]
    public float MemoryLoad { get; }

    /// <summary>
    /// 生肖视图模型
    /// </summary>
    [ObservableAsProperty]
    public ReactiveObject ChineseZodiacViewModel { get; }

    public MainViewModel()
    {
        Calendar = new CalendarViewModel();
        WorkTimer = new WorkTimerViewModel();

        GotoMonthCommand = Calendar.GotoMonthCommand;
        CurrentMonthCommand = Calendar.CurrentMonthCommand;
        LastMonthCommand = Calendar.LastMonthCommand;
        NextMonthCommand = Calendar.NextMonthCommand;

        var appService = Locator.Current.GetService<AppService>();

        appService
            .TimerPerSecond
            .ObserveOnDispatcher()
            .ToPropertyEx(this, model => model.CurrentDateTime);

        var calendarService = Locator.Current.GetService<CalendarService>();

        this.WhenAnyValue(x => x.CurrentDateTime)
            .Select(calendarService.GetStemsAndBranchesYearNameOfFirstMonth)
            .ToPropertyEx(this, model => model.StemsAndBranchesYearNameOfFirstMonth);

        this.WhenAnyValue(x => x.CurrentDateTime)
            .Select(calendarService.GetStemsAndBranchesYearNameOfSpringBegins)
            .ToPropertyEx(this, model => model.StemsAndBranchesYearNameOfSpringBegins);

        this.WhenAnyValue(x => x.CurrentDateTime)
            .Select(calendarService.GetChineseZodiacOfFirstMonth)
            .ToPropertyEx(this, model => model.ChineseZodiacOfFirstMonth);

        this.WhenAnyValue(x => x.CurrentDateTime)
            .Select(calendarService.GetChineseZodiacOfSpringBegins)
            .ToPropertyEx(this, model => model.ChineseZodiacOfSpringBegins);

        this.WhenAnyValue(x => x.CurrentDateTime)
            .Select(calendarService.GetStemsAndBranchesMonthName)
            .ToPropertyEx(this, model => model.StemsAndBranchesMonthName);

        this.WhenAnyValue(x => x.CurrentDateTime)
            .Select(calendarService.GetStemsAndBranchesDayName)
            .ToPropertyEx(this, model => model.StemsAndBranchesDayName);

        this.WhenAnyValue(x => x.CurrentDateTime)
            .Select(calendarService.GetLunarMonthInfo)
            .ToPropertyEx(this, model => model.LunarMonthInfo);

        var chineseZodiacService = Locator.Current.GetService<IChineseZodiacService>();

        this.WhenAnyValue(x => x.ChineseZodiacOfFirstMonth)
            .Select(chineseZodiacService.GetChineseZodiacViewModel)
            .ToPropertyEx(this, model => model.ChineseZodiacViewModel);

        var weatherService = Locator.Current.GetService<WeatherService>();

        weatherService
            .WhenAnyValue(x => x.Forecast)
            .ObserveOnDispatcher()
            .ToPropertyEx(this, model => model.Forecast);

        this.WhenAnyValue(x => x.Forecast)
            .Select(w => w?.GetCurrentWeather())
            .Select(w =>
            {
                if (w == null)
                    return null;

                var (weather, isNight) = w.Value;
                if (weather == null)
                    return null;

                return Locator.Current.GetService<IWeatherImageService>()?.GetWeatherImageViewModel(weather.Weather, isNight);
            })
            .ToPropertyEx(this, model => model.WeatherImageViewModel);

        var systemInfoService = Locator.Current.GetService<SystemInfoService>();

        systemInfoService
            .WhenAnyValue(x => x.NetWorkInfo)
            .ToPropertyEx(this, model => model.NetWorkInfo, false, RxApp.MainThreadScheduler);

        systemInfoService
            .WhenAnyValue(x => x.CpuLoad)
            .ToPropertyEx(this, model => model.CpuLoad, false, RxApp.MainThreadScheduler);

        systemInfoService
            .WhenAnyValue(x => x.MemoryLoad)
            .ToPropertyEx(this, model => model.MemoryLoad, false, RxApp.MainThreadScheduler);
    }
}
