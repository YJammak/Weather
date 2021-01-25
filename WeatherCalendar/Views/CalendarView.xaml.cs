using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
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
                    model => model.CurrentPageRows, 
                    view => view.UniformGrid.Rows)
                .DisposeWith(disposable);

            this.OneWayBind(
                    ViewModel, 
                    model => model.CurrentPageDate, 
                    view => view.DateTextBlock.Text, 
                    time => time.ToString("yyyy-MM"))
                .DisposeWith(disposable);

            this.BindCommand(ViewModel!, model => model.LastMonthCommand, view => view.LastMonth)
                .DisposeWith(disposable);

            this.BindCommand(ViewModel!, model => model.NextMonthCommand, view => view.NextMonth)
                .DisposeWith(disposable);

            this.ViewModel
                .WhenAnyValue(x => x.Days)
                .Do(ds =>
                {
                    UniformGrid.Children.Clear();

                    if (ds == null)
                        return;

                    foreach (var mode in ds)
                    {
                        var viewFor = ViewLocator.Current.ResolveView(mode);
                        if (viewFor is not UIElement element)
                            continue;

                        viewFor.ViewModel = mode;

                        UniformGrid.Children.Add(element);
                    }
                })
                .Subscribe()
                .DisposeWith(disposable);
        }
    }
}
