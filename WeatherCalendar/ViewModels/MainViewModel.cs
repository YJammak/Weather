using System;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Weather;
using WeatherCalendar.Services;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        /// <summary>
        /// 日历
        /// </summary>
        public CalendarViewModel Calendar { get; }

        /// <summary>
        /// 当前时间
        /// </summary>
        [ObservableAsProperty]
        public DateTime CurrentDateTime { get; }

        /// <summary>
        /// 干支年名称（正月）
        /// </summary>
        [ObservableAsProperty]
        public string StemsAndBranchesYearNameOfFirstMonth { get; }

        /// <summary>
        /// 干支年名称（立春）
        /// </summary>
        [ObservableAsProperty]
        public string StemsAndBranchesYearNameOfSpringBegins { get; }
        
        /// <summary>
        /// 生肖（正月）
        /// </summary>
        [ObservableAsProperty]
        public string ChineseZodiacOfFirstMonth { get; }
        
        /// <summary>
        /// 生肖（立春）
        /// </summary>
        [ObservableAsProperty]
        public string ChineseZodiacOfSpringBegins { get; }

        /// <summary>
        /// 干支月
        /// </summary>
        [ObservableAsProperty]
        public string StemsAndBranchesMonthName { get; }
        
        /// <summary>
        /// 干支日
        /// </summary>
        [ObservableAsProperty]
        public string StemsAndBranchesDayName { get; }
        
        /// <summary>
        /// 农历月信息
        /// </summary>
        [ObservableAsProperty]
        public string LunarMonthInfo { get; }
        
        /// <summary>
        /// 天气预报
        /// </summary>
        [ObservableAsProperty]
        public WeatherForecast Forecast { get; }

        public MainViewModel()
        {
            Calendar = new CalendarViewModel();

            Observable
                .Timer(
                    TimeSpan.FromSeconds(0), 
                    TimeSpan.FromSeconds(1),
                    RxApp.MainThreadScheduler)
                .Select(_ => DateTime.Now)
                .ToPropertyEx(this, model => model.CurrentDateTime);

            var calendarService = Locator.Current.GetService<CalendarService>();

            this.WhenAnyValue(x => x.CurrentDateTime)
                .Select(calendarService.GetStemsAndBranchesYearNameOfFirstMonth)
                .ToPropertyEx(this, model => model.StemsAndBranchesYearNameOfFirstMonth);

            this.WhenAnyValue(x => x.CurrentDateTime)
                .Select(calendarService.GetStemsAndBranchesYearNameOfSpringBegins)
                .ToPropertyEx(this, model => model.StemsAndBranchesYearNameOfSpringBegins);

            this.WhenAnyValue(x => x.CurrentDateTime)
                .Select(calendarService.GetChineseZodiacOfFirstMonth)
                .ToPropertyEx(this, model => model.ChineseZodiacOfFirstMonth);

            this.WhenAnyValue(x => x.CurrentDateTime)
                .Select(calendarService.GetChineseZodiacOfSpringBegins)
                .ToPropertyEx(this, model => model.ChineseZodiacOfSpringBegins);

            this.WhenAnyValue(x => x.CurrentDateTime)
                .Select(calendarService.GetStemsAndBranchesMonthName)
                .ToPropertyEx(this, model => model.StemsAndBranchesMonthName);

            this.WhenAnyValue(x => x.CurrentDateTime)
                .Select(calendarService.GetStemsAndBranchesDayName)
                .ToPropertyEx(this, model => model.StemsAndBranchesDayName);

            this.WhenAnyValue(x => x.CurrentDateTime)
                .Select(calendarService.GetLunarMonthInfo)
                .ToPropertyEx(this, model => model.LunarMonthInfo);

            var weatherService = Locator.Current.GetService<WeatherService>();

            weatherService
                .WhenAnyValue(x => x.Forecast)
                .ToPropertyEx(this, model => model.Forecast);
        }
    }
}
