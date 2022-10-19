using WeatherCalendar.Models;
using WeatherCalendar.Utils;

namespace WeatherCalendar.Services;

public class AppConfigService
{
    public AppConfig Config { get; private set; }

    private string File { get; set; }

    public AppConfig Load(string file)
    {
        try
        {
            Config = JsonHelper.LoadFromFileToObject<AppConfig>(file);
        }
        catch
        {
            Config = new AppConfig
            {
                CityKey = "101020600", // 浦东
            };
        }

        File = file;

        return Config;
    }

    public void Save()
    {
        try
        {
            if (Config == null)
                return;

            var json = JsonHelper.SerializeObjectToFormatJson(Config);
            System.IO.File.WriteAllText(File, json);
        }
        catch
        {
            //
        }
    }
}
