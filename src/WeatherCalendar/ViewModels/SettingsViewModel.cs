using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Weather;
using WeatherCalendar.Services;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.ViewModels;

public class SettingsViewModel : ReactiveObject
{
    private CityKeyInfo[] AllCities { get; }

    /// <summary>
    /// 省份
    /// </summary>
    [ObservableAsProperty]
    public string[] Provinces { get; }

    /// <summary>
    /// 地区
    /// </summary>
    [ObservableAsProperty]
    public string[] Districts { get; }

    /// <summary>
    /// 城市
    /// </summary>
    [ObservableAsProperty]
    public string[] Cities { get; }

    /// <summary>
    /// 选中的省份
    /// </summary>
    [Reactive]
    public string SelectedProvince { get; set; }

    /// <summary>
    /// 选中的地区
    /// </summary>
    [Reactive]
    public string SelectedDistrict { get; set; }

    /// <summary>
    /// 选中的城市
    /// </summary>
    [Reactive]
    public string SelectedCity { get; set; }

    /// <summary>
    /// 选中的城市信息
    /// </summary>
    [ObservableAsProperty]
    public CityKeyInfo SelectedCityInfo { get; }

    /// <summary>
    /// 是否显示上下班倒计时
    /// </summary>
    [Reactive]
    public bool IsWorkTimerVisible { get; set; }

    /// <summary>
    /// 上下班倒计时开始时间
    /// </summary>
    [Reactive]
    public TimeSpan WorkTimerStartTime { get; set; }

    /// <summary>
    /// 上下班倒计时结束时间
    /// </summary>
    [Reactive]
    public TimeSpan WorkTimerEndTime { get; set; }

    /// <summary>
    /// 是否为自定义天气图标
    /// </summary>
    [Reactive]
    public bool IsCustomWeatherIcon { get; set; }

    /// <summary>
    /// 自定义天气图标路径
    /// </summary>
    [Reactive]
    public string CustomWeatherIconPath { get; set; }

    public SettingsViewModel()
    {
        var weatherService = Locator.Current.GetService<WeatherService>();

        AllCities = weatherService.GetCities();

        Observable
            .Return(
                AllCities
                    .Select(c => c.Province)
                    .Distinct()
                    .ToArray())
            .ToPropertyEx(this, model => model.Provinces);

        this.WhenAnyValue(x => x.SelectedProvince)
            .Select(p =>
                AllCities
                    .Where(c => c.Province == p)
                    .Select(c => c.District)
                    .Distinct()
                    .ToArray())
            .Do(_ => SelectedDistrict = "")
            .ToPropertyEx(this, model => model.Districts);

        this.WhenAnyValue(
                x => x.SelectedProvince,
                x => x.SelectedDistrict)
            .Select(p =>
                AllCities
                    .Where(c => c.Province == p.Item1 && c.District == p.Item2)
                    .Select(c => c.City)
                    .Distinct()
                    .ToArray())
            .Do(_ => SelectedCity = "")
            .ToPropertyEx(this, model => model.Cities);

        SelectedProvince = weatherService.City?.Province;
        SelectedDistrict = weatherService.City?.District;
        SelectedCity = weatherService.City?.City;

        this.WhenAnyValue(
                x => x.SelectedProvince,
                x => x.SelectedDistrict,
                x => x.SelectedCity,
                (province, district, city) =>
                    AllCities.FirstOrDefault(c =>
                        c.Province == province &&
                        c.District == district &&
                        c.City == city))
            .ToPropertyEx(this, model => model.SelectedCityInfo);

        this.WhenAnyValue(x => x.SelectedCityInfo)
            .Where(c => c != null)
            .Do(city =>
            {
                Task.Run(() => weatherService.UpdateWeather(city));

                Task.Run(() =>
                {
                    var configService = Locator.Current.GetService<AppConfigService>();
                    configService.Config.CityKey = city?.CityKey;
                    configService.Save();
                });
            })
            .Subscribe();

        var appService = Locator.Current.GetService<AppService>();
        var appConfigService = Locator.Current.GetService<AppConfigService>();
        var workTimerService = Locator.Current.GetService<WorkTimerService>();

        IsWorkTimerVisible = appConfigService.Config.IsShowWorkTimer;
        WorkTimerStartTime = appConfigService.Config.WorkStartTime;
        WorkTimerEndTime = appConfigService.Config.WorkEndTime;
        IsCustomWeatherIcon = appConfigService.Config.IsCustomWeatherIcon;
        CustomWeatherIconPath = appConfigService.Config.CustomWeatherIconPath;

        this.WhenAnyValue(x => x.IsWorkTimerVisible)
            .Do(isVisible =>
            {
                workTimerService.IsVisible = isVisible;

                appConfigService.Config.IsShowWorkTimer = isVisible;
                appConfigService.Save();
            })
            .Subscribe();

        this.WhenAnyValue(x => x.WorkTimerStartTime)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Do(time =>
            {
                workTimerService.StartTime = time;

                appConfigService.Config.WorkStartTime = time;
                appConfigService.Save();
            })
            .Subscribe();

        this.WhenAnyValue(x => x.WorkTimerEndTime)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Do(time =>
            {
                workTimerService.EndTime = time;

                appConfigService.Config.WorkEndTime = time;
                appConfigService.Save();
            })
            .Subscribe();

        this.WhenAnyValue(x => x.IsCustomWeatherIcon)
            .Do(isCustom =>
            {
                appService.LoadWeatherIcon(isCustom, CustomWeatherIconPath);

                appConfigService.Config.IsCustomWeatherIcon = isCustom;
                appConfigService.Save();
            })
            .Subscribe();

        this.WhenAnyValue(x => x.CustomWeatherIconPath)
            .Do(path =>
            {
                appService.LoadWeatherIcon(IsCustomWeatherIcon, path);

                appConfigService.Config.CustomWeatherIconPath = path;
                appConfigService.Save();
            })
            .Subscribe();
    }
}
