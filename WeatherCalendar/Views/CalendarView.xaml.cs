using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;

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
                    model => model.CurrentMonthRows,
                    view => view.UniformGrid.Rows)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel,
                    model => model.CurrentMonth,
                    view => view.DateTextBlock.Text,
                    time => time.ToString("yyyy-MM"))
                .DisposeWith(disposable);

            this.BindCommand(ViewModel!, model => model.LastMonthCommand, view => view.LastMonth)
                .DisposeWith(disposable);

            this.BindCommand(ViewModel!, model => model.NextMonthCommand, view => view.NextMonth)
                .DisposeWith(disposable);

            if (UniformGrid.Children.Count > 0)
                return;

            foreach (var mode in ViewModel.Days)
            {
                var viewFor = ViewLocator.Current.ResolveView(mode);
                if (viewFor is not UIElement element)
                    continue;

                viewFor.ViewModel = mode;

                UniformGrid.Children.Add(element);
            }
        }
    }
}
