using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace WeatherCalendar.Services
{
    public class AppService
    {
        public IObservable<DateTime> TimerPerSecond { get; }

        public IObservable<DateTime> TimerPerMinute { get; }

        public AppService()
        {
            var timer =
                Observable
                    .Timer(DateTimeOffset.Now, TimeSpan.FromMilliseconds(10))
                    .Select(_ => DateTime.Now)
                    .SubscribeOn(RxApp.TaskpoolScheduler);

            TimerPerSecond =
                timer.Buffer(2, 1)
                    .Where(buffer =>
                        buffer.Count == 2 &&
                        buffer[0].Second != buffer[1].Second)
                    .Select(buffer => buffer[1]);

            TimerPerMinute =
                TimerPerSecond
                    .Buffer(2, 1)
                    .Where(buffer =>
                        buffer.Count == 2 &&
                        buffer[0].Minute != buffer[1].Minute)
                    .Select(buffer => buffer[1]);
        }
    }
}
