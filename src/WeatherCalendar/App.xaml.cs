using ReactiveUI;
using Splat;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using WeatherCalendar.Services;
using WeatherCalendar.Themes;

namespace WeatherCalendar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Locator.CurrentMutable.RegisterConstant(new AppService());

            var weatherService = new WeatherService();

            var appConfig = new AppConfigService();
            appConfig.Load("config.json");

            Task.Run(() =>
            {
                var city = weatherService.GetCities().FirstOrDefault(c => c.CityKey == appConfig.Config.CityKey);
                weatherService.UpdateWeather(city);
                weatherService.StartUpdate();
            });

            Locator.CurrentMutable.RegisterConstant(weatherService);
            Locator.CurrentMutable.RegisterLazySingleton(() => new CalendarService());

            var festivalService = new FestivalService();
            festivalService.Load("festivals.json");

            var holidayService = new HolidayFileService();
            holidayService.Load("holidays.json");

            Locator.CurrentMutable.RegisterConstant(appConfig);
            Locator.CurrentMutable.RegisterConstant(festivalService);
            Locator.CurrentMutable.RegisterConstant(holidayService, typeof(IHolidayService));
            Locator.CurrentMutable.RegisterConstant(new WeatherImageService(), typeof(IWeatherImageService));
            Locator.CurrentMutable.RegisterConstant(new ChineseZodiacFontService(), typeof(IChineseZodiacService));
            Locator.CurrentMutable.RegisterConstant(new SystemInfoService());

            var workTimerService = new WorkTimerService
            {
                IsVisible = appConfig.Config.IsShowWorkTimer,
                StartTime = appConfig.Config.WorkStartTime,
                EndTime = appConfig.Config.WorkEndTime
            };

            Locator.CurrentMutable.RegisterConstant(workTimerService);

            Locator.CurrentMutable.RegisterConstant<ITheme>(new DefaultTheme());

            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
        }
    }
}
