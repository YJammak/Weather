using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WeatherCalendar.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        [Reactive]
        public ReactiveObject CurrentViewModel { get; set; }

        public MainWindowViewModel()
        {
            CurrentViewModel = new MainViewModel();
        }
    }
}
