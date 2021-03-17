using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WeatherCalendar.Themes;

namespace WeatherCalendar.Services
{
    public class AppService
    {
        public IObservable<DateTime> TimerPerSecond { get; private set; }

        public IObservable<DateTime> TimerPerMinute { get; private set; }

        public bool IsInitialed { get; private set; }

        private void InitialTimer()
        {
            var timer =
                Observable
                    .Timer(DateTimeOffset.Now, TimeSpan.FromMilliseconds(10))
                    .Select(_ => DateTime.Now)
                    .SubscribeOn(RxApp.TaskpoolScheduler);

            TimerPerSecond =
                timer.Buffer(2, 1)
                    .Where(buffer =>
                        buffer.Count == 2 &&
                        buffer[0].Second != buffer[1].Second)
                    .Select(buffer => buffer[1]);

            TimerPerMinute =
                TimerPerSecond
                    .Buffer(2, 1)
                    .Where(buffer =>
                        buffer.Count == 2 &&
                        buffer[0].Minute != buffer[1].Minute)
                    .Select(buffer => buffer[1]);
        }

        public void Initial()
        {
            if (IsInitialed)
                return;

            IsInitialed = true;

            InitialTimer();

            var appConfig = new AppConfigService();
            appConfig.Load("config.json");

            Locator.CurrentMutable.RegisterLazySingleton(() => new CalendarService());

            var festivalService = new FestivalService();
            festivalService.Load("festivals.json");

            var holidayService = new HolidayFileService();
            holidayService.Load("holidays.json");

            Locator.CurrentMutable.RegisterConstant(appConfig);
            Locator.CurrentMutable.RegisterConstant(festivalService);
            Locator.CurrentMutable.RegisterConstant(holidayService, typeof(IHolidayService));
            Locator.CurrentMutable.RegisterConstant(new ChineseZodiacFontService(), typeof(IChineseZodiacService));
            Locator.CurrentMutable.RegisterConstant(new SystemInfoService());

            var workTimerService = new WorkTimerService
            {
                IsVisible = appConfig.Config.IsShowWorkTimer,
                StartTime = appConfig.Config.WorkStartTime,
                EndTime = appConfig.Config.WorkEndTime
            };

            Locator.CurrentMutable.RegisterConstant(workTimerService);

            var weatherService = new WeatherService();

            Task.Run(() =>
            {
                var city = weatherService.GetCities().FirstOrDefault(c => c.CityKey == appConfig.Config.CityKey);
                weatherService.UpdateWeather(city);
                weatherService.StartUpdate();
            });

            Locator.CurrentMutable.RegisterConstant(weatherService);

            LoadWeatherIcon(appConfig.Config.IsCustomWeatherIcon, appConfig.Config.CustomWeatherIconPath);

            Locator.CurrentMutable.RegisterConstant<ITheme>(new DefaultTheme());

            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
        }

        public void LoadWeatherIcon(bool isCustomIcon, string iconPath)
        {
            var currentService = Locator.Current.GetService<IWeatherImageService>();

            if (isCustomIcon)
            {
                if (string.IsNullOrWhiteSpace(iconPath))
                    return;

                if (currentService is WeatherImageService image && image.WeatherImagePath == iconPath)
                    return;

                Locator.CurrentMutable.UnregisterAll(typeof(IWeatherImageService));
                Locator.CurrentMutable.RegisterConstant(new WeatherImageService(iconPath), typeof(IWeatherImageService));
            }
            else
            {
                if (currentService is WeatherFontService)
                    return;

                Locator.CurrentMutable.UnregisterAll(typeof(IWeatherImageService));
                Locator.CurrentMutable.RegisterConstant(new WeatherFontService(), typeof(IWeatherImageService));
            }

            var weatherService = Locator.Current.GetService<WeatherService>();
            weatherService.UpdateWeather();
        }
    }
}
