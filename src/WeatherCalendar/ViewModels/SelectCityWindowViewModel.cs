using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Weather;
using WeatherCalendar.Services;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.ViewModels;

public class SelectCityWindowViewModel : ReactiveObject
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
    /// 更新天气命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> UpdateWeatherCommand;

    /// <summary>
    /// 更新成功
    /// </summary>
    public Interaction<Unit, Unit> UpdateSuccessInteraction { get; }

    public SelectCityWindowViewModel()
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

        var canUpdateWeatherCommandExecute =
            this.WhenAnyValue(x => x.SelectedCityInfo)
                .Select(c => c != null);

        UpdateWeatherCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (SelectedCityInfo == null)
                return;

#pragma warning disable 4014
            Task.Run(() => weatherService.UpdateWeather(SelectedCityInfo));
#pragma warning restore 4014

            var appConfigService = Locator.Current.GetService<AppConfigService>();
            appConfigService.Config.CityKey = SelectedCityInfo?.CityKey;
            appConfigService.Save();

            await UpdateSuccessInteraction.Handle(Unit.Default);
        }, canUpdateWeatherCommandExecute);

        UpdateSuccessInteraction = new Interaction<Unit, Unit>();
    }
}
