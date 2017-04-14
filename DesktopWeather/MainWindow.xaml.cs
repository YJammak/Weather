using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Weather;

namespace DesktopWeather
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "no pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private WeatherInfo _weatherInfo;
        /// <summary>
        /// 当前天气信息
        /// </summary>
        public WeatherInfo CurrentWeatherInfo
        {
            get => _weatherInfo;
            set
            {
                _weatherInfo = value;
                UpdateProperty();
            }
        }

        private DateTime _dateTime;

        public DateTime CurrentDateTime
        {
            get => _dateTime;
            set
            {
                _dateTime = value;
                UpdateProperty();
            }
        }

        private DateTime _updateDateTime;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDateTime
        {
            get => _updateDateTime;
            set
            {
                _updateDateTime = value;
                UpdateProperty();
            }
        }

        private bool _showUpdateTime;
        /// <summary>
        /// 是否显示更新时间
        /// </summary>
        public bool ShowUpdateTime
        {
            get => _showUpdateTime;
            set
            {
                _showUpdateTime = value;
                UpdateProperty();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            #if DEBUG
            ShowUpdateTime = true;
            #else
            ShowUpdateTime = false;
            #endif

            UpdateWeather();
            CurrentDateTime = DateTime.Now;

            Timer t = new Timer();
            t.AutoReset = true;
            t.Interval = 1000;
            t.Elapsed += (sender, args) =>
            {
                CurrentDateTime = DateTime.Now;
            };
            t.Start();

            Timer weatherTimer = new Timer();
            weatherTimer.AutoReset = true;
            weatherTimer.Interval = 60 * 1000 * 10;
            weatherTimer.Elapsed += (sender, args) =>
            {
                UpdateWeather();
            };

            weatherTimer.Start();
        }

        private void UpdateWeather()
        {
            WeatherHelper.Instance.UpdateWeather("上海");

            if (!WeatherHelper.Instance.IsValid) return;

            CurrentWeatherInfo = WeatherHelper.Instance.WeatherInfo;
            UpdateDateTime = DateTime.Now;
        }

        private void Window_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.PrimaryScreenWidth - ActualWidth;
            Top = 0;

            YUI.Controls.WindowHelper.SetWindowCanPenetrate(this, true);
            YUI.Controls.WindowHelper.SetWindowBottom(this);
            YUI.Controls.WindowHelper.SetWindowToolWindow(this);
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Left = SystemParameters.PrimaryScreenWidth - ActualWidth;
            Top = 0;
            YUI.Controls.WindowHelper.SetWindowBottom(this);
        }
    }
}
