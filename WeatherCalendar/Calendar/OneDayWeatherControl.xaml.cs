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

                    if (control == null)
                        return;

                    try
                    {
                        //control.Image = weather == null ? null :
                        //    new BitmapImage(
                        //        new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Weathers/{weather.Weather}.png",
                        //            UriKind.RelativeOrAbsolute));

                        if (weather?.DayWeather?.Weather == null)
                            control.DayImage = null;
                        else
                        {
                            BitmapImage bitmap = new BitmapImage();
                            var imagePath = $"{AppDomain.CurrentDomain.BaseDirectory}Weathers/{WeatherConvert.Convert(weather.DayWeather.Weather)}.png";
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

                                control.DayImage = bitmap;
                            }
                            else
                            {
                                control.DayImage = null;
                            }
                        }
                    }
                    catch
                    {
                        control.DayImage = null;
                    }


                    try
                    {
                        //control.Image = weather == null ? null :
                        //    new BitmapImage(
                        //        new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Weathers/{weather.Weather}.png",
                        //            UriKind.RelativeOrAbsolute));

                        if (weather?.DayWeather?.Weather == null)
                            control.NightImage = null;
                        else
                        {
                            BitmapImage bitmap = new BitmapImage();
                            var imagePath = $"{AppDomain.CurrentDomain.BaseDirectory}Weathers/{WeatherConvert.Convert(weather.NightWeather.Weather)}_夜.png";
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

                                control.NightImage = bitmap;
                            }
                            else
                            {
                                control.NightImage = null;
                            }
                        }
                    }
                    catch
                    {
                        control.NightImage = null;
                    }
                }));

        public static readonly DependencyProperty DayImageProperty = DependencyProperty.Register(
            "DayImage", typeof(ImageSource), typeof(OneDayWeatherControl), new PropertyMetadata(default(ImageSource)));

        public ImageSource DayImage
        {
            get => (ImageSource)GetValue(DayImageProperty);
            set => SetValue(DayImageProperty, value);
        }

        public static readonly DependencyProperty NightImageProperty = DependencyProperty.Register(
            "NightImage", typeof(ImageSource), typeof(OneDayWeatherControl), new PropertyMetadata(default(ImageSource)));

        public ImageSource NightImage
        {
            get => (ImageSource)GetValue(NightImageProperty);
            set => SetValue(NightImageProperty, value);
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
