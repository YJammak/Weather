using System.Collections.Generic;
using Newtonsoft.Json;

namespace Weather
{
    /// <summary>
    /// 
    /// </summary>
    public class CurrentInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(propertyName:"shidu")]
        public string Humidity { get; set; }

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
        /// 温度
        /// </summary>
        [JsonProperty(propertyName: "temp")]
        public string Temperature { get; set; }

        /// <summary>
        /// 体感温度
        /// </summary>
        [JsonProperty(propertyName: "tigan")]
        public string FeelTemperature { get; set; }

        /// <summary>
        /// 风力信息
        /// </summary>
        [JsonIgnore]
        public string WindInfo => WindDirection + WindForce;
    }

    /// <summary>
    /// 天气预报信息
    /// </summary>
    public class WeatherInfo
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonIgnore]
        public bool IsValid => !string.IsNullOrEmpty(Status.Updatetime);

        /// <summary>
        /// 获取状态描述
        /// </summary>
        [JsonProperty(propertyName: "meta")]
        public Meta Status { get; set; }

        /// <summary>
        /// 7天天气预报
        /// </summary>
        [JsonProperty(propertyName: "forecast")]
        public List<OneDayWeather> Forecast { get; set; }

        /// <summary>
        /// 15天天气预报
        /// </summary>
        [JsonProperty(propertyName: "forecast15")]
        public List<OneDayWeather> ForecastFifteenDays { get; set; }

        /// <summary>
        /// 当前信息
        /// </summary>
        [JsonProperty(propertyName: "observe")]
        public CurrentInfo CurrentInfo { get; set; }
    }
}
