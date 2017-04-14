using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherCalendar
{
    public class CountDown : IComparable
    {
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 倒计时天数
        /// </summary>
        [JsonIgnore]
        public double Days => (Date.Date - DateTime.Today).Days;

        /// <summary>
        /// 精确倒计时天数
        /// </summary>
        [JsonIgnore]
        public TimeSpan DaysTimeSpan => (Date.Date - DateTime.Now);

        public int CompareTo(object other)
        {
            var otherCountDown = other as CountDown;

            if (otherCountDown == null)
                return -1;

            if (Date.Date > otherCountDown.Date.Date)
                return 1;

            if (Date.Date < otherCountDown.Date.Date)
                return -1;

            return 0;
        }
    }

    public class CountDownHelper
    {
        public event Action CountDownsChanged;

        private static CountDownHelper _instance;

        public static CountDownHelper Instance => _instance ?? (_instance = new CountDownHelper());

        private List<CountDown> CountDowns { get; set; }

        private CountDownHelper()
        {
            Load();
        }

        public CountDown GetCountDown(DateTime date)
        {
            if (CountDowns == null)
                return null;

            foreach (var countDown in CountDowns)
            {
                if (countDown.Date.Date == date.Date)
                    return countDown;
            }

            return null;
        }

        public CountDown[] GetAllCountDowns()
        {
            return CountDowns?.ToArray();
        }

        public CountDown[] GetTopCountDowns(int num)
        {
            if (CountDowns == null)
                return null;

            var r = (from c in CountDowns where c.Date.Date > DateTime.Today orderby c.Date select c).ToList();

            num = r.Count() > num ? num : r.Count();
            CountDown[] count = new CountDown[num];

            if (num > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    count[i] = r[i];
                }
            }

            return count;
        }

        public void AddCountDown(DateTime date, string remarks)
        {
            AddCountDown(new CountDown() {Date = date, Remarks = remarks});
        }

        public void AddCountDown(CountDown countDown)
        {
            try
            {
                if (CountDowns == null)
                    CountDowns = new List<CountDown>();

                if (countDown.Date.Date < DateTime.Now.Date)
                    return;

                foreach (var down in CountDowns)
                {
                    if (down.Date.Date == countDown.Date.Date)
                    {
                        down.Remarks = countDown.Remarks;
                        return;
                    }
                }

                CountDowns.Add(countDown);
                CountDowns.Sort();
            }
            finally
            {
                CountDownsChanged?.Invoke();
            }
        }

        public void RemoveCountDown(DateTime date)
        {
            List<CountDown> removes = new List<CountDown>();

            foreach (var countDown in CountDowns)
            {
                if (countDown.Date.Date == date.Date)
                    removes.Add(countDown);
            }

            foreach (var countDown in removes)
            {
                CountDowns.Remove(countDown);
            }

            if (removes.Count > 0)
                CountDownsChanged?.Invoke();
        }

        public bool Load(string fileName = "countdowns.json")
        {
            var configFile = AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (!File.Exists(configFile))
            {
                CountDowns = new List<CountDown>();
                return true;
            }

            string jsonStr = string.Empty;
            try
            {
                jsonStr = File.ReadAllText(configFile);
                CountDowns = JsonHelper.DeserializeJsonToList<CountDown>(jsonStr);

                List<CountDown> removes = new List<CountDown>();

                foreach (var countDown in CountDowns)
                {
                    if (countDown.Days < 0)
                        removes.Add(countDown);
                }

                foreach (var countDown in removes)
                {
                    CountDowns.Remove(countDown);
                }

                CountDowns.Sort();
                CountDownsChanged?.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Save(string fileName = "countdowns.json")
        {
            try
            {
                var configFile = AppDomain.CurrentDomain.BaseDirectory + fileName;
                var jsonStr = JsonHelper.SerializeObject(CountDowns, "yyyy-MM-dd");

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
