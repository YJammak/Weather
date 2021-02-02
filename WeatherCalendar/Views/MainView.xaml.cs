using System;
using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;

namespace WeatherCalendar.Views
{
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
                    model => model.Forecast,
                    view => view.WeatherTextBlock.Text,
                    forecast => forecast?.Forecast[1].DayWeather.Weather)
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
        }
    }
}
