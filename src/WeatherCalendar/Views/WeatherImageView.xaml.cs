using ReactiveUI;
using System.Reactive.Disposables;

namespace WeatherCalendar.Views;

/// <summary>
/// WeatherImageView.xaml 的交互逻辑
/// </summary>
public partial class WeatherImageView
{
    public WeatherImageView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(
                    ViewModel,
                    model => model.ImageFile,
                    view => view.WeatherImage.Source)
                .DisposeWith(disposable);
        });
    }
}
