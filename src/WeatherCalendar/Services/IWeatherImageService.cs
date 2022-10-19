using ReactiveUI;

namespace WeatherCalendar.Services;

public interface IWeatherImageService
{
    public ReactiveObject GetWeatherImageViewModel(string weather, bool isNight);
}
