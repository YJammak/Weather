using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace WeatherCalendar
{
    /// <summary>
    /// OneDayWeatherControl.xaml 的交互逻辑
    /// </summary>
    public partial class CurrentWeatherControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "no pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty CurrentDayWeatherProperty =
            DependencyProperty.Register("CurrentDayWeather", typeof(OneDayWeather), typeof(CurrentWeatherControl),
                new PropertyMetadata((o, args) =>
                {
                    var control = o as CurrentWeatherControl;
                    var weather = args.NewValue as OneDayWeather;

                    if (control == null)
                        return;

                    try
                    {
                        //control.Image = weather == null ? null :
                        //    new BitmapImage(
                        //        new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Weathers/{weather.Weather}.png",
                        //            UriKind.RelativeOrAbsolute));

                        if (weather?.DayWeather?.Weather == null)
                            control.Image = null;
                        else
                        {
                            var bitmap = new BitmapImage();

                            var imagePath = control.PeriodOfDay == PeriodOfDay.Day ? $"{AppDomain.CurrentDomain.BaseDirectory}Weathers/{WeatherConvert.Convert(weather.DayWeather.Weather)}.png" : 
                                                                                 $"{AppDomain.CurrentDomain.BaseDirectory}Weathers/{WeatherConvert.Convert(weather.NightWeather.Weather)}_夜.png";

                            if (File.Exists(imagePath))
                            {
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                                using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                                {
                                    bitmap.StreamSource = ms;
                                    bitmap.EndInit();
                                    bitmap.Freeze();
                                }

                                control.Image = bitmap;
                            }
                            else
                            {
                                control.Image = null;
                            }
                        }
                    }
                    catch
                    {
                        control.Image = null;
                    }
                }));

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image", typeof(ImageSource), typeof(CurrentWeatherControl), new PropertyMetadata(default(ImageSource)));

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

        public static readonly DependencyProperty CurrentInfoProperty = DependencyProperty.Register(
            "CurrentInfo", typeof(CurrentInfo), typeof(CurrentWeatherControl), new PropertyMetadata(default(CurrentInfo)));

        public CurrentInfo CurrentInfo
        {
            get => (CurrentInfo) GetValue(CurrentInfoProperty);
            set => SetValue(CurrentInfoProperty, value);
        }

        public static readonly DependencyProperty PeriodOfDayProperty = DependencyProperty.Register(
            "PeriodOfDay", typeof(PeriodOfDay), typeof(CurrentWeatherControl), new PropertyMetadata(default(PeriodOfDay)));

        public PeriodOfDay PeriodOfDay
        {
            get => (PeriodOfDay)GetValue(PeriodOfDayProperty);
            set => SetValue(PeriodOfDayProperty, value);
        }

        public CurrentWeatherControl()
        {
            InitializeComponent();
        }
    }
}
