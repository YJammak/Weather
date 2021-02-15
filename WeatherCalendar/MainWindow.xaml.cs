using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using WeatherCalendar.ViewModels;

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
            this.OneWayBind(
                    ViewModel, 
                    model => model.CurrentViewModel, 
                    window => window.ViewModelViewHost.ViewModel)
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
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
