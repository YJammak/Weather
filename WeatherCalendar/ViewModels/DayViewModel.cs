using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using Weather;
using WeatherCalendar.Models;

namespace WeatherCalendar.ViewModels
{
    public class DayViewModel : ReactiveObject
    {
        /// <summary>
        /// 日期信息
        /// </summary>
        [Reactive]
        public DateInfo Date { get; set; }

        /// <summary>
        /// 是否为当前月
        /// </summary>
        [Reactive]
        public bool IsCurrentMonth { get; set; }

        /// <summary>
        /// 天气信息
        /// </summary>
        [Reactive]
        public ForecastInfo Forecast { get; set; }

        /// <summary>
        /// 公历日期
        /// </summary>
        [ObservableAsProperty]
        public string Day { get; }

        /// <summary>
        /// 副标题1
        /// </summary>
        [ObservableAsProperty]
        public string Subtitle1 { get; }

        /// <summary>
        /// 副标题2
        /// </summary>
        [ObservableAsProperty]
        public string Subtitle2 { get; set; }

        public DayViewModel()
        {
            Date = new DateInfo();

            this.WhenAnyValue(x => x.Date.Date)
                .Select(d => d.Day.ToString())
                .ToPropertyEx(this, model => model.Day);

            this.WhenAnyValue(x => x.Date.LunarDayName)
                .ToPropertyEx(this, model => model.Subtitle1);
        }

        public override string ToString()
        {
            return Date?.Date.ToString("yyyy-MM-dd");
        }
    }
}
