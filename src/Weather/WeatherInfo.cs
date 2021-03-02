using Newtonsoft.Json;

namespace Weather
{
    /// <summary>
    /// 白天或夜间天气
    /// </summary>
    public class WeatherInfo
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
}