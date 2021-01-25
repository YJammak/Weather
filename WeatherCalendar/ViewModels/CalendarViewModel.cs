using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Weather;
using WeatherCalendar.Models;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.ViewModels
{
    public class CalendarViewModel : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }

        //private readonly SourceList<DayViewModel> _daysSource;
        //private readonly ReadOnlyObservableCollection<DayViewModel> _days;
        ///// <summary>
        ///// 当前页的天
        ///// </summary>
        //public ReadOnlyObservableCollection<DayViewModel> Days => _days;

        /// <summary>
        /// 当前页的天
        /// </summary>
        [Reactive]
        public DayViewModel[] Days { get; set; }

        /// <summary>
        /// 当前页日期（某月1号）
        /// </summary>
        [Reactive]
        public DateTime CurrentPageDate { get; set; }
        
        /// <summary>
        /// 当前页行数
        /// </summary>
        [Reactive]
        public int CurrentPageRows { get; set; }
        
        /// <summary>
        /// 天气预报
        /// </summary>
        [ObservableAsProperty]
        public WeatherForecast Forecast { get; }

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

        public CalendarViewModel()
        {
            Activator = new ViewModelActivator();

            //_daysSource = new SourceList<DayViewModel>();
            //_daysSource.Connect()
            //    .ObserveOnDispatcher()
            //    .Bind(out _days)
            //    .Subscribe();

            CurrentPageDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            GotoMonthCommand = ReactiveCommand.Create(new Action<DateTime>(UpdateDate));
            NextMonthCommand = ReactiveCommand.Create(() =>
            {
                CurrentPageDate = CurrentPageDate.AddMonths(1);
            });
            LastMonthCommand = ReactiveCommand.Create(() =>
            {
                CurrentPageDate = CurrentPageDate.AddMonths(-1);
            });
            CurrentMonthCommand = ReactiveCommand.Create(() =>
            {
                CurrentPageDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            });

            this.WhenActivated(disposable =>
            {
                this.WhenAnyValue(x => x.CurrentPageDate)
                    .InvokeCommand(GotoMonthCommand)
                    .DisposeWith(disposable);

                MessageBus
                    .Current
                    .ListenIncludeLatest<WeatherForecast>()
                    .Do(UpdateForecast)
                    .ToPropertyEx(this, model => model.Forecast)
                    .DisposeWith(disposable);
            });
        }

        private void UpdateForecast(WeatherForecast weatherForecast)
        {
            foreach (var day in Days)
            {
                var day1 = day;
                var forecast = weatherForecast
                    ?.Forecast
                    .FirstOrDefault(f => f.DateTime.Date == day1.Date.Date.Date) ?? weatherForecast
                    ?.ForecastFifteenDays
                    .FirstOrDefault(f => f.DateTime.Date == day.Date.Date.Date);

                day.Forecast = forecast;
            }
        }

        private void UpdateDate(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var startDateOfPage = firstDayOfMonth.AddDays(1 - (int) firstDayOfMonth.DayOfWeek);

            var daysOfMonth = (int)(firstDayOfMonth.AddMonths(1) - firstDayOfMonth).TotalDays;
            var endDayOfMonth = firstDayOfMonth.AddDays(daysOfMonth - 1);
            

            var rows = 4;
            var endDateOfPage = startDateOfPage.AddDays(7 * rows - 1);

            while (endDateOfPage < endDayOfMonth)
            {
                rows++;
                endDateOfPage = startDateOfPage.AddDays(7 * rows - 1);
            }

            CurrentPageRows = rows;

            var days = new DayViewModel[7 * rows];
            for (var i = 0; i < days.Length; i++)
            {
                days[i] = new DayViewModel
                {
                    Date = new DateInfo
                    {
                        Date = startDateOfPage.AddDays(i)
                    }
                };
            }

            Days = days;

            //_daysSource.Edit(list =>
            //{
            //    list.Clear();
            //    list.AddRange(days);
            //});

            UpdateForecast(Forecast);
        }
    }
}
