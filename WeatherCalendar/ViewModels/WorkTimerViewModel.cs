using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Reactive.Linq;
using WeatherCalendar.Services;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.ViewModels
{
    public class WorkTimerViewModel : ReactiveObject
    {
        /// <summary>
        /// 倒计时类型
        /// </summary>
        [ObservableAsProperty]
        public WorkCountdownType CountdownType { get; }

        /// <summary>
        /// 倒计时
        /// </summary>
        [ObservableAsProperty]
        public TimeSpan CountdownTime { get; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [Reactive]
        public bool IsVisible { get; set; }

        public WorkTimerViewModel()
        {
            var appConfigService = Locator.Current.GetService<AppConfigService>();
            IsVisible = appConfigService.Config.IsShowWorkTimer;

            var workTimerService = Locator.Current.GetService<WorkTimerService>();

            workTimerService
                .WhenAnyValue(x => x.CountdownType)
                .ObserveOnDispatcher()
                .ToPropertyEx(this, model => model.CountdownType);

            workTimerService
                .WhenAnyValue(x => x.CountdownTime)
                .ObserveOnDispatcher()
                .ToPropertyEx(this, model => model.CountdownTime);
        }
    }
}
