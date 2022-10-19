using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WeatherCalendar.ViewModels;

public class WeatherFontViewModel : ReactiveObject
{
    [Reactive]
    public string WeatherText { get; set; }
}
