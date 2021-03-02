using Newtonsoft.Json;

namespace Weather
{
    /// <summary>
    /// 
    /// </summary>
    public class Meta
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(propertyName: "upper")]
        public string Upper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(propertyName: "City")]
        public string City { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(propertyName: "citykey")]
        public string CityKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(propertyName: "up_time")]
        public string UpdateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(propertyName: "desc")]
        public string Description { get; set; }
    }
}