using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Reactive.Linq;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.Services
{
    /// <summary>
    /// 工作倒计时类型
    /// </summary>
    public enum WorkCountdownType
    {
        /// <summary>
        /// 无
        /// </summary>
        None,

        /// <summary>
        /// 工作前
        /// </summary>
        BeforeWork,

        /// <summary>
        /// 下班前
        /// </summary>
        BeforeOffWork
    }

    /// <summary>
    /// 工作计时器服务
    /// 用来为上下班倒计时的服务
    /// </summary>
    public class WorkTimerService : ReactiveObject
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        [Reactive]
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Reactive]
        public TimeSpan EndTime { get; set; }

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

        public WorkTimerService()
        {
            var appService = Locator.Current.GetService<AppService>();

            var timer =
                appService
                    .TimerPerSecond
                    .Select(time =>
                    {
                        var currentTime = time.TimeOfDay;

                        if (currentTime <= StartTime)
                            return (WorkCountdownType.BeforeWork, StartTime - currentTime);

                        if (currentTime <= EndTime)
                            return (WorkCountdownType.BeforeOffWork, EndTime - currentTime);

                        return (WorkCountdownType.None, TimeSpan.Zero);
                    });

            timer.Select(d => d.Item1)
                .ToPropertyEx(this, service => service.CountdownType);

            timer.Select(d => d.Item2)
                .ToPropertyEx(this, service => service.CountdownTime);
        }
    }
}
