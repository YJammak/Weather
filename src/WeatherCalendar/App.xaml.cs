using Splat;
using System.Windows;
using WeatherCalendar.Services;

namespace WeatherCalendar;

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
