using ReactiveUI;
using System;
using System.Reactive;

namespace WeatherCalendar.ViewModels;

public class CalendarBaseViewModel : ReactiveObject
{
    /// <summary>
    /// 跳转到指定月命令
    /// </summary>
    public ReactiveCommand<DateTime, Unit> GotoMonthCommand;

    /// <summary>
    /// 跳转到当前月命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> CurrentMonthCommand;

    /// <summary>
    /// 跳转到上一个月
    /// </summary>
    public ReactiveCommand<Unit, Unit> LastMonthCommand;

    /// <summary>
    /// 跳转到下一个月
    /// </summary>
    public ReactiveCommand<Unit, Unit> NextMonthCommand;
}
