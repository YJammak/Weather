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
    /// SetCountDownWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetCountDownWindow : Window
    {
        public event Action<string> RemarksChanged;


        public static readonly DependencyProperty RemarksProperty =
            DependencyProperty.Register("Remarks", typeof(string), typeof(SetCountDownWindow), new FrameworkPropertyMetadata(""));

        public string Remarks
        {
            get => (string)GetValue(RemarksProperty);
            set => SetValue(RemarksProperty, value);
        }

        public SetCountDownWindow()
        {
            InitializeComponent();
        }

        private void SetCountDownWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void GetCityWindow_OnActivated(object sender, EventArgs e)
        {
            remarksTextBox.Focus();
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            RemarksChanged?.Invoke(Remarks);
            this.Close();
        }
    }
}
