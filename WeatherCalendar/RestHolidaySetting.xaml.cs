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
using System.Windows.Shapes;
using Weather;

namespace WeatherCalendar
{
    /// <summary>
    /// RestHolidaySetting.xaml 的交互逻辑
    /// </summary>
    public partial class RestHolidaySetting : Window, INotifyPropertyChanged
    {
        public event Action UpdateWeather;

        public event Action ChangeCity;

        public event Action StopWatchOpened;

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

        public List<string> Years { get; set; } = new List<string>();

        public List<string> Mouth { get; set; } = new List<string>();

        public RestHolidaySetting()
        {
            InitializeComponent();

            for (int i = 0; i < 199; i++)
            {
                Years.Add($"{1902 + i} 年");
            }

            for (int i = 0; i < 12; i++)
            {
                Mouth.Add($"{1 + i:D02} 月");
            }
        }

        public new void Show()
        {
            if (!IsVisible)
            {
                Today();
            }

            base.Show();

            CurrentWeatherInfo = WeatherHelper.Instance.WeatherInfo;
        }

        private void RestHolidaySetting_OnLoaded(object sender, RoutedEventArgs e)
        {
            Today();
            
            WeatherHelper.Instance.WeatherInfoUpdated += () =>
            {
                CurrentWeatherInfo = WeatherHelper.Instance.WeatherInfo;
            };
        }

        private void Today()
        {
            var date = DateTime.Now;
            yearComboBox.SelectedValue = $"{date.Year} 年";
            mouthComboBox.SelectedValue = $"{date.Month:D02} 月";

            calendar.CurrentDate = date;
        }

        private void GetCityWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Hide();
        }

        private void YearComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCurrentDate();
        }

        private void MouthComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCurrentDate();
        }

        private void UpdateCurrentDate()
        {
            try
            {
                var year = int.Parse(yearComboBox.SelectedValue.ToString().Substring(0, 4));
                var mouth = int.Parse(mouthComboBox.SelectedValue.ToString().Substring(0, 2));

                calendar.CurrentDate = new DateTime(year, mouth, 1);
            }
            catch (Exception)
            {
                //
            }
        }

        private void LastButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (mouthComboBox.SelectedIndex == 0)
            {
                yearComboBox.SelectedIndex--;
                mouthComboBox.SelectedIndex = mouthComboBox.Items.Count - 1;
            }
            else
            {
                mouthComboBox.SelectedIndex--;
            } 
        }

        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (mouthComboBox.SelectedIndex == mouthComboBox.Items.Count - 1)
            {
                yearComboBox.SelectedIndex++;
                mouthComboBox.SelectedIndex = 0;
            }
            else
                mouthComboBox.SelectedIndex++;
        }

        private void TodayButton_OnClick(object sender, RoutedEventArgs e)
        {
            Today();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="rows"></param>
        private void Calendar_OnCalendarRowsChanged(object sender, int rows)
        {
            Dispatcher.Invoke(() =>
            {
                this.Height = rows == 6 ? 475 : 420;
            });
        }

        private void UpdateWeatherButton_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateWeather?.Invoke();
        }

        private void ChangeCityButton_OnClick(object sender, RoutedEventArgs e)
        {
            ChangeCity?.Invoke();
        }

        private void StopWatchButton_OnClick(object sender, RoutedEventArgs e)
        {
            StopWatchOpened?.Invoke();
        }
    }
}
