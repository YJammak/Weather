using ReactiveUI;

namespace WeatherCalendar.Services;

public interface IChineseZodiacService
{
    public ReactiveObject GetChineseZodiacViewModel(string chineseZodiac);
}
