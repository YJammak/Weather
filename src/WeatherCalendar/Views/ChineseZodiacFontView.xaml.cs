using ReactiveUI;
using System.Reactive.Disposables;

namespace WeatherCalendar.Views;

/// <summary>
/// ChineseZodiacFontView.xaml 的交互逻辑
/// </summary>
public partial class ChineseZodiacFontView
{
    public ChineseZodiacFontView()
    {
        InitializeComponent();

        this.WhenActivated(WhenActivated);
    }

    private void WhenActivated(CompositeDisposable disposable)
    {
        this.OneWayBind(
                ViewModel,
                model => model.Text,
                view => view.ChineseZodiacTextBlock.Text)
            .DisposeWith(disposable);
    }
}
