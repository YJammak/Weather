using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCalendar
{
    public class RestHolidayInfo
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public bool IsRest { get; set; } = true;
    }
}
