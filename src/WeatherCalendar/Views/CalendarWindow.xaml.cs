using ReactiveUI;
using Splat;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WeatherCalendar.Services;
using WeatherCalendar.ViewModels;

namespace WeatherCalendar.Views;

/// <summary>
/// CalendarWindow.xaml 的交互逻辑
/// </summary>
public partial class CalendarWindow
{
    public CalendarWindow()
    {
        InitializeComponent();

        ViewModel = new CalendarWindowViewModel();

        this.WhenActivated(WhenActivated);
    }

    private void WhenActivated(CompositeDisposable disposable)
    {
        this.OneWayBind(
                ViewModel,
                model => model.Years,
                view => view.YearsComboBox.ItemsSource)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Months,
                view => view.MonthsComboBox.ItemsSource)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Calendar,
                view => view.CalendarViewModelViewHost.ViewModel)
            .DisposeWith(disposable);

        this.Bind(
                ViewModel,
                model => model.SelectedYear,
                view => view.YearsComboBox.SelectedValue)
            .DisposeWith(disposable);

        this.Bind(
                ViewModel,
                model => model.SelectedMonth,
                view => view.MonthsComboBox.SelectedValue)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Forecast,
                view => view.UpdateTimeTextBlock.Text,
                forecast =>
                {
                    var time = forecast?.Status?.UpdateTime;
                    return string.IsNullOrWhiteSpace(time) ? "" : $"{time}更新";
                })
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Forecast,
                view => view.CityButton.Content,
                forecast => forecast?.Status?.City)
            .DisposeWith(disposable);

        this.UpdateButton
            .Events()
            .Click
            .Do(_ =>
            {
                var weatherService = Locator.Current.GetService<WeatherService>();
                weatherService.UpdateWeather();
            })
            .Subscribe()
            .DisposeWith(disposable);

        this.CityButton
            .Events()
            .Click
            .Do(_ => ChangeWeatherCity())
            .Subscribe()
            .DisposeWith(disposable);

        this.PinButton
            .Events()
            .Click
            .Do(_ => Topmost = !Topmost)
            .Subscribe()
            .DisposeWith(disposable);

        this.BindCommand(
                ViewModel!,
                model => model.CurrentMonthCommand,
                view => view.TodayButton)
            .DisposeWith(disposable);

        this.BindCommand(
                ViewModel!,
                model => model.LastMonthCommand,
                view => view.LastMonthButton)
            .DisposeWith(disposable);

        this.BindCommand(
                ViewModel!,
                model => model.NextMonthCommand,
                view => view.NextMonthButton)
            .DisposeWith(disposable);

        this.TitleBorder
            .Events()
            .MouseLeftButtonDown
            .Do(_ => DragMove())
            .Subscribe()
            .DisposeWith(disposable);

        this.CloseButton
            .Events()
            .Click
            .Do(_ => Close())
            .Subscribe()
            .DisposeWith(disposable);

        this.YearsComboBox
            .Events()
            .SelectionChanged
            .Do(_ => this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)))
            .Subscribe()
            .DisposeWith(disposable);

        this.MonthsComboBox
            .Events()
            .SelectionChanged
            .Do(_ => this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)))
            .Subscribe()
            .DisposeWith(disposable);
    }

    private bool IsChangingCity { get; set; }
    private void ChangeWeatherCity()
    {
        if (IsChangingCity)
            return;

        IsChangingCity = true;
        var cityWindow = new SelectCityWindow();
        cityWindow.Closed += (_, _) => IsChangingCity = false;
        cityWindow.Show();
    }
}
