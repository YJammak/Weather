using ReactiveUI;
using Splat;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WeatherCalendar.Themes;

namespace WeatherCalendar.Services;

public class AppService
{
    public IObservable<DateTime> TimerPerSecond { get; private set; }

    public IObservable<DateTime> TimerPerMinute { get; private set; }

    public IObservable<DateTime> TimerPerDay { get; private set; }

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
                    buffer.Count == 2 && (
                        buffer[0].Year != buffer[1].Year ||
                        buffer[0].Month != buffer[1].Month ||
                        buffer[0].Day != buffer[1].Day ||
                        buffer[0].Hour != buffer[1].Hour ||
                        buffer[0].Minute != buffer[1].Minute ||
                        buffer[0].Second != buffer[1].Second))
                .Select(buffer => buffer[1]);

        TimerPerMinute =
            TimerPerSecond
                .Buffer(2, 1)
                .Where(buffer =>
                    buffer.Count == 2 && (
                        buffer[0].Year != buffer[1].Year ||
                        buffer[0].Month != buffer[1].Month ||
                        buffer[0].Day != buffer[1].Day ||
                        buffer[0].Hour != buffer[1].Hour ||
                        buffer[0].Minute != buffer[1].Minute))
                .Select(buffer => buffer[1]);

        TimerPerDay =
            TimerPerSecond
                .Buffer(2, 1)
                .Where(buffer =>
                    buffer.Count == 2 && (
                        buffer[0].Year != buffer[1].Year ||
                        buffer[0].Month != buffer[1].Month ||
                        buffer[0].Day != buffer[1].Day))
                .Select(buffer => buffer[1]);
    }

    public void Initial()
    {
        if (IsInitialed)
            return;

        IsInitialed = true;

        InitialTimer();

        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var appConfigFile = Path.Combine(baseDirectory, "config.json");
        var appConfig = new AppConfigService();
        appConfig.Load(appConfigFile);

        Locator.CurrentMutable.RegisterLazySingleton(() => new CalendarService());

        var festivalFile = Path.Combine(baseDirectory, "festivals.json");
        var festivalService = new FestivalService();
        festivalService.Load(festivalFile);

        var holidaysFile = Path.Combine(baseDirectory, "holidays.json");
        var holidayService = new HolidayFileService();
        holidayService.Load(holidaysFile);

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
        weatherService!.UpdateWeather();
    }
}
