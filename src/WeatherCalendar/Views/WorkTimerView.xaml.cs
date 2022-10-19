using ReactiveUI;
using Splat;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using WeatherCalendar.Services;
using WeatherCalendar.Themes;

namespace WeatherCalendar.Views;

/// <summary>
/// WorkTimerView.xaml 的交互逻辑
/// </summary>
public partial class WorkTimerView
{
    public WorkTimerView()
    {
        InitializeComponent();

        this.WhenActivated(WhenActivated);
    }

    private void WhenActivated(CompositeDisposable disposable)
    {
        this.OneWayBind(
                ViewModel,
                model => model.CountdownType,
                view => view.TimerTypeTextBlock.Text,
                type =>
                {
                    return type switch
                    {
                        WorkCountdownType.BeforeWork => "上班",
                        WorkCountdownType.BeforeOffWork => "下班",
                        _ => ""
                    };
                })
            .DisposeWith(disposable);

        this.WhenAnyValue(x => x.ViewModel.CountdownTime)
            .Do(time =>
            {
                var totalMinutes = time.TotalMinutes;
                if (totalMinutes > 1)
                {
                    totalMinutes++;
                    this.HourTextBlock.Text = $"{Math.Floor(totalMinutes / 60)}";
                    this.MinuteTextBlock.Text = $"{Math.Floor(totalMinutes % 60)}";
                    this.Const3TextBlock.Text = "小时";
                    this.Const4TextBlock.Visibility = Visibility.Visible;
                    this.MinuteTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    this.HourTextBlock.Text = $"{Math.Floor(time.TotalSeconds)}";
                    this.Const3TextBlock.Text = "秒";
                    this.Const4TextBlock.Visibility = Visibility.Collapsed;
                    this.MinuteTextBlock.Visibility = Visibility.Collapsed;
                }

                var theme = Locator.Current.GetService<ITheme>();
                this.Const1TextBlock.Foreground = theme.WorkTimerNormalForeground;
                this.Const2TextBlock.Foreground = theme.WorkTimerNormalForeground;
                this.Const3TextBlock.Foreground = theme.WorkTimerNormalForeground;
                this.Const4TextBlock.Foreground = theme.WorkTimerNormalForeground;

                this.TimerTypeTextBlock.Foreground = theme.WorkTimerTimeForeground;
                this.HourTextBlock.Foreground = theme.WorkTimerTimeForeground;
                this.MinuteTextBlock.Foreground = theme.WorkTimerTimeForeground;
            })
            .Subscribe()
            .DisposeWith(disposable);

        this.WhenAnyValue(
                x => x.ViewModel.IsVisible,
                x => x.ViewModel.CountdownType,
                (isVisible, type) =>
                    isVisible && type != WorkCountdownType.None
                        ? Visibility.Visible
                        : Visibility.Collapsed)
            .BindTo(this, view => view.Visibility)
            .DisposeWith(disposable);
    }
}
