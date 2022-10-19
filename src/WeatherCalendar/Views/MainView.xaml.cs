using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;

namespace WeatherCalendar.Views;

/// <summary>
/// MainView.xaml 的交互逻辑
/// </summary>
public partial class MainView
{
    public MainView()
    {
        InitializeComponent();

        this.WhenActivated(WhenActivated);
    }

    private void WhenActivated(CompositeDisposable disposable)
    {
        this.OneWayBind(
                ViewModel,
                model => model.Forecast,
                view => view.CityTextBlock.Text,
                forecast => forecast?.Status.City)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Forecast,
                view => view.TemperatureTextBlock.Text,
                forecast =>
                {
                    if (forecast == null)
                        return null;

                    return $"{forecast.RealTimeWeather.Temperature} ℃";
                })
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.WeatherImageViewModel,
                view => view.WeatherImageViewHost.ViewModel)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Forecast,
                view => view.WeatherTextBlock.Text,
                forecast => forecast?.GetCurrentWeather().Weather?.Weather)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Forecast,
                view => view.HumidityTextBlock.Text,
                forecast =>
                {
                    if (forecast == null)
                        return null;

                    var today =
                        forecast
                            .Forecast
                            .FirstOrDefault(
                                f =>
                                    f.DateTime.Date == DateTime.Today);

                    if (today == null)
                        return forecast.RealTimeWeather.Humidity;

                    return $"{forecast.RealTimeWeather.Humidity} ( {today.AirQualityIndex} )";
                })
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Forecast,
                view => view.WindTextBlock.Text,
                forecast => forecast?.RealTimeWeather.WindInfo)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.StemsAndBranchesYearNameOfFirstMonth,
                view => view.StemsAndBranchesYearTextBlock.Text,
                year => $"{year}年")
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.ChineseZodiacOfFirstMonth,
                view => view.ChineseZodiacTextBlock.Text,
                chineseZodiac => $"( {chineseZodiac}年 )")
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.StemsAndBranchesMonthName,
                view => view.StemsAndBranchesMonthTextBlock.Text,
                month => $"{month}月")
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.StemsAndBranchesDayName,
                view => view.StemsAndBranchesDayTextBlock.Text,
                day => $"{day}日")
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.LunarMonthInfo,
                view => view.LunarMonthInfoTextBlock.Text)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.CurrentDateTime,
                view => view.HourMinuteTextBlock.Text,
                time => time.ToString("HH:mm"))
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.CurrentDateTime,
                view => view.SecondTextBlock.Text,
                time => time.ToString("ss"))
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.CurrentDateTime,
                view => view.DateInfoTextBlock.Text,
                date => date.ToString("M月dd日  ddd"))
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Calendar,
                view => view.CalendarViewHost.ViewModel)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.WorkTimer,
                view => view.WorkTimerViewHost.ViewModel)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.CpuLoad,
                view => view.CpuLoadTextBlock.Text,
                cpuLoad => $"{cpuLoad:F0} %")
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.MemoryLoad,
                view => view.MemoryLoadTextBlock.Text,
                memoryLoad => $"{memoryLoad:F0} %")
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.NetWorkInfo,
                view => view.UploadSpeedTextBlock.Text,
                info => GetNetworkSpeed(info?.SentSpeed ?? 0))
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.NetWorkInfo,
                view => view.DownloadSpeedTextBlock.Text,
                info => GetNetworkSpeed(info?.ReceivedSpeed ?? 0))
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.ChineseZodiacViewModel,
                view => view.ChineseZodiacModelViewHost.ViewModel)
            .DisposeWith(disposable);
    }

    private static string GetNetworkSpeed(double speed)
    {
        if (speed < 0)
            speed = 0d;

        var units = new[] { "KB/s", "MB/s", "TB/s" };

        var unit = units[0];

        for (var i = 0; i < 2; i++)
        {
            if (speed >= 999.5)
            {
                speed /= 1024d;
                unit = units[i + 1];
            }
        }

        return $"{GetValue(speed)} {unit}";
    }

    private static string GetValue(double value)
    {
        return value switch
        {
            <= 0 => "0",
            < 9.5 => $"{value:F2}",
            < 99.5 => $"{value:F1}",
            _ => $"{value:F0}"
        };
    }
}
