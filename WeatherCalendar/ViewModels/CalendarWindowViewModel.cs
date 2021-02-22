using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Linq;

namespace WeatherCalendar.ViewModels
{
    public class CalendarWindowViewModel : CalendarBaseViewModel
    {
        /// <summary>
        /// 日历
        /// </summary>
        public CalendarViewModel Calendar { get; }

        /// <summary>
        /// 年
        /// </summary>
        public int[] Years { get; }

        /// <summary>
        /// 月
        /// </summary>
        public int[] Months { get; }

        /// <summary>
        /// 选中的年
        /// </summary>
        [Reactive]
        public int SelectedYear { get; set; }

        /// <summary>
        /// 选中的月
        /// </summary>
        [Reactive]
        public int SelectedMonth { get; set; }

        public CalendarWindowViewModel()
        {
            Calendar = new CalendarViewModel();

            GotoMonthCommand = Calendar.GotoMonthCommand;
            CurrentMonthCommand = Calendar.CurrentMonthCommand;
            LastMonthCommand = Calendar.LastMonthCommand;
            NextMonthCommand = Calendar.NextMonthCommand;

            Years = new int[199];
            for (var i = 0; i < Years.Length; i++)
            {
                Years[i] = 1902 + i;
            }

            Months = new int[12];
            for (var i = 0; i < Months.Length; i++)
            {
                Months[i] = i + 1;
            }

            SelectedYear = Calendar.CurrentMonth.Year;
            SelectedMonth = Calendar.CurrentMonth.Month;

            this.Calendar
                .WhenAnyValue(x => x.CurrentMonth)
                .Do(date =>
                {
                    SelectedYear = date.Year;
                    SelectedMonth = date.Month;
                })
                .Subscribe();

            this.WhenAnyValue(
                    x => x.SelectedYear,
                    x => x.SelectedMonth,
                    (year, month) => new DateTime(year, month, 1))
                .InvokeCommand(this, model => model.GotoMonthCommand);
        }
    }
}
