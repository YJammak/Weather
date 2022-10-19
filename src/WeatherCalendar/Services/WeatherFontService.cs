using ReactiveUI;
using WeatherCalendar.ViewModels;

namespace WeatherCalendar.Services;

public class WeatherFontService : IWeatherImageService
{
    public ReactiveObject GetWeatherImageViewModel(string weather, bool isNight)
    {
        return new WeatherFontViewModel
        {
            WeatherText = isNight
                ? GetWeatherFontStringOfNight(weather)
                : GetWeatherFontStringOfDay(weather)
        };
    }

    private static string GetWeatherFontStringOfDay(string weather)
    {
        return weather switch
        {
            "霾" => "\ue64e",
            "暴雪" => "\ue61e",
            "暴雨" => "\ue611",
            "暴雨转大暴雨" => "\ue621",
            "大暴雨" => "\ue61d",
            "大暴雨转特大暴雨" => "\ue61f",
            "大雪" => "\ue624",
            "大雪转暴雪" => "\ue620",
            "大雨" => "\ue622",
            "大雨转暴雨" => "\ue623",
            "冻雨" => "\ue626",
            "多云" => "\ue625",
            "浮尘" => "\ue6ec",
            "雷阵雨" => "\ue627",
            "雷阵雨并伴有冰雹" => "\ue628",
            "强沙尘暴" => "\ue632",
            "晴" => "\ue60b",
            "沙尘暴" => "\ue610",
            "特大暴雨" => "\ue60d",
            "雾" => "\ue60c",
            "小雪" => "\ue612",
            "小雪转中雪" => "\ue614",
            "小雨" => "\ue60f",
            "小雨转中雨" => "\ue60e",
            "扬尘" => "\ue61c",
            "阴" => "\ue613",
            "雨夹雪" => "\ue615",
            "阵雪" => "\ue619",
            "阵雨" => "\ue616",
            "中雪" => "\ue61a",
            "中雪转大雪" => "\ue61b",
            "中雨" => "\ue617",
            "中雨转大雨" => "\ue618",
            _ => ""
        };
    }

    private static string GetWeatherFontStringOfNight(string weather)
    {
        return weather switch
        {
            "霾" => "\ue64e",
            "暴雪" => "\ue61e",
            "暴雨" => "\ue611",
            "暴雨转大暴雨" => "\ue621",
            "大暴雨" => "\ue61d",
            "大暴雨转特大暴雨" => "\ue61f",
            "大雪" => "\ue624",
            "大雪转暴雪" => "\ue620",
            "大雨" => "\ue622",
            "大雨转暴雨" => "\ue623",
            "冻雨" => "\ue626",
            "多云" => "\ue9f0",
            "浮尘" => "\ue6ec",
            "雷阵雨" => "\ue627",
            "雷阵雨并伴有冰雹" => "\ue628",
            "强沙尘暴" => "\ue632",
            "晴" => "\ue62a",
            "沙尘暴" => "\ue610",
            "特大暴雨" => "\ue60d",
            "雾" => "\ue60c",
            "小雪" => "\ue612",
            "小雪转中雪" => "\ue614",
            "小雨" => "\ue60f",
            "小雨转中雨" => "\ue60e",
            "扬尘" => "\ue61c",
            "阴" => "\ue613",
            "雨夹雪" => "\ue615",
            "阵雪" => "\ue619",
            "阵雨" => "\ue616",
            "中雪" => "\ue61a",
            "中雪转大雪" => "\ue61b",
            "中雨" => "\ue617",
            "中雨转大雨" => "\ue618",
            _ => ""
        };
    }
}
