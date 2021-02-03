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

            var weatherService = new WeatherService();

            Task.Run(() =>
            {
                var city = weatherService.GetCities().FirstOrDefault(c => c.City == "浦东");
                weatherService.UpdateWeather(city);
                weatherService.StartUpdate();
            });

            Locator.CurrentMutable.RegisterConstant(weatherService);
            Locator.CurrentMutable.RegisterLazySingleton(() => new CalendarService());

            var festivalService = new FestivalService();
            festivalService.Load("festivals.json");

            Locator.CurrentMutable.RegisterConstant(festivalService);
            Locator.CurrentMutable.RegisterConstant(new WeatherImageService(), typeof(IWeatherImageService));
            Locator.CurrentMutable.RegisterConstant(new SystemInfoService());

            Locator.CurrentMutable.RegisterConstant<ITheme>(new DefaultTheme());

            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
        }
    }
}
