using ReactiveUI;
using WeatherCalendar.ViewModels;

namespace WeatherCalendar.Services;

public class ChineseZodiacFontService : IChineseZodiacService
{
    public ReactiveObject GetChineseZodiacViewModel(string chineseZodiac)
    {
        return new ChineseZodiacFontViewModel { ChineseZodiac = chineseZodiac };
    }
}
