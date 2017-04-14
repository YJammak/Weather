using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WeatherCalendar
{
    static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 30 });

            SingleInstanceManager manager = new SingleInstanceManager();

            manager.Run(args);
        }
    }
}
