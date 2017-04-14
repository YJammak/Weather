using System;
using System.Globalization;
using System.Security.Policy;
using Newtonsoft.Json;

namespace Weather
{
    /// <summary>
    /// 白天或夜间天气
    /// </summary>
    public class DayOrNightWeather
    {
        /// <summary>
        /// 天气
        /// </summary>
        [JsonProperty(propertyName: "wthr")]
        public string Weather { get; set; }

        /// <summary>
        /// 天气类型
        /// </summary>
        [JsonProperty(propertyName: "type")]
        public int Type { get; set; }

        /// <summary>
        /// 风向
        /// </summary>
        [JsonProperty(propertyName: "wd")]
        public string WindDirection { get; set; }

        /// <summary>
        /// 风力
        /// </summary>
        [JsonProperty(propertyName: "wp")]
        public string WindForce { get; set; }

        /// <summary>
        /// 提示
        /// </summary>
        [JsonProperty(propertyName: "notice")]
        public string Notice { get; set; }


        /// <summary>
        /// 风力信息
        /// </summary>
        [JsonIgnore]
        public string WindInfo => WindDirection + WindForce;
    }

    /// <summary>
    /// 一天天气
    /// </summary>
    public class OneDayWeather
    {
        /// <summary>
        /// 日升
        /// </summary>
        [JsonProperty(propertyName: "sunrise")]
        public string Sunrise { get; set; }

        /// <summary>
        /// 日落
        /// </summary>
        [JsonProperty(propertyName: "sunset")]
        public string Sunset { get; set; }

        /// <summary>
        /// 高温
        /// </summary>
        [JsonProperty(propertyName: "high")]
        public string HighTemperature { get; set; }

        /// <summary>
        /// 低温
        /// </summary>
        [JsonProperty(propertyName: "low")]
        public string LowTemperature { get; set; }

        /// <summary>
        /// 白天天气
        /// </summary>
        [JsonProperty(propertyName:"day")]
        public DayOrNightWeather DayWeather { get; set; }


        /// <summary>
        /// 夜间天气
        /// </summary>
        [JsonProperty(propertyName: "night")]
        public DayOrNightWeather NightWeather { get; set; }

        /// <summary>
        /// 温度范围
        /// </summary>
        [JsonIgnore]
        public string TemperatureRange
        {
            get
            {
                if (string.IsNullOrEmpty(HighTemperature) || string.IsNullOrEmpty(LowTemperature))
                    return string.Empty;

                return $"{LowTemperature} ~ {HighTemperature}℃";
            }
        }

        private string _date;

        /// <summary>
        /// 日期
        /// </summary>
        [JsonProperty(propertyName: "date")]
        public string Date
        {
            get => _date;
            set
            {
                _date = value;
                DateTime = System.DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// 日期
        /// </summary>
        [JsonIgnore]
        public DateTime DateTime { get; set; }
    }
}
