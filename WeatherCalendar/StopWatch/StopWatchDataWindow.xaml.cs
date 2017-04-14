using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
    /// StopWatchDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StopWatchDataWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "No Pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public StopWatchDataWindow()
        {
            InitializeComponent();
        }

        private readonly object lockObject = new object();
        public void AddRecord(int stopWatchTime)
        {
            Dispatcher.Invoke(() =>
            {
                lock (lockObject)
                {
                    stackPanel.Children.Add(new StopWatchRecordControl { Index = stackPanel.Children.Count + 1, StopWatchTime = stopWatchTime });

                    scrollViewer.ScrollToBottom();
                }
            });
        }

        public void ClearRecords()
        {
            Dispatcher.Invoke(() =>
            {
                lock (lockObject)
                {
                    stackPanel.Children.Clear();
                }
            });
        }

        private void StopWatchWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
