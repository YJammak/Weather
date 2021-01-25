using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Weather;
using WeatherCalendar.Models;

namespace WeatherCalendar.ViewModels
{
    public class DayViewModel : ReactiveObject
    {
        [Reactive]
        public DateInfo Date { get; set; }
        
        [Reactive]
        public ForecastInfo Forecast { get; set; }

        public DayViewModel()
        {
            Date = new DateInfo();
        }

        public override string ToString()
        {
            return Date?.Date.ToString("yyyy-MM-dd");
        }
    }
}
