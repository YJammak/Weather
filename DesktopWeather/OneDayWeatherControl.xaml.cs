using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Weather;

namespace DesktopWeather
{
    /// <summary>
    /// OneDayWeatherControl.xaml 的交互逻辑
    /// </summary>
    public partial class OneDayWeatherControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "no pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty CurrentDayWeatherProperty =
            DependencyProperty.Register("CurrentDayWeather", typeof(OneDayWeather), typeof(OneDayWeatherControl),
                new PropertyMetadata((o, args) =>
                {
                    var control = o as OneDayWeatherControl;
                    var weather = args.NewValue as OneDayWeather;

                    if (control == null || weather == null)
                        return;

                    control.Image = new BitmapImage(new Uri($"Resources/Weathers/{weather.Weather}.png", UriKind.RelativeOrAbsolute));
                }));

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image", typeof(ImageSource), typeof(OneDayWeatherControl), new PropertyMetadata(default(ImageSource)));

        public ImageSource Image
        {
            get => (ImageSource) GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public OneDayWeather CurrentDayWeather
        {
            get => (OneDayWeather)GetValue(CurrentDayWeatherProperty);
            set => SetValue(CurrentDayWeatherProperty, value);
        }



        public OneDayWeatherControl()
        {
            InitializeComponent();
        }
    }
}
