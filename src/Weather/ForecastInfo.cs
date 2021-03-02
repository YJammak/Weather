using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Weather
{
    public class ForecastInfo
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
        [JsonProperty(propertyName: "day")]
        public WeatherInfo DayWeather { get; set; }


        /// <summary>
        /// 夜间天气
        /// </summary>
        [JsonProperty(propertyName: "night")]
        public WeatherInfo NightWeather { get; set; }
        
        /// <summary>
        /// 空气质量指数
        /// </summary>
        [JsonProperty(propertyName: "aqi")]
        public int AirQualityIndex { get; set; }

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
                DateTime = DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// 日期
        /// </summary>
        [JsonIgnore]
        public DateTime DateTime { get; set; }
    }
}