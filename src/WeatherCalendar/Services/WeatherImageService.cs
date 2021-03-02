using ReactiveUI;
using System;
using System.IO;
using WeatherCalendar.ViewModels;

namespace WeatherCalendar.Services
{
    public class WeatherImageService : IWeatherImageService
    {
        private readonly string _weatherImagePath;

        public WeatherImageService()
        {
            _weatherImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "weathers");
        }

        public ReactiveObject GetWeatherImageViewModel(string weather)
        {
            var imageFile = Path.Combine(_weatherImagePath, $"{weather}.png");
            if (!File.Exists(imageFile))
                return null;

            return new WeatherImageViewModel { ImageFile = imageFile };
        }
    }
}
