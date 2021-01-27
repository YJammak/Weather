﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Weather;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WeatherCalendar.ViewModels
{
    public class CalendarViewModel : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }

        /// <summary>
        /// 当前月的天
        /// </summary>
        public DayViewModel[] Days { get; }

        /// <summary>
        /// 当前月日期（某月1号）
        /// </summary>
        [Reactive]
        public DateTime CurrentMonth { get; set; }

        /// <summary>
        /// 当前月行数
        /// </summary>
        [Reactive]
        public int CurrentMonthRows { get; set; }

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

            var days = new DayViewModel[7 * 6];
            for (var i = 0; i < days.Length; i++)
            {
                days[i] = new DayViewModel();
            }
            Days = days;

            CurrentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            GotoMonthCommand = ReactiveCommand.Create<DateTime>(month =>
            {
                CurrentMonth = new DateTime(month.Year, month.Month, 1);
            });
            NextMonthCommand = ReactiveCommand.Create(() =>
            {
                CurrentMonth = CurrentMonth.AddMonths(1);
            });
            LastMonthCommand = ReactiveCommand.Create(() =>
            {
                CurrentMonth = CurrentMonth.AddMonths(-1);
            });
            CurrentMonthCommand = ReactiveCommand.Create(() =>
            {
                CurrentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            });

            this.WhenActivated(disposable =>
            {
                this.WhenAnyValue(x => x.CurrentMonth)
                    .Do(UpdateDate)
                    .Subscribe()
                    .DisposeWith(disposable);

                MessageBus
                    .Current
                    .ListenIncludeLatest<WeatherForecast>()
                    .Do(UpdateForecast)
                    .ToPropertyEx(this, model => model.Forecast)
                    .DisposeWith(disposable);
            });
        }

        /// <summary>
        /// 更新天气
        /// </summary>
        /// <param name="weatherForecast"></param>
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

        /// <summary>
        /// 更新日历日期
        /// </summary>
        /// <param name="date"></param>
        private void UpdateDate(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var startDateOfPage = firstDayOfMonth.AddDays(1 - (int)firstDayOfMonth.DayOfWeek);

            var daysOfMonth = (int)(firstDayOfMonth.AddMonths(1) - firstDayOfMonth).TotalDays;
            var endDayOfMonth = firstDayOfMonth.AddDays(daysOfMonth - 1);


            var rows = 4;
            var endDateOfPage = startDateOfPage.AddDays(7 * rows - 1);

            while (endDateOfPage < endDayOfMonth)
            {
                rows++;
                endDateOfPage = startDateOfPage.AddDays(7 * rows - 1);
            }

            CurrentMonthRows = rows;

            for (var i = 0; i < 7 * rows; i++)
            {
                var dateOfDay = startDateOfPage.AddDays(i);
                Days[i].Date.Date = dateOfDay;
                Days[i].IsCurrentMonth =
                    dateOfDay.Year == date.Year &&
                    dateOfDay.Month == date.Month;
            }

            UpdateForecast(Forecast);
        }
    }
}