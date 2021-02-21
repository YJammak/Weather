using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WeatherCalendar.Services;
using WeatherCalendar.Utils;
using WeatherCalendar.ViewModels;
using WeatherCalendar.Views;
using EventExtensions = System.Windows.Controls.EventExtensions;
using ITheme = WeatherCalendar.Themes.ITheme;

namespace WeatherCalendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            this.WhenActivated(WhenActivated);
        }

        private void WhenActivated(CompositeDisposable disposable)
        {
            var appConfigService = Locator.Current.GetService<AppConfigService>();
            var theme = Locator.Current.GetService<ITheme>();

            this.MousePenetrationMenuItem.IsChecked = appConfigService.Config.IsMousePenetrate;
            this.LockedPositionMenuItem.IsChecked = appConfigService.Config.IsLockedPosition;
            this.BackgroundTransparentMenuItem.IsChecked = appConfigService.Config.IsBackgroundTransparent;
            this.Left = appConfigService.Config.WindowLeft;
            this.Top = appConfigService.Config.WindowTop;

            this.SetWindowCanPenetrate(appConfigService.Config.IsMousePenetrate);
            this.SetWindowToolWindow();
            this.SetWindowBottom();

            this.MainGrid.Background =
                appConfigService.Config.IsBackgroundTransparent ?
                    new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)) :
                    theme.MainWindowBackground;

            this.OneWayBind(
                    ViewModel,
                    model => model.CurrentViewModel,
                    window => window.ViewModelViewHost.ViewModel)
                .DisposeWith(disposable);

            this.NotifyIcon
                .LeftClickCommand = ReactiveCommand.Create(ShowCalendarWindow);

            this.CalendarDetailMenuItem
                .Events()
                .Click
                .Do(_ => ShowCalendarWindow())
                .Subscribe()
                .DisposeWith(disposable);

            this.MousePenetrationMenuItem
                .Events()
                .Checked
                .Do(_ =>
                {
                    this.SetWindowCanPenetrate(true);
                    appConfigService.Config.IsMousePenetrate = true;
                    appConfigService.Save();
                })
                .Subscribe()
                .DisposeWith(disposable);

            this.MousePenetrationMenuItem
                .Events()
                .Unchecked
                .Do(_ =>
                {
                    this.SetWindowCanPenetrate(false);
                    appConfigService.Config.IsMousePenetrate = false;
                    appConfigService.Save();
                })
                .Subscribe()
                .DisposeWith(disposable);

            this.LockedPositionMenuItem
                .Events()
                .Checked
                .Do(_ =>
                {
                    appConfigService.Config.IsLockedPosition = true;
                    appConfigService.Save();
                })
                .Subscribe()
                .DisposeWith(disposable);

            this.LockedPositionMenuItem
                .Events()
                .Unchecked
                .Do(_ =>
                {
                    appConfigService.Config.IsLockedPosition = false;
                    appConfigService.Save();
                })
                .Subscribe()
                .DisposeWith(disposable);

            this.BackgroundTransparentMenuItem
                .Events()
                .Checked
                .Do(_ =>
                {
                    MainGrid.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
                    appConfigService.Config.IsBackgroundTransparent = true;
                    appConfigService.Save();
                })
                .Subscribe()
                .DisposeWith(disposable);

            this.BackgroundTransparentMenuItem
                .Events()
                .Unchecked
                .Do(_ =>
                {
                    MainGrid.Background = theme.MainWindowBackground;
                    appConfigService.Config.IsBackgroundTransparent = false;
                    appConfigService.Save();
                })
                .Subscribe()
                .DisposeWith(disposable);

            this.UpdateWeatherMenuItem
                .Events()
                .Click
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Do(_ =>
                {
                    var weatherService = Locator.Current.GetService<WeatherService>();
                    weatherService.UpdateWeather();
                })
                .Subscribe()
                .DisposeWith(disposable);

            this.ChangeCityMenuItem
                .Events()
                .Click
                .Do(_ => ChangeWeatherCity())
                .Subscribe()
                .DisposeWith(disposable);

            this.CurrentMonthMenuItem
                .Events()
                .Click
                .Select(_ => Unit.Default)
                .InvokeCommand(this,
                    window => window.ViewModel.CurrentViewModel.CurrentMonthCommand)
                .DisposeWith(disposable);

            this.LastMonthMenuItem
                .Events()
                .Click
                .Select(_ => Unit.Default)
                .InvokeCommand(this,
                    window => window.ViewModel.CurrentViewModel.LastMonthCommand)
                .DisposeWith(disposable);

            this.NextMonthMenuItem
                .Events()
                .Click
                .Select(_ => Unit.Default)
                .InvokeCommand(this,
                    window => window.ViewModel.CurrentViewModel.NextMonthCommand)
                .DisposeWith(disposable);

            this.QuitMenuItem
                .Events()
                .Click
                .Do(_ => Close())
                .Subscribe();

            EventExtensions.Events(this)
                .MouseLeftButtonDown
                .Do(_ =>
                {
                    if (!appConfigService.Config.IsLockedPosition)
                        DragMove();
                })
                .Subscribe()
                .DisposeWith(disposable);

            EventExtensions.Events(this)
                .MouseLeftButtonUp
                .Do(_ =>
                {
                    var left = (int)Left;
                    var top = (int)Top;
                    if (left != appConfigService.Config.WindowLeft ||
                        top != appConfigService.Config.WindowTop)
                    {
                        appConfigService.Config.WindowLeft = left;
                        appConfigService.Config.WindowTop = top;
                        appConfigService.Save();
                    }
                })
                .Subscribe()
                .DisposeWith(disposable);
        }

        private CalendarWindow CalendarWindow { get; set; }
        private void ShowCalendarWindow()
        {
            if (CalendarWindow == null)
            {
                CalendarWindow = new CalendarWindow();
                CalendarWindow.Closed += (_, _) => CalendarWindow = null;
            }

            if (CalendarWindow.WindowState == WindowState.Minimized)
                CalendarWindow.WindowState = WindowState.Normal;

            if (!CalendarWindow.IsVisible)
                CalendarWindow.Show();

            CalendarWindow.Activate();
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
}
