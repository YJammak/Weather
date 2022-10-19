using ReactiveUI;
using Splat;
using System;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WeatherCalendar.Themes;

namespace WeatherCalendar.Views;

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
                model => model.Date.Date,
                view => view.DaysTextBlock.Text,
                dateInfo => GetDaysFromTodayInfo(dateInfo.Date))
            .DisposeWith(disposable);

        this.WhenAnyValue(
                x => x.ViewModel.Date.Date,
                x => x.ViewModel.HolidayName,
                x => x.ViewModel.IsHolidayRestDay,
                (date, holidayName, isRestDay) =>
                {
                    if (!string.IsNullOrWhiteSpace(holidayName))
                        return isRestDay ? $"休息({holidayName})" : $"上班({holidayName})";

                    if (date.DayOfWeek == DayOfWeek.Saturday ||
                        date.DayOfWeek == DayOfWeek.Sunday)
                        return "休息(周末)";

                    return "上班(工作日)";
                })
            .BindTo(this, view => view.DutyTextBlock.Text)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.ChineseZodiacViewModel,
                view => view.ChineseZodiacModelViewHost.ViewModel)
            .DisposeWith(disposable);

        this.WhenAnyValue(
                x => x.ViewModel.Date.Date,
                x => x.ViewModel.Date.SolarTerm,
                x => x.ViewModel.Date.DogDaysDetail,
                x => x.ViewModel.Date.ShuJiuDetail,
                x => x.ViewModel.Date.Festival,
                x => x.ViewModel.Date.ChineseFestival,
                GetYearInfo)
            .BindTo(this, view => view.YearInfoTextBlock.Text)
            .DisposeWith(disposable);

        this.WhenAnyValue(
                x => x.ViewModel.Date.Date,
                x => x.ViewModel.Date.LunarMonthInfo,
                GetDateInfo)
            .BindTo(this, view => view.DateInfoTextBlock.Text)
            .DisposeWith(disposable);

        this.WhenAnyValue(
                x => x.ViewModel.Date.StemsAndBranchesYearNameOfFirstMonth,
                x => x.ViewModel.Date.ChineseZodiacOfFirstMonth,
                x => x.ViewModel.Date.StemsAndBranchesMonthName,
                x => x.ViewModel.Date.StemsAndBranchesDayName,
                GetLunarInfo)
            .BindTo(this, view => view.LunarInfoTextBlock.Text)
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
                forecast => $"高温 :   {forecast?.HighTemperature} ℃")
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Forecast,
                view => view.LowTemperatureTextBlock.Text,
                forecast => $"低温 :   {forecast?.LowTemperature} ℃")
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Forecast,
                view => view.SunriseTextBlock.Text,
                forecast => $"日出 :   {forecast?.Sunrise}")
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Forecast,
                view => view.SunsetTextBlock.Text,
                forecast => $"日落 :   {forecast?.Sunset}")
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Forecast,
                view => view.WeatherNoticeTextBlock.Text,
                forecast => forecast?.DayWeather.Notice)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.HolidayName,
                view => view.HolidayTextBlock.Visibility,
                name => string.IsNullOrWhiteSpace(name) ? Visibility.Collapsed : Visibility.Visible)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.IsHolidayRestDay,
                view => view.HolidayTextBlock.Text,
                isRestDay => isRestDay ? "休" : "班")
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.IsHolidayRestDay,
                view => view.HolidayTextBlock.Foreground,
                isRestDay =>
                {
                    var theme = Locator.Current.GetService<ITheme>();
                    return isRestDay ? theme.HolidayRestDayForeground : theme.HolidayWorkDayForeground;
                })
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

        this.OneWayBind(
                ViewModel,
                model => model.HolidayName,
                view => view.RemoveHolidayMenuItem.Visibility,
                name => string.IsNullOrWhiteSpace(name) ? Visibility.Collapsed : Visibility.Visible)
            .DisposeWith(disposable);

        this.Border
            .Events()
            .ContextMenuOpening
            .Do(x => x.Handled = ViewModel!.IsEditing)
            .Subscribe()
            .DisposeWith(disposable);

        this.BindCommand(
                ViewModel!,
                model => model.EditHolidayCommand,
                view => view.EditHolidayMenuItem)
            .DisposeWith(disposable);

        this.BindCommand(
                ViewModel!,
                model => model.RemoveHolidayCommand,
                view => view.RemoveHolidayMenuItem)
            .DisposeWith(disposable);

        this.ViewModel
            .GetHolidayInfoInteraction
            .RegisterHandler(async interaction =>
            {
                var (holidayName, isRestDay) = interaction.Input;

                var editWindow = new EditHolidayWindow();
                editWindow.ViewModel!.HolidayName = holidayName;
                editWindow.ViewModel!.IsRestDay = isRestDay;

                editWindow.Show();

                while (editWindow.IsVisible)
                {
                    await Task.Delay(10);
                }

                if (editWindow.ViewModel!.IsConfirmed)
                    interaction.SetOutput((editWindow.ViewModel!.HolidayName, editWindow.ViewModel!.IsRestDay));
                else
                    interaction.SetOutput((string.Empty, false));
            });
    }

    private static string GetLunarInfo(
        string stemsAndBranchesYearNameOfFirstMonth,
        string chineseZodiacOfFirstMonth,
        string stemsAndBranchesMonthName,
        string stemsAndBranchesDayName)
    {
        return $"{stemsAndBranchesYearNameOfFirstMonth}{chineseZodiacOfFirstMonth}年 {stemsAndBranchesMonthName}月 {stemsAndBranchesDayName}日";
    }

    private static string GetDateInfo(DateTime date, string lunarMonthInfo)
    {
        return $"{date:yyyy年M月d日} ({lunarMonthInfo})";
    }

    private static string GetYearInfo(
        DateTime date,
        string solarTerm,
        string dogDaysDetail,
        string shuJiuDetail,
        string festival,
        string chineseFestival)
    {
        var gc = new GregorianCalendar();

        var result = $"第{date.DayOfYear}天 第{gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}周";

        var info = "";

        if (!string.IsNullOrWhiteSpace(solarTerm))
            info += solarTerm;

        if (!string.IsNullOrWhiteSpace(dogDaysDetail))
            info += string.IsNullOrWhiteSpace(info) ? dogDaysDetail : " " + dogDaysDetail;

        if (!string.IsNullOrWhiteSpace(shuJiuDetail))
            info += string.IsNullOrWhiteSpace(info) ? shuJiuDetail : " " + shuJiuDetail;

        if (!string.IsNullOrWhiteSpace(festival))
            info += string.IsNullOrWhiteSpace(info) ? festival : " " + festival;

        if (!string.IsNullOrWhiteSpace(chineseFestival))
            info += string.IsNullOrWhiteSpace(info) ? chineseFestival : " " + chineseFestival;

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
