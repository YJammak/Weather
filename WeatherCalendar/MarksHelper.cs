using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCalendar
{
    public class Mark
    {
        public DateTime Date { get; set; }
    }

    public class MarksHelper
    {
        public event Action MarksChanged;

        private static MarksHelper _instance;

        public static MarksHelper Instance => _instance ?? (_instance = new MarksHelper());

        public List<Mark> Marks { get; set; }

        private object locker = new object();
        private MarksHelper()
        {
            Load();
        }

        public void AddMark(Mark mark)
        {
            lock (locker)
            {
                foreach (var m in Marks)
                {
                    if (mark.Date.Date == m.Date.Date)
                        return;
                }

                Marks.Add(mark);

                MarksChanged?.Invoke();
            }
        }

        public void AddMark(DateTime date)
        {
            AddMark(new Mark() {Date = date});
        }

        public void RemoveMark(DateTime date)
        {
            lock (locker)
            {
                List<Mark> remove = new List<Mark>();

                foreach (var m in Marks)
                {
                    if (date.Date == m.Date.Date)
                        remove.Add(m);
                }

                foreach (var mark in remove)
                {
                    Marks.Remove(mark);
                }

                MarksChanged?.Invoke();
            }
        }

        public Mark GetMark(DateTime date)
        {

            foreach (var m in Marks)
            {
                if (date.Date == m.Date.Date)
                    return m;
            }

            return null;
        }

        public bool Load(string fileName = "marks.json")
        {
            lock (locker)
            {
                var configFile = AppDomain.CurrentDomain.BaseDirectory + fileName;
                if (!File.Exists(configFile))
                {
                    Marks = new List<Mark>();
                    return true;
                }

                string jsonStr = string.Empty;
                try
                {
                    jsonStr = File.ReadAllText(configFile);
                    Marks = JsonHelper.DeserializeJsonToList<Mark>(jsonStr);

                    MarksChanged?.Invoke();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool Save(string fileName = "marks.json")
        {
            lock (locker)
            {
                try
                {
                    var configFile = AppDomain.CurrentDomain.BaseDirectory + fileName;
                    var jsonStr = JsonHelper.SerializeObject(Marks, "yyyy-MM-dd");

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
}
