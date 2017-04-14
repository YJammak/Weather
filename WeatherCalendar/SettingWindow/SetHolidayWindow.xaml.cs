using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WeatherCalendar
{
    /// <summary>
    /// SetHolidayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetHolidayWindow : Window
    {
        public static readonly DependencyProperty HolidayNameProperty =
            DependencyProperty.Register("HolidayName", typeof(string), typeof(GetCityWindow), new FrameworkPropertyMetadata(""));

        public string HolidayName
        {
            get => (string)GetValue(HolidayNameProperty);
            set => SetValue(HolidayNameProperty, value);
        }

        public DateTime Date { get; set; }

        public bool IsChineseHoliday { get; set; }

        public SetHolidayWindow(DateTime date, bool isChineseHoliday)
        {
            InitializeComponent();

            Date = date;
            IsChineseHoliday = isChineseHoliday;

            if (IsChineseHoliday)
            {
                Title = "设置农历节日";
                HolidayName = HolidayHelper.Instance.HolidayInfo.GetChineseHoliday(date);
            }
            else
            {
                Title = "设置公历节日";
                HolidayName = HolidayHelper.Instance.HolidayInfo.GetHoliday(date);
            }
        }

        private void GetCityWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void GetCityWindow_OnActivated(object sender, EventArgs e)
        {
            cityTextBox.Focus();
            cityTextBox.SelectionStart = cityTextBox.Text.Length;
        }

        private void CloseWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (HolidayName == "")
            {
                if (IsChineseHoliday)
                    HolidayHelper.Instance.HolidayInfo.RemoveChineseHoliday(Date);
                else
                    HolidayHelper.Instance.HolidayInfo.RemoveHoliday(Date);
            }
            else
            {
                if (IsChineseHoliday)
                    HolidayHelper.Instance.HolidayInfo.AddChineseHoliday(HolidayName, Date);
                else
                    HolidayHelper.Instance.HolidayInfo.AddHoliday(HolidayName, Date);
            }
            Close();
        }
    }
}
