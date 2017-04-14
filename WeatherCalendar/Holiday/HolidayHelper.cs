using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherCalendar
{
    public enum WorkType
    {
        Unknown = -1,
        WorkDay = 0,
        RestDay = 1,
        Holiday = 2,
    }

    public class WorkInfo
    {
        public DateTime Date { get; set; }

        public WorkType ToDayWorkType { get; set; }
    }

    public class HolidayHelper
    {
        private static HolidayHelper _instance;

        public static HolidayHelper Instance => _instance ?? (_instance = new HolidayHelper());

        public event Action HolidayChanged;

        private HolidayInfo _holidayInfo;

        /// <summary>
        /// 假期信息
        /// </summary>
        public HolidayInfo HolidayInfo
        {
            get => _holidayInfo;
            set
            {
                _holidayInfo = value;
                value.HolidayChanged += () =>
                {
                    HolidayChanged?.Invoke();
                };
            }
        }

        private HolidayHelper()
        {
            LoadHolidayInfo();

            HolidayChanged += () =>
            {
                SaveHolidayInfo();
            };
        }

        public bool LoadHolidayInfo(string fileName = "holidays.json")
        {
            var configFile = AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (!File.Exists(configFile))
            {
                HolidayInfo = new HolidayInfo();
                return true;
            }

            string jsonStr = string.Empty;
            try
            {
                jsonStr = File.ReadAllText(configFile);
                HolidayInfo = JsonHelper.DeserializeJsonToObject<HolidayInfo>(jsonStr);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SaveHolidayInfo(string fileName = "holidays.json")
        {
            try
            {
                var configFile = AppDomain.CurrentDomain.BaseDirectory + fileName;
                var jsonStr = JsonHelper.SerializeObject(HolidayInfo, "yyyy-MM-dd");

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
