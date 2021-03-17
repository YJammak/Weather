using ReactiveUI;
using System;
using System.IO;
using WeatherCalendar.ViewModels;

namespace WeatherCalendar.Services
{
    public class WeatherImageService : IWeatherImageService
    {
        public string WeatherImagePath { get; }

        public WeatherImageService(string imagePath)
        {
            WeatherImagePath = imagePath;
        }

        public ReactiveObject GetWeatherImageViewModel(string weather)
        {
            var imageFile = Path.Combine(WeatherImagePath, $"{weather}.png");
            if (!File.Exists(imageFile))
                return null;

            return new WeatherImageViewModel { ImageFile = imageFile };
        }
    }
}
