using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Weather;
using WeatherCalendar.Models;
using WeatherCalendar.Services;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.ViewModels;

public class DayViewModel : ReactiveObject
{
    /// <summary>
    /// 是否有效
    /// </summary>
    [Reactive]
    public bool IsValid { get; set; }

    /// <summary>
    /// 日期信息
    /// </summary>
    [Reactive]
    public DateInfo Date { get; set; }

    /// <summary>
    /// 是否为当日
    /// </summary>
    [Reactive]
    public bool IsCurrentDay { get; private set; }

    /// <summary>
    /// 是否为当前月
    /// </summary>
    [Reactive]
    public bool IsCurrentPageMonth { get; set; }

    /// <summary>
    /// 天气信息
    /// </summary>
    [Reactive]
    public ForecastInfo Forecast { get; set; }

    /// <summary>
    /// 生肖视图模型
    /// </summary>
    [ObservableAsProperty]
    public ReactiveObject ChineseZodiacViewModel { get; }

    /// <summary>
    /// 白天天气图片视图模型
    /// </summary>
    [ObservableAsProperty]
    public ReactiveObject DayWeatherImageViewModel { get; }

    /// <summary>
    /// 夜间天气图片视图模型
    /// </summary>
    [ObservableAsProperty]
    public ReactiveObject NightWeatherImageViewModel { get; }

    /// <summary>
    /// 公历日期
    /// </summary>
    [ObservableAsProperty]
    public string DayName { get; }

    /// <summary>
    /// 农历日期
    /// </summary>
    [ObservableAsProperty]
    public string LunarDayName { get; }

    /// <summary>
    /// 节气
    /// </summary>
    [ObservableAsProperty]
    public string SolarTermName { get; }

    /// <summary>
    /// 节假日
    /// </summary>
    [ObservableAsProperty]
    public string FestivalName { get; }

    /// <summary>
    /// 是否为中国节假日
    /// </summary>
    [ObservableAsProperty]
    public bool IsChineseFestival { get; }

    /// <summary>
    /// 是否为周末
    /// </summary>
    [ObservableAsProperty]
    public bool IsWeekend { get; }

    /// <summary>
    /// 假日名城
    /// </summary>
    [ObservableAsProperty]
    public string HolidayName { get; }

    /// <summary>
    /// 是否为休息日
    /// </summary>
    [ObservableAsProperty]
    public bool IsHolidayRestDay { get; }

    /// <summary>
    /// 是否正在编辑
    /// </summary>
    [Reactive]
    public bool IsEditing { get; set; }

    /// <summary>
    /// 编辑假日命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> EditHolidayCommand;

    /// <summary>
    /// 删除假日命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> RemoveHolidayCommand;

    /// <summary>
    /// 获取假日信息交互
    /// </summary>
    public Interaction<(string, bool), (string, bool)> GetHolidayInfoInteraction;

    /// <summary>
    /// 所有视图模型
    /// </summary>
    private static readonly List<WeakReference<DayViewModel>> AllDayViewModels = new List<WeakReference<DayViewModel>>();

    public DayViewModel()
    {
        var appService = Locator.Current.GetService<AppService>();

        AllDayViewModels.Add(new WeakReference<DayViewModel>(this));

        Date = new DateInfo();

        appService
            .TimerPerMinute
            .Select(_ => Date.Date == DateTime.Today)
            .ObserveOnDispatcher()
            .Do(isToday => IsCurrentDay = isToday)
            .Subscribe();

        this.WhenAnyValue(x => x.Date.Date)
            .Select(d => d.Date == DateTime.Today)
            .Do(isToday => IsCurrentDay = isToday)
            .Subscribe();

        this.WhenAnyValue(x => x.Date.Date)
            .Select(d => d.Day.ToString())
            .ToPropertyEx(this, model => model.DayName);

        this.WhenAnyValue(x => x.Date.Date)
            .Select(d => d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
            .ToPropertyEx(this, model => model.IsWeekend);

        this.WhenAnyValue(
                x => x.Date.LunarDayName,
                x => x.Date.LunarMonthName,
                x => x.Date.LunarMonthSizeFlag,
                x => x.Date.LunarLeapMonthFlag,
                (lunarDayName,
                    lunarMonthName,
                    lunarMonthSizeFlag,
                    lunarLeapMonthFlag) =>
                {
                    if (lunarDayName == "初一")
                        return $"{lunarLeapMonthFlag}{lunarMonthName}{lunarMonthSizeFlag}";

                    return lunarDayName;
                })
            .ToPropertyEx(this, model => model.LunarDayName);

        this.WhenAnyValue(
                x => x.Date.SolarTerm,
                x => x.Date.ShuJiuOrDogDays,
                (solarTerm, shuJiuOrDogDays) =>
                {
                    if (!string.IsNullOrWhiteSpace(solarTerm))
                        return solarTerm;

                    if (!string.IsNullOrWhiteSpace(shuJiuOrDogDays))
                        return shuJiuOrDogDays;

                    return "";
                })
            .ToPropertyEx(this, model => model.SolarTermName);

        this.WhenAnyValue(
                x => x.Date.ChineseFestival,
                x => x.Date.Festival,
                (chineseFestival, festival) =>
                {
                    if (!string.IsNullOrWhiteSpace(chineseFestival))
                        return chineseFestival;

                    return festival;
                })
            .ToPropertyEx(this, model => model.FestivalName);

        this.WhenAnyValue(x => x.Date.ChineseFestival)
            .Select(chineseFestival => !string.IsNullOrWhiteSpace(chineseFestival))
            .ToPropertyEx(this, model => model.IsChineseFestival);

        var chineseZodiacService = Locator.Current.GetService<IChineseZodiacService>();

        this.WhenAnyValue(x => x.Date.ChineseZodiacOfFirstMonth)
            .Select(chineseZodiacService.GetChineseZodiacViewModel)
            .ToPropertyEx(this, model => model.ChineseZodiacViewModel);

        this.WhenAnyValue(x => x.Forecast)
            .Select(f => f?.DayWeather?.Weather)
            .Select(w => Locator.Current.GetService<IWeatherImageService>()?.GetWeatherImageViewModel(w, false))
            .ToPropertyEx(this, model => model.DayWeatherImageViewModel);

        this.WhenAnyValue(x => x.Forecast)
            .Select(f => f?.NightWeather?.Weather)
            .Select(w => Locator.Current.GetService<IWeatherImageService>()?.GetWeatherImageViewModel(w, true))
            .ToPropertyEx(this, model => model.NightWeatherImageViewModel);

        var holidayService = Locator.Current.GetService<IHolidayService>();

        this.WhenAnyValue(
                x => x.Date.Date,
                x => x.IsValid,
                (date, _) => date)
            .Select(d =>
            {
                var holiday = holidayService.GetHoliday(d);
                return holiday?.Name;
            })
            .ToPropertyEx(this, model => model.HolidayName);

        this.WhenAnyValue(
                x => x.Date.Date,
                x => x.IsValid,
                (date, _) => date)
            .Select(d =>
            {
                var holiday = holidayService.GetHoliday(d);
                if (holiday == null)
                    return false;

                return holiday.RestDates?.Contains(d.Date) ?? false;
            })
            .ToPropertyEx(this, model => model.IsHolidayRestDay);

        var canEditHoliday =
            this.WhenAnyValue(
                x => x.IsEditing,
                isEditing => !isEditing);

        this.EditHolidayCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                SetIsEditing(true);

                var (holidayName, isRestDay) = await GetHolidayInfoInteraction.Handle((HolidayName, IsHolidayRestDay));

                if (string.IsNullOrWhiteSpace(holidayName))
                    return;

                if (!string.IsNullOrWhiteSpace(HolidayName) && HolidayName != holidayName)
                    holidayService.Remove(Date.Date.Year, HolidayName, Date.Date);

                holidayService.Add(Date.Date.Year, holidayName, Date.Date, isRestDay);
                IsValid = false;
                IsValid = true;
            }
            finally
            {
                SetIsEditing(false);
            }
        }, canEditHoliday);

        this.RemoveHolidayCommand = ReactiveCommand.Create(() =>
        {
            holidayService.Remove(Date.Date.Year, HolidayName, Date.Date);
            IsValid = false;
            IsValid = true;
        }, canEditHoliday);

        GetHolidayInfoInteraction = new Interaction<(string, bool), (string, bool)>();
    }

    ~DayViewModel()
    {
        AllDayViewModels.RemoveAll(d =>
        {
            if (d.TryGetTarget(out var day))
                return day == this;

            return false;
        });
    }

    private static void SetIsEditing(bool isEditing)
    {
        foreach (var dayViewModel in AllDayViewModels)
        {
            if (dayViewModel.TryGetTarget(out var day))
                day.IsEditing = isEditing;
        }
    }

    public override string ToString()
    {
        return Date?.Date.ToString("yyyy-MM-dd");
    }
}
