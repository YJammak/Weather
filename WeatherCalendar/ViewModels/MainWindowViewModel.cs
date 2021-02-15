using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WeatherCalendar.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        [Reactive]
        public CalendarBaseViewModel CurrentViewModel { get; set; }

        public MainWindowViewModel()
        {
            CurrentViewModel = new MainViewModel();
        }
    }
}
