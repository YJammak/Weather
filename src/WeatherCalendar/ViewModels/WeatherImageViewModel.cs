using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WeatherCalendar.ViewModels;

public class WeatherImageViewModel : ReactiveObject
{
    [Reactive]
    public string ImageFile { get; set; }
}
