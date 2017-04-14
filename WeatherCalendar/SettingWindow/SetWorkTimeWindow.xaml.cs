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
using YUI.Controls;

namespace WeatherCalendar
{
    /// <summary>
    /// SetWorkTimeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetWorkTimeWindow : Window
    {
        public event Action ConfigChanged;

        public SetWorkTimeWindow()
        {
            InitializeComponent();
        }

        private void CloseWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void GetCityWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (allowCheckBox.IsChecked == null)
                return;

            if (allowCheckBox.IsChecked.Value)
            {
                DateTime start;
                DateTime end;

                try
                {
                    start = DateTime.Parse(startTextBox.Text);
                }
                catch (Exception)
                {
                    startTextBox.ShowPopUpOnControl("时间格式不正确，格式为 HH:ss（例如：12:00）", 3000);
                    return;
                }
                
                try
                {
                    end = DateTime.Parse(endTextBox.Text);
                }
                catch (Exception)
                {
                    endTextBox.ShowPopUpOnControl("时间格式不正确，格式为 HH:ss（例如：12:00）", 3000);
                    return;
                }

                if (ConfigHelper.Instance.Config.WorkTime == null)
                    ConfigHelper.Instance.Config.WorkTime = new WorkTime();

                ConfigHelper.Instance.Config.WorkTime.StartTime = start;
                ConfigHelper.Instance.Config.WorkTime.EndTime = end;
            }

            ConfigHelper.Instance.Config.IsShowWorkTime = allowCheckBox.IsChecked.Value;
            ConfigHelper.Instance.Save();
            ConfigChanged?.Invoke();
            Close();
        }

        private void SetWorkTimeWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ConfigHelper.Instance.Config == null)
                return;

            allowCheckBox.IsChecked = ConfigHelper.Instance.Config.IsShowWorkTime;

            if (ConfigHelper.Instance.Config.WorkTime == null)
                return;
            
            startTextBox.Text = ConfigHelper.Instance.Config.WorkTime.StartTime.ToString("HH:mm");
            endTextBox.Text = ConfigHelper.Instance.Config.WorkTime.EndTime.ToString("HH:mm");
        }
    }
}
