using ReactiveUI;
using System.Reactive.Disposables;

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
                model => model.Day,
                view => view.DayTextBlock.Text);

            this.OneWayBind(
                ViewModel,
                model => model.Subtitle1,
                view => view.LunarTextBlock.Text);
        }
    }
}
