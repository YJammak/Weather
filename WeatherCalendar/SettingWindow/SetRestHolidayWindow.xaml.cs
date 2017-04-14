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
    /// SetRestHolidayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetRestHolidayWindow : Window
    {
        public static readonly DependencyProperty RestHolidayInfoProperty =
            DependencyProperty.Register("RestHolidayInfo", typeof(RestHolidayInfo), typeof(GetCityWindow), new FrameworkPropertyMetadata(new RestHolidayInfo()));

        public RestHolidayInfo RestHolidayInfo
        {
            get => (RestHolidayInfo)GetValue(RestHolidayInfoProperty);
            set => SetValue(RestHolidayInfoProperty, value);
        }

        public SetRestHolidayWindow(RestHolidayInfo info)
        {
            InitializeComponent();

            RestHolidayInfo = info;
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
            Close();
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (RestHolidayInfo.Name == "")
            {
                HolidayHelper.Instance.HolidayInfo.RemoveRestHoliday(RestHolidayInfo.Date);
            }
            else
            {
                HolidayHelper.Instance.HolidayInfo.AddRestHoliday(RestHolidayInfo);
            }
            Close();
        }

        private void SetRestHolidayWindow_OnActivated(object sender, EventArgs e)
        {
            nameTextBox.Focus();
            nameTextBox.SelectionStart = nameTextBox.Text.Length;
        }
    }
}
