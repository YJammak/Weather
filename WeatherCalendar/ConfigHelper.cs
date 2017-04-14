using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WeatherCalendar
{
    public class TimeConverter : IsoDateTimeConverter
    {
        public TimeConverter() : base()
        {
            DateTimeFormat = "HH:mm:ss";
        }
    }

    public class WorkTime
    {
        [JsonConverter(typeof(TimeConverter))]
        public DateTime StartTime { get; set; }

        [JsonConverter(typeof(TimeConverter))]
        public DateTime EndTime { get; set; }
    }


    public class ConfigInfo
    {
        [DefaultValue(true)]
        public bool IsShowWorkTime { get; set; }

        public WorkTime WorkTime { get; set; }

        [DefaultValue(0)]
        public double Top { get; set; }

        [DefaultValue(0)]
        public double Left { get; set; }

        [DefaultValue(true)]
        public bool CanMove { get; set; }

        [DefaultValue(false)]
        public bool CanPenetrate { get; set; }

        [DefaultValue(false)]
        public bool TransParent { get; set; }
    }

    public class ConfigHelper
    {
        private static ConfigHelper _instance;

        public static ConfigHelper Instance => _instance ?? (_instance = new ConfigHelper());

        public ConfigInfo Config { get; set; }

        private ConfigHelper()
        {
            Load();
        }

        public bool Load(string fileName = "config.json")
        {
            var configFile = AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (!File.Exists(configFile))
            {
                Config = new ConfigInfo();
                return false;
            }

            string jsonStr = string.Empty;
            try
            {
                jsonStr = File.ReadAllText(configFile);
                Config = JsonHelper.DeserializeJsonToObject<ConfigInfo>(jsonStr);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Save(string fileName = "config.json")
        {
            try
            {
                var configFile = AppDomain.CurrentDomain.BaseDirectory + fileName;
                var jsonStr = JsonHelper.SerializeObject(Config);

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
