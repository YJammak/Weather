using System.Reactive.Disposables;
using ReactiveUI;

namespace WeatherCalendar.Views
{
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
                model => model.Days,
                view => view.ListBox.ItemsSource);
        }
    }
}
