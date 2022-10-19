using System;
using System.Linq;
using WeatherCalendar.Models;
using WeatherCalendar.Utils;

namespace WeatherCalendar.Services;

public sealed class HolidayFileService : IHolidayService
{
    private string File { get; set; }

    public Holiday[] Holidays { get; private set; }

    public Holiday[] Load(string file)
    {
        try
        {
            Holidays = JsonHelper.LoadFromFileToList<Holiday>(file, "yyyy-MM-dd").ToArray();
        }
        catch
        {
            Holidays = Array.Empty<Holiday>();
        }

        File = file;

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
                holiday.WorkDates = Array.Empty<DateTime>();
                holiday.RestDates = new[] { date.Date };
            }
            else
            {
                holiday.WorkDates = new[] { date.Date };
                holiday.RestDates = Array.Empty<DateTime>();
            }

            Holidays =
                Holidays == null
                    ? new[] { holiday }
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
