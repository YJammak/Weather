using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WeatherCalendar
{
    /// <summary>
    /// StopWatchRecordControl.xaml 的交互逻辑
    /// </summary>
    public partial class StopWatchRecordControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "No Pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _index;

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                UpdateProperty();
            }
        }

        private int _stopWatchTime;

        public int StopWatchTime
        {
            get => _stopWatchTime;
            set
            {
                _stopWatchTime = value;
                UpdateProperty();
            }
        }

        public StopWatchRecordControl()
        {
            InitializeComponent();
        }
    }

    public class StopWatchTimeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var time = (int) value;
                return $"{time / 1000 / 60:D02}:{ time / 1000 % 60:D02}.{time % 1000 / 10:D02}";
            }
            catch (Exception)
            {
                return "00:00.00";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IndexToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return $"#{(int)value:D02}";
            }
            catch (Exception)
            {
                return "#00";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
