using System;
using WeatherCalendar.Models;
using WeatherCalendar.Utils;

namespace WeatherCalendar.Services;

public class NoticeService
{
    private string File { get; set; }

    public Notice[] Notices { get; private set; }

    public NoticeService()
    {

    }

    public Notice[] Load(string file)
    {
        try
        {
            Notices = JsonHelper.LoadFromFileToList<Notice>(file).ToArray();
        }
        catch
        {
            Notices = Array.Empty<Notice>();
        }

        File = file;

        return Notices;
    }

    private void Save()
    {
        try
        {
            if (Notices == null)
                return;

            var json = JsonHelper.SerializeObjectToFormatJson(Notices);
            System.IO.File.WriteAllText(File, json);
        }
        catch
        {
            //
        }
    }
}
