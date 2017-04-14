using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeatherCalendar
{
    public class WeatherConvert
    {
        public static string Convert(string weather)
        {
            if (weather.Contains("到"))
            {
                var r = weather.Split(new[] { '到' }, StringSplitOptions.RemoveEmptyEntries);

                if (r.Length > 1)
                    return r.Last();
            }

            return weather;
        }
    }
}
