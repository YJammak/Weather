using System.Reactive.Disposables;
using ReactiveUI;

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
                model => model.Date,
                view => view.TextBlock.Text, 
                info => info.Date.ToString("s"));
        }
    }
}
