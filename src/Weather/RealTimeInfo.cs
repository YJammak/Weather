using Newtonsoft.Json;

namespace Weather;

/// <summary>
/// 
/// </summary>
public class RealTimeInfo
{
    /// <summary>
    /// 湿度
    /// </summary>
    [JsonProperty(propertyName: "shidu")]
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