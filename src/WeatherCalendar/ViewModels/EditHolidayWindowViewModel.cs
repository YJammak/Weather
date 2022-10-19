using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Reactive.Linq;

namespace WeatherCalendar.ViewModels;

public class EditHolidayWindowViewModel : ReactiveObject
{
    /// <summary>
    /// 假日名称
    /// </summary>
    [Reactive]
    public string HolidayName { get; set; }

    /// <summary>
    /// 是否为休息日
    /// </summary>
    [Reactive]
    public bool IsRestDay { get; set; }

    /// <summary>
    /// 是否已确定
    /// </summary>
    [Reactive]
    public bool IsConfirmed { get; set; }

    /// <summary>
    /// 确定命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> ConfirmCommand;

    /// <summary>
    /// 提交完成交互
    /// </summary>
    public Interaction<(string, bool), Unit> ConfirmedInteraction;

    public EditHolidayWindowViewModel()
    {
        IsConfirmed = false;

        var canConfirmCommandExecute =
            this.WhenAnyValue(x => x.HolidayName)
                .Select(n => !string.IsNullOrWhiteSpace(n));

        ConfirmCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            IsConfirmed = true;
            await ConfirmedInteraction.Handle((HolidayName, IsRestDay));
        }, canConfirmCommandExecute);

        ConfirmedInteraction = new Interaction<(string, bool), Unit>();
    }
}
