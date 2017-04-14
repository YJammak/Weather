using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using WeatherCalendar.SettingWindow;

namespace WeatherCalendar
{
    /// <summary>
    /// SetNoteWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetNoteWindow : Window
    {
        public ObservableCollection<Note> Notes { get; set; }

        private DateTime currentDateTime;

        public SetNoteWindow(DateTime date)
        {
            InitializeComponent();

            currentDateTime = date;

            update();
        }

        private void update()
        {
            var notes = NotesHelper.Instance.GetNotes(currentDateTime);

            if (notes == null)
            {
                Notes = new ObservableCollection<Note>();
                return;
            }

            var notesList = notes.ToList();
            notesList.Sort();
            Notes = new ObservableCollection<Note>(notesList);
        }

        private void SetNoteWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void ButtonRemove_OnClick(object sender, RoutedEventArgs e)
        {

            var selects = listView.SelectedItems;
            List<Note> removed = new List<Note>();

            try
            {
                foreach (var @select in selects)
                {
                    var s = select as Note;

                    if (s != null)
                        removed.Add(s);
                }

                foreach (var note in removed)
                {
                    Notes.Remove(note);
                }

                NotesHelper.Instance.RemoveNote(removed);
                NotesHelper.Instance.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            AddNoteWindow anw = new AddNoteWindow();
            anw.NoteAdded += note =>
            {
                Notes.Add(note);
                note.Date = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, note.Date.Hour, note.Date.Minute, 0);
                NotesHelper.Instance.AddNote(note);
                NotesHelper.Instance.Save();
            };
            anw.ShowDialog();
        }
    }

    public class NoteToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var note = value as Note;

            if (note == null)
                return "";

            if (note.Type == Note.NoteType.Day)
                return "全天";

            return note.Date.ToString("HH:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
