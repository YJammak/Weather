using System.Reflection;
using System.Windows;
using ReactiveUI;
using Splat;
using WeatherCalendar.Services;

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

            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
        }
    }
}
