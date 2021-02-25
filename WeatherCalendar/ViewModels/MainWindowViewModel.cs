using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Reactive;
using WeatherCalendar.Services;
using WeatherCalendar.Utils;

namespace WeatherCalendar.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        [Reactive]
        public CalendarBaseViewModel CurrentViewModel { get; set; }

        [Reactive]
        public bool IsAutoStart { get; set; }

        public ReactiveCommand<Unit, Unit> SwitchAutoStartCommand;

        public MainWindowViewModel()
        {
            CurrentViewModel = new MainViewModel();

            var appConfigService = Locator.Current.GetService<AppConfigService>();
            IsAutoStart = appConfigService.Config.IsAutoStart;

            SwitchAutoStartCommand = ReactiveCommand.Create(() =>
            {
                if (!AppHelper.SetAutoStart(!IsAutoStart))
                    return;

                IsAutoStart = !IsAutoStart;
                appConfigService.Config.IsAutoStart = IsAutoStart;
                appConfigService.Save();
            });
        }
    }
}
