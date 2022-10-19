using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using WeatherCalendar.Themes;

namespace WeatherCalendar.Views;

/// <summary>
/// WeatherFontView.xaml 的交互逻辑
/// </summary>
public partial class WeatherFontView
{
    public WeatherFontView()
    {
        InitializeComponent();

        this.WhenActivated(WhenActivated);
    }

    private void WhenActivated(CompositeDisposable disposable)
    {
        this.OneWayBind(
                ViewModel,
                model => model.WeatherText,
                view => view.WeatherTextBlock.Text)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.WeatherText,
                view => view.WeatherTextBlock.Foreground,
                _ =>
                {
                    var theme = Locator.Current.GetService<ITheme>();
                    return theme.WeatherIconForeground;
                })
            .DisposeWith(disposable);
    }
}
