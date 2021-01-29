using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;
using Splat;
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

            this.OneWayBind(
                    ViewModel,
                    model => model.IsCurrentDay,
                    view => view.Border.Background,
                    isCurrentDay =>
                    {
                        var theme = Locator.Current.GetService<ITheme>();
                        return isCurrentDay ? theme.DayViewCurrentDayBackground : theme.DayViewBackground;
                    })
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
        }
    }
}
