using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using WeatherCalendar.Models;
using WeatherCalendar.Utils;

namespace WeatherCalendar.Services;

public sealed class HolidayFileService : IHolidayService
{
    private string File { get; set; }

    public Holiday[] Holidays { get; private set; }

    public Holiday[] Load(string file)
    {
        List<Holiday> fileHolidays;
        try
        {
            fileHolidays = JsonHelper.LoadFromFileToList<Holiday>(file, "yyyy-MM-dd");
        }
        catch
        {
            fileHolidays = [];
        }

        List<Holiday> apiHolidays;
        try
        {
            var client = new RestClient("http://yjammak.net",
                configureSerialization: cfg => cfg.UseNewtonsoftJson());
            var request = new RestRequest("api/weatherCalender/holidays");
            apiHolidays = client.Get<List<Holiday>>(request);
        }
        catch
        {
            apiHolidays = [];
        }

        File = file;

        var fileMaxYear = fileHolidays.Count > 0 ? fileHolidays.Max(f => f.Year) : -1;
        var apiMaxYear = apiHolidays.Count > 0 ? apiHolidays.Max(a => a.Year) : -1;
        if (apiMaxYear > fileMaxYear)
        {
            foreach (var fileHoliday in fileHolidays)
            {
                var holiday = apiHolidays.FirstOrDefault(h => h.Year == fileHoliday.Year && h.Name == fileHoliday.Name);
                if (holiday == null)
                {
                    apiHolidays.Add(fileHoliday);
                }
                else
                {
                    if (!Holiday.Equals(holiday, fileHoliday))
                    {
                        apiHolidays.Remove(holiday);
                        apiHolidays.Add(Holiday.Combine(holiday, fileHoliday));
                    }
                }
            }

            Holidays = apiHolidays.ToArray();
            Save();
        }
        else
        {
            Holidays = fileHolidays.ToArray();
        }

        return Holidays;
    }

    public Holiday GetHoliday(DateTime date)
    {
        return Holidays?.FirstOrDefault(h =>
            h.Year == date.Year &&
            ((h.RestDates?.Contains(date.Date) ?? false) ||
             (h.WorkDates?.Contains(date.Date) ?? false)));
    }

    private void Save()
    {
        try
        {
            if (Holidays == null)
                return;

            var json = JsonHelper.SerializeObjectToFormatJson(Holidays, "yyyy-MM-dd");
            System.IO.File.WriteAllText(File, json);
        }
        catch
        {
            //
        }
    }

    public void Add(int year, string name, DateTime date, bool isRestDay)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        var holiday = Holidays?.FirstOrDefault(h => h.Year == year && h.Name == name);

        if (holiday == null)
        {
            holiday = new Holiday
            {
                Name = name,
                Year = year
            };

            if (isRestDay)
            {
                holiday.WorkDates = [];
                holiday.RestDates = [date.Date];
            }
            else
            {
                holiday.WorkDates = [date.Date];
                holiday.RestDates = [];
            }

            Holidays =
                Holidays == null
                    ? [holiday]
                    : Holidays
                        .Append(holiday)
                        .OrderBy(h => h.Year)
                        .ThenBy(h =>
                            h.RestDates?.FirstOrDefault() ??
                            h.WorkDates?.FirstOrDefault())
                        .ToArray();
        }
        else
        {
            if (isRestDay)
            {
                holiday.RestDates =
                    holiday
                        .RestDates
                        .Append(date.Date)
                        .Distinct()
                        .OrderBy(d => d)
                        .ToArray();

                if (holiday.WorkDates.Contains(date.Date))
                    holiday.WorkDates =
                        holiday
                            .WorkDates
                            .Where(d => d != date.Date)
                            .ToArray();
            }
            else
            {
                holiday.WorkDates =
                    holiday
                        .WorkDates
                        .Append(date.Date)
                        .Distinct()
                        .OrderBy(d => d)
                        .ToArray();

                if (holiday.RestDates.Contains(date.Date))
                    holiday.RestDates =
                        holiday
                            .RestDates
                            .Where(d => d != date.Date)
                            .ToArray();
            }
        }

        Save();
    }

    public void Remove(int year, string name, DateTime date, bool isRestDay)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        var holiday = Holidays?.FirstOrDefault(h => h.Year == year && h.Name == name);

        if (holiday == null)
            return;

        if (isRestDay)
        {
            if (holiday.RestDates.Contains(date.Date))
                holiday.RestDates =
                    holiday
                        .RestDates
                        .Where(d => d != date.Date)
                        .ToArray();
        }
        else
        {
            if (holiday.WorkDates.Contains(date.Date))
                holiday.WorkDates =
                    holiday
                        .WorkDates
                        .Where(d => d != date.Date)
                        .ToArray();
        }

        if (holiday.WorkDates.Length <= 0 && holiday.RestDates.Length <= 0)
            Holidays = Holidays.Where(h => h != holiday).ToArray();

        Save();
    }

    public void Remove(int year, string name, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        var holiday = Holidays?.FirstOrDefault(h => h.Year == year && h.Name == name);

        if (holiday == null)
            return;

        if (holiday.RestDates.Contains(date.Date))
            holiday.RestDates =
                holiday
                    .RestDates
                    .Where(d => d != date.Date)
                    .ToArray();

        if (holiday.WorkDates.Contains(date.Date))
            holiday.WorkDates =
                holiday
                    .WorkDates
                    .Where(d => d != date.Date)
                    .ToArray();

        if (holiday.WorkDates.Length <= 0 && holiday.RestDates.Length <= 0)
            Holidays = Holidays.Where(h => h != holiday).ToArray();

        Save();
    }
}
