using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using System.Windows;
using WeatherCalendar.Themes;

namespace WeatherCalendar.Views;

/// <summary>
/// CalendarView.xaml 的交互逻辑
/// </summary>
public partial class CalendarView
{
    public CalendarView()
    {
        InitializeComponent();

        this.WhenActivated(WhenActivated);
    }

    private void WhenActivated(CompositeDisposable disposable)
    {
        this.OneWayBind(
                ViewModel,
                model => model.CurrentMonthRows,
                view => view.UniformGrid.Rows)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.CurrentMonth,
                view => view.MonthTextBlock.Text,
                month => month.Month.ToString())
            .DisposeWith(disposable);

        this.OneWayBind(
            ViewModel,
            model => model.CurrentMonth,
            view => view.Column1TextBlock.Foreground,
            _ =>
            {
                var theme = Locator.Current.GetService<ITheme>();
                return theme.DayNameNormalForeground;
            });

        this.OneWayBind(
            ViewModel,
            model => model.CurrentMonth,
            view => view.Column2TextBlock.Foreground,
            _ =>
            {
                var theme = Locator.Current.GetService<ITheme>();
                return theme.DayNameNormalForeground;
            });

        this.OneWayBind(
            ViewModel,
            model => model.CurrentMonth,
            view => view.Column3TextBlock.Foreground,
            _ =>
            {
                var theme = Locator.Current.GetService<ITheme>();
                return theme.DayNameNormalForeground;
            });

        this.OneWayBind(
            ViewModel,
            model => model.CurrentMonth,
            view => view.Column4TextBlock.Foreground,
            _ =>
            {
                var theme = Locator.Current.GetService<ITheme>();
                return theme.DayNameNormalForeground;
            });

        this.OneWayBind(
            ViewModel,
            model => model.CurrentMonth,
            view => view.Column5TextBlock.Foreground,
            _ =>
            {
                var theme = Locator.Current.GetService<ITheme>();
                return theme.DayNameNormalForeground;
            });

        this.OneWayBind(
            ViewModel,
            model => model.CurrentMonth,
            view => view.Column6TextBlock.Foreground,
            _ =>
            {
                var theme = Locator.Current.GetService<ITheme>();
                return theme.DayNameWeekendForeground;
            });

        this.OneWayBind(
            ViewModel,
            model => model.CurrentMonth,
            view => view.Column7TextBlock.Foreground,
            _ =>
            {
                var theme = Locator.Current.GetService<ITheme>();
                return theme.DayNameWeekendForeground;
            });

        this.OneWayBind(
            ViewModel,
            model => model.CurrentMonth,
            view => view.MonthTextBlock.Foreground,
            _ =>
            {
                var theme = Locator.Current.GetService<ITheme>();
                return theme.BackgroundMonthForeground;
            });

        this.OneWayBind(
            ViewModel,
            model => model.CurrentMonth,
            view => view.MonthTextBlock.Opacity,
            _ =>
            {
                var theme = Locator.Current.GetService<ITheme>();
                return theme.BackgroundMonthOpacity;
            });

        if (UniformGrid.Children.Count > 0)
            return;

        foreach (var mode in ViewModel!.Days)
        {
            var viewFor = ViewLocator.Current.ResolveView(mode);
            if (viewFor is not UIElement element)
                continue;

            viewFor.ViewModel = mode;

            UniformGrid.Children.Add(element);
        }
    }
}
