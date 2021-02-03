using ReactiveUI;
using Splat;
using System;
using System.Globalization;
using System.Reactive.Disposables;
using System.Windows;
using WeatherCalendar.Models;
using WeatherCalendar.Themes;

namespace WeatherCalendar.Views
{
    /// <summary>
    /// DayView.xaml 的交互逻辑
    /// </summary>
    public partial class DayView
    {
        public DayView()
        {
            InitializeComponent();

            this.WhenActivated(WhenActivated);
        }

        private void WhenActivated(CompositeDisposable disposable)
        {
            this.OneWayBind(
                    ViewModel,
                    model => model.IsValid,
                    view => view.Visibility,
                    isValid => isValid ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.DayName,
                    view => view.DayTextBlock.Text)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.LunarDayName,
                    view => view.LunarTextBlock.Text)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.SolarTermName,
                    view => view.SolarTermTextBlock.Text)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.FestivalName,
                    view => view.FestivalTextBlock.Text)
                .DisposeWith(disposable);

            this.WhenAnyValue(
                    x => x.ViewModel.SolarTermName,
                    x => x.ViewModel.FestivalName,
                    (solarTermName, festivalName) =>
                    {
                        if (string.IsNullOrWhiteSpace(solarTermName) &&
                            string.IsNullOrWhiteSpace(festivalName))
                            return Visibility.Visible;

                        return Visibility.Collapsed;
                    })
                .BindTo(this, view => view.LunarTextBlock.Visibility)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.SolarTermName,
                    view => view.SolarTermTextBlock.Visibility,
                    solarTermName =>
                        string.IsNullOrWhiteSpace(solarTermName) ? Visibility.Collapsed : Visibility.Visible)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.FestivalName,
                    view => view.FestivalTextBlock.Visibility,
                    festivalName =>
                        string.IsNullOrWhiteSpace(festivalName) ? Visibility.Collapsed : Visibility.Visible)
                .DisposeWith(disposable);

            this.WhenAnyValue(
                    x => x.IsMouseOver,
                    x => x.ViewModel.IsCurrentDay,
                    (isMouseOver, isCurrentDay) =>
                    {
                        var theme = Locator.Current.GetService<ITheme>();

                        if (isMouseOver)
                            return theme.DayViewMouseOverDayBackground;

                        if (isCurrentDay)
                            return theme.DayViewCurrentDayBackground;

                        return theme.DayViewBackground;
                    })
                .BindTo(this, view => view.Border.Background)
                .DisposeWith(disposable);

            this.WhenAnyValue(
                    x => x.ViewModel.IsCurrentPageMonth,
                    x => x.ViewModel.IsWeekend,
                    (isCurrentMonth, isWeekend) =>
                    {
                        var theme = Locator.Current.GetService<ITheme>();
                        if (!isCurrentMonth)
                            return theme.DayNameAnotherMonthForeground;

                        if (isWeekend)
                            return theme.DayNameWeekendForeground;

                        return theme.DayNameNormalForeground;
                    })
                .BindTo(this, view => view.DayTextBlock.Foreground)
                .DisposeWith(disposable);

            this.WhenAnyValue(
                    x => x.ViewModel.IsCurrentPageMonth,
                    x => x.ViewModel.IsWeekend,
                    (isCurrentMonth, isWeekend) =>
                    {
                        var theme = Locator.Current.GetService<ITheme>();
                        if (!isCurrentMonth)
                            return theme.LunarDayAnotherMonthForeground;

                        if (isWeekend)
                            return theme.LunarDayWeekendForeground;

                        return theme.LunarDayNormalForeground;
                    })
                .BindTo(this, view => view.LunarTextBlock.Foreground)
                .DisposeWith(disposable);

            this.WhenAnyValue(
                    x => x.ViewModel.IsCurrentPageMonth,
                    x => x.ViewModel.IsWeekend,
                    (isCurrentMonth, isWeekend) =>
                    {
                        var theme = Locator.Current.GetService<ITheme>();
                        if (!isCurrentMonth)
                            return theme.SolarTermAnotherMonthForeground;

                        if (isWeekend)
                            return theme.SolarTermWeekendForeground;

                        return theme.SolarTermNormalForeground;
                    })
                .BindTo(this, view => view.SolarTermTextBlock.Foreground)
                .DisposeWith(disposable);

            this.WhenAnyValue(
                    x => x.ViewModel.IsCurrentPageMonth,
                    x => x.ViewModel.IsWeekend,
                    x => x.ViewModel.IsChineseFestival,
                    (isCurrentMonth, isWeekend, isChineseFestival) =>
                    {
                        var theme = Locator.Current.GetService<ITheme>();
                        if (!isCurrentMonth)
                            return theme.FestivalAnotherMonthForeground;

                        if (isWeekend)
                        {
                            return isChineseFestival ?
                                theme.ChineseFestivalWeekendForeground :
                                theme.FestivalWeekendForeground;
                        }

                        return isChineseFestival ?
                            theme.ChineseFestivalNormalForeground :
                            theme.FestivalNormalForeground;
                    })
                .BindTo(this, view => view.FestivalTextBlock.Foreground)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    mode => mode.Forecast,
                    view => view.WeatherGrid.Visibility,
                    forecast => forecast == null ? Visibility.Collapsed : Visibility.Visible)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.DayWeatherImageViewModel,
                    view => view.DayWeatherImageViewHost.ViewModel)
                .DisposeWith(disposable);

            this.OneWayBind(
                ViewModel,
                model => model.NightWeatherImageViewModel,
                view => view.NightWeatherImageViewHost.ViewModel);

            this.OneWayBind(
                    ViewModel,
                    model => model.Date,
                    view => view.DaysTextBlock.Text,
                    dateInfo => GetDaysFromTodayInfo(dateInfo.Date))
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Date,
                    view => view.YearInfoTextBlock.Text,
                    GetYearInfo)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Date,
                    view => view.DateInfoTextBlock.Text,
                    GetDateInfo)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Date,
                    view => view.LunarInfoTextBlock.Text,
                    GetLunarInfo)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Forecast,
                    view => view.DayWeatherTextBlock.Text,
                    forecast => forecast?.DayWeather.Weather)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Forecast,
                    view => view.NightWeatherTextBlock.Text,
                    forecast => forecast?.NightWeather.Weather)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Forecast,
                    view => view.DayWindInfoTextBlock.Text,
                    forecast => forecast?.DayWeather.WindInfo)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Forecast,
                    view => view.NightWindInfoTextBlock.Text,
                    forecast => forecast?.NightWeather.WindInfo)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Forecast,
                    view => view.HighTemperatureTextBlock.Text,
                    forecast => $"高温 :  {forecast?.HighTemperature} ℃")
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Forecast,
                    view => view.LowTemperatureTextBlock.Text,
                    forecast => $"低温 :  {forecast?.LowTemperature} ℃")
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Forecast,
                    view => view.SunriseTextBlock.Text,
                    forecast => $"日出 :  {forecast?.Sunrise}")
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Forecast,
                    view => view.SunsetTextBlock.Text,
                    forecast => $"日落 :  {forecast?.Sunset}")
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.Forecast,
                    view => view.WeatherNoticeTextBlock.Text,
                    forecast => forecast?.DayWeather.Notice)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.DayWeatherImageViewModel,
                    view => view.TooltipDayWeatherImageViewHost.ViewModel)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.NightWeatherImageViewModel,
                    view => view.TooltipNightWeatherImageViewHost.ViewModel)
                .DisposeWith(disposable);
        }

        private static string GetLunarInfo(DateInfo dateInfo)
        {
            return $"{dateInfo.StemsAndBranchesYearNameOfFirstMonth}{dateInfo.ChineseZodiacOfFirstMonth}年 {dateInfo.StemsAndBranchesMonthName}月 {dateInfo.StemsAndBranchesDayName}日";
        }

        private static string GetDateInfo(DateInfo dateInfo)
        {
            return $"{dateInfo.Date:yyyy年M月d日} ({dateInfo.LunarMonthInfo})";
        }

        private static string GetYearInfo(DateInfo dateInfo)
        {
            var gc = new GregorianCalendar();

            var result = $"第{dateInfo.Date.DayOfYear}天 第{gc.GetWeekOfYear(dateInfo.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}周";

            var info = "";

            if (!string.IsNullOrWhiteSpace(dateInfo.SolarTerm))
                info += dateInfo.SolarTerm;

            if (!string.IsNullOrWhiteSpace(dateInfo.DogDaysDetail))
                info += string.IsNullOrWhiteSpace(info) ? dateInfo.DogDaysDetail : " " + dateInfo.DogDaysDetail;

            if (!string.IsNullOrWhiteSpace(dateInfo.ShuJiuDetail))
                info += string.IsNullOrWhiteSpace(info) ? dateInfo.ShuJiuDetail : " " + dateInfo.ShuJiuDetail;

            if (!string.IsNullOrWhiteSpace(dateInfo.Festival))
                info += string.IsNullOrWhiteSpace(info) ? dateInfo.Festival : " " + dateInfo.Festival;

            if (!string.IsNullOrWhiteSpace(dateInfo.ChineseFestival))
                info += string.IsNullOrWhiteSpace(info) ? dateInfo.ChineseFestival : " " + dateInfo.ChineseFestival;

            result += $"{(string.IsNullOrWhiteSpace(info) ? "" : $" ({info})")}";

            return result;
        }

        private static string GetDaysFromTodayInfo(DateTime date)
        {
            var days = (date.Date - DateTime.Now.Date).Days;

            return days switch
            {
                -2 => "前天",
                -1 => "昨天",
                0 => "今天",
                1 => "明天",
                2 => "后天",
                _ => $"{Math.Abs(days)}天{(days > 0 ? "后" : "前")}"
            };
        }
    }
}
