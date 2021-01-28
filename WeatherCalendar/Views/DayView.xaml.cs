using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;

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
                    model => model.HolidayName,
                    view => view.HolidayTextBlock.Text)
                .DisposeWith(disposable);

            this.WhenAnyValue(
                    x => x.ViewModel.SolarTermName,
                    x => x.ViewModel.HolidayName,
                    (solarTermName, holidayName) =>
                    {
                        if (string.IsNullOrWhiteSpace(solarTermName) &&
                            string.IsNullOrWhiteSpace(holidayName))
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
                    model => model.HolidayName,
                    view => view.HolidayTextBlock.Visibility,
                    holidayName =>
                        string.IsNullOrWhiteSpace(holidayName) ? Visibility.Collapsed : Visibility.Visible)
                .DisposeWith(disposable);
        }
    }
}
