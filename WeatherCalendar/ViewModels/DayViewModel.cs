using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Reactive.Linq;
using Weather;
using WeatherCalendar.Models;
using WeatherCalendar.Services;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.ViewModels
{
    public class DayViewModel : ReactiveObject
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        [Reactive]
        public bool IsValid { get; set; }

        /// <summary>
        /// 日期信息
        /// </summary>
        [Reactive]
        public DateInfo Date { get; set; }

        /// <summary>
        /// 是否为当日
        /// </summary>
        [ObservableAsProperty]
        public bool IsCurrentDay { get; }

        /// <summary>
        /// 是否为当前月
        /// </summary>
        [Reactive]
        public bool IsCurrentPageMonth { get; set; }

        /// <summary>
        /// 天气信息
        /// </summary>
        [Reactive]
        public ForecastInfo Forecast { get; set; }

        /// <summary>
        /// 白天天气图片视图模型
        /// </summary>
        [ObservableAsProperty]
        public ReactiveObject DayWeatherImageViewModel { get; }

        /// <summary>
        /// 夜间天气图片视图模型
        /// </summary>
        [ObservableAsProperty]
        public ReactiveObject NightWeatherImageViewModel { get; }

        /// <summary>
        /// 公历日期
        /// </summary>
        [ObservableAsProperty]
        public string DayName { get; }

        /// <summary>
        /// 农历日期
        /// </summary>
        [ObservableAsProperty]
        public string LunarDayName { get; }

        /// <summary>
        /// 节气
        /// </summary>
        [ObservableAsProperty]
        public string SolarTermName { get; }

        /// <summary>
        /// 节假日
        /// </summary>
        [ObservableAsProperty]
        public string FestivalName { get; }

        /// <summary>
        /// 是否为中国节假日
        /// </summary>
        [ObservableAsProperty]
        public bool IsChineseFestival { get; }

        /// <summary>
        /// 是否为周末
        /// </summary>
        [ObservableAsProperty]
        public bool IsWeekend { get; }

        public DayViewModel()
        {
            Date = new DateInfo();

            this.WhenAnyValue(x => x.Date.Date)
                .Select(d => d.Date == DateTime.Today)
                .ToPropertyEx(this, model => model.IsCurrentDay);

            this.WhenAnyValue(x => x.Date.Date)
                .Select(d => d.Day.ToString())
                .ToPropertyEx(this, model => model.DayName);

            this.WhenAnyValue(x => x.Date.Date)
                .Select(d => d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                .ToPropertyEx(this, model => model.IsWeekend);

            this.WhenAnyValue(
                    x => x.Date.LunarDayName,
                    x => x.Date.LunarMonthName,
                    x => x.Date.LunarMonthSizeFlag,
                    x => x.Date.LunarLeapMonthFlag,
                    (lunarDayName,
                        lunarMonthName,
                        lunarMonthSizeFlag,
                        lunarLeapMonthFlag) =>
                    {
                        if (lunarDayName == "初一")
                            return $"{lunarLeapMonthFlag}{lunarMonthName}{lunarMonthSizeFlag}";

                        return lunarDayName;
                    })
                .ToPropertyEx(this, model => model.LunarDayName);

            this.WhenAnyValue(
                    x => x.Date.SolarTerm,
                    x => x.Date.ShuJiuOrDogDays,
                    (solarTerm, shuJiuOrDogDays) =>
                    {
                        if (!string.IsNullOrWhiteSpace(solarTerm))
                            return solarTerm;

                        if (!string.IsNullOrWhiteSpace(shuJiuOrDogDays))
                            return shuJiuOrDogDays;

                        return "";
                    })
                .ToPropertyEx(this, model => model.SolarTermName);

            this.WhenAnyValue(
                    x => x.Date.ChineseFestival,
                    x => x.Date.Festival,
                    (chineseFestival, festival) =>
                    {
                        if (!string.IsNullOrWhiteSpace(chineseFestival))
                            return chineseFestival;

                        return festival;
                    })
                .ToPropertyEx(this, model => model.FestivalName);

            this.WhenAnyValue(x => x.Date.ChineseFestival)
                .Select(chineseFestival => !string.IsNullOrWhiteSpace(chineseFestival))
                .ToPropertyEx(this, model => model.IsChineseFestival);

            var weatherImageService = Locator.Current.GetService<IWeatherImageService>();

            this.WhenAnyValue(x => x.Forecast)
                .Select(f => f?.DayWeather?.Weather)
                .Select(w => weatherImageService.GetWeatherImageViewModel(w))
                .ToPropertyEx(this, model => model.DayWeatherImageViewModel);

            this.WhenAnyValue(x => x.Forecast)
                .Select(f => f?.NightWeather?.Weather)
                .Select(w => weatherImageService.GetWeatherImageViewModel(w))
                .ToPropertyEx(this, model => model.NightWeatherImageViewModel);
        }

        public override string ToString()
        {
            return Date?.Date.ToString("yyyy-MM-dd");
        }
    }
}
