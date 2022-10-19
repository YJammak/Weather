using ReactiveUI;
using System.IO;
using WeatherCalendar.ViewModels;

namespace WeatherCalendar.Services;

public class WeatherImageService : IWeatherImageService
{
    public string WeatherImagePath { get; }

    public WeatherImageService(string imagePath)
    {
        WeatherImagePath = imagePath;
    }

    public ReactiveObject GetWeatherImageViewModel(string weather, bool isNight)
    {
        var imageFile = Path.Combine(WeatherImagePath, $"{weather}.png");

        if (isNight)
        {
            var imageFileOfNight = Path.Combine(WeatherImagePath, $"{weather}_夜.png");

            if (!File.Exists(imageFileOfNight))
                return File.Exists(imageFile)
                    ? new WeatherImageViewModel { ImageFile = imageFile }
                    : null;

            return new WeatherImageViewModel { ImageFile = imageFileOfNight };
        }
        else
        {
            if (!File.Exists(imageFile))
                return null;

            return new WeatherImageViewModel { ImageFile = imageFile };
        }
    }
}
