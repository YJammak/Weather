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

            var appService = new AppService();
            Locator.CurrentMutable.RegisterConstant(appService);

            appService.Initial();
        }
    }
}
