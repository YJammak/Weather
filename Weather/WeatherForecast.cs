using System.Collections.Generic;
using Newtonsoft.Json;

namespace Weather
{
    /// <summary>
    /// 天气预报信息
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonIgnore]
        public bool IsValid => !string.IsNullOrEmpty(Status.UpdateTime);

        /// <summary>
        /// 获取状态描述
        /// </summary>
        [JsonProperty(propertyName: "meta")]
        public Meta Status { get; set; }

        /// <summary>
        /// 7天天气预报
        /// </summary>
        [JsonProperty(propertyName: "forecast")]
        public List<ForecastInfo> Forecast { get; set; }

        /// <summary>
        /// 15天天气预报
        /// </summary>
        [JsonProperty(propertyName: "forecast15")]
        public List<ForecastInfo> ForecastFifteenDays { get; set; }

        /// <summary>
        /// 当前信息
        /// </summary>
        [JsonProperty(propertyName: "observe")]
        public RealTimeInfo RealTimeWeather { get; set; }
    }
}