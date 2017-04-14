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
using YUI.Controls;

namespace WeatherCalendar.SettingWindow
{
    /// <summary>
    /// AddNoteWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddNoteWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "No pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event Action<Note> NoteAdded;

        public AddNoteWindow()
        {
            InitializeComponent();
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (contentTextbox.Text == "")
            {
                contentTextbox.ShowPopUpOnControl("记事内容不能为空", 3000);
                return;
            }

            if (allDayCheckBox.IsChecked != null && (!allDayCheckBox.IsChecked.Value && timeTextBox.Text == ""))
            {
                timeTextBox.ShowPopUpOnControl("时间不能为空", 3000);
                return;
            }

            DateTime time = DateTime.Now;

            if (allDayCheckBox.IsChecked != null && !allDayCheckBox.IsChecked.Value)
            {
                try
                {
                    time = DateTime.Parse(timeTextBox.Text);
                }
                catch (Exception)
                {
                    timeTextBox.ShowPopUpOnControl("时间格式不正确，格式为 HH:ss（例如：12:00）", 3000);
                    return;
                }
            }

            var note = new Note
            {
                Date = time,
                Type =
                    allDayCheckBox.IsChecked != null && allDayCheckBox.IsChecked.Value
                        ? Note.NoteType.Day
                        : Note.NoteType.Time,
                Circle = Note.CircleType.Once,
                Content = contentTextbox.Text
            };

            NoteAdded?.Invoke(note);

            this.Close();
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
            this.Close();
        }
    }
}
