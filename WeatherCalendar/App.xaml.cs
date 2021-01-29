using System.Reflection;
using System.Windows;
using ReactiveUI;
using Splat;
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

            Locator.CurrentMutable.RegisterLazySingleton(() => new WeatherService());
            Locator.CurrentMutable.RegisterLazySingleton(() => new CalendarService());

            var festivalService = new FestivalService();
            festivalService.Load("festivals.json");
            
            Locator.CurrentMutable.RegisterConstant(festivalService);

            Locator.CurrentMutable.RegisterConstant<ITheme>(new DefaultTheme());
            
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
        }
    }
}
