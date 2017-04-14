using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WeatherCalendar
{

    public class Note : IComparable
    {
        public enum NoteType
        {
            /// <summary>
            /// 全天
            /// </summary>
            Day,

            /// <summary>
            /// 准确时间
            /// </summary>
            Time,
        }

        public enum CircleType
        {
            /// <summary>
            /// 一次
            /// </summary>
            Once,

            /// <summary>
            /// 每周
            /// </summary>
            Week,

            /// <summary>
            /// 每月
            /// </summary>
            Month,

            /// <summary>
            /// 每年
            /// </summary>
            Year,
        }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public NoteType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CircleType Circle { get; set; }

        [JsonIgnore]
        public string TypeInfo
        {
            get
            {
                if (Type == NoteType.Day)
                    return "";

                return Date.ToString("HH:mm");
            }
        }

        public int CompareTo(object other)
        {
            var otherNote = other as Note;

            if (otherNote == null)
                return -1;

            if (Type == NoteType.Day)
            {
                if (otherNote.Type == NoteType.Time)
                    return -1;

                return Date > otherNote.Date ? -1 : 1;
            }
            else
            {
                if (otherNote.Type == NoteType.Day)
                    return 1;

                return Date > otherNote.Date ? -1 : 1;
            }
        }
    }

    public class NotesHelper
    {
        public event Action NotesChanged;

        private static NotesHelper _instance;

        public static NotesHelper Instance => _instance ?? (_instance = new NotesHelper());

        private List<Note> Notes { get; set; }

        private NotesHelper()
        {
            Load();
        }

        public Note[] GetNotes(DateTime date)
        {
            if (Notes == null)
                return null;

            var result = from n in Notes
                         where n.Date.Date == date.Date || 
                         (n.Circle == Note.CircleType.Week && n.Date.DayOfWeek == date.Date.DayOfWeek) ||
                         (n.Circle == Note.CircleType.Month && n.Date.Day == date.Date.Day) ||
                         (n.Circle == Note.CircleType.Year && n.Date.Month == date.Date.Month && n.Date.Day == date.Date.Day)
                         orderby n
                         select n;

            return result.Any() ? result.ToArray() : null;
        }

        public Note[] GetAllNotes()
        {
            return Notes?.ToArray();
        }

        public void AddNote(Note note)
        {
            if (Notes == null)
                Notes = new List<Note>();

            Notes.Add(note);

            NotesChanged?.Invoke();
        }

        public void RemoveNote(Note note)
        {
            if (Notes?.RemoveAll(n => n.Date == note.Date && n.Circle == note.Circle
                                      && n.Content == note.Content && n.Type == note.Type && n.Title == note.Title) > 0)
                NotesChanged?.Invoke();
        }

        public void RemoveNote(IEnumerable<Note> notes)
        {
            if (notes == null || !notes.Any())
                return;

            var count = notes.Sum(note => Notes?.RemoveAll(n => n.Date == note.Date && n.Circle == note.Circle && n.Content == note.Content && n.Type == note.Type && n.Title == note.Title) ?? 0);

            if (count > 0)
                NotesChanged?.Invoke();
        }

        public bool Load(string fileName = "notes.json")
        {
            var configFile = AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (!File.Exists(configFile))
            {
                Notes = new List<Note>();
                return true;
            }

            string jsonStr = string.Empty;
            try
            {
                jsonStr = File.ReadAllText(configFile);
                Notes = JsonHelper.DeserializeJsonToList<Note>(jsonStr);

                NotesChanged?.Invoke();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Save(string fileName = "notes.json")
        {
            try
            {
                var configFile = AppDomain.CurrentDomain.BaseDirectory + fileName;
                var jsonStr = JsonHelper.SerializeObject(Notes);

                using (StreamWriter sw = new StreamWriter(File.Create(configFile)))
                {
                    sw.Write(jsonStr);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
