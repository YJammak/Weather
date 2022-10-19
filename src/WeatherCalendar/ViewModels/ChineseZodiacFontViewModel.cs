using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.ViewModels;

public class ChineseZodiacFontViewModel : ReactiveObject
{
    [Reactive]
    public string ChineseZodiac { get; set; }

    [ObservableAsProperty]
    public string Text { get; }

    public ChineseZodiacFontViewModel()
    {
        this.WhenAnyValue(x => x.ChineseZodiac)
            .Select(GetText)
            .ToPropertyEx(this, model => model.Text);
    }

    private string GetText(string chineseZodiac)
    {
        return chineseZodiac switch
        {
            "鼠" => "\ue663",
            "牛" => "\ue66a",
            "虎" => "\ue668",
            "兔" => "\ue669",
            "龙" => "\ue661",
            "蛇" => "\ue662",
            "马" => "\ue665",
            "羊" => "\ue66b",
            "猴" => "\ue660",
            "鸡" => "\ue666",
            "狗" => "\ue667",
            "猪" => "\ue664",
            _ => ""
        };
    }
}
