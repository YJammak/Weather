using System.Reactive.Disposables;
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
            this.OneWayBind(ViewModel, model => model.CurrentViewModel, window => window.ViewModelViewHost.ViewModel)
                .DisposeWith(disposable);
        }
    }
}
