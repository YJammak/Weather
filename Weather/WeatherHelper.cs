using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace Weather
{
    /// <summary>
    /// 查询类型
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// 城市名称
        /// </summary>
        CityName,

        /// <summary>
        /// 城市代码
        /// </summary>
        CityKey,
    }

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
        public string Citykey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(propertyName: "up_time")]
        public string Updatetime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(propertyName: "desc")]
        public string Desc { get; set; }
    }

    public class CityKeyInfo
    {
        public string City { get; set; }

        public string Area { get; set; }

        public string Province { get; set; }

        public string CityKey { get; set; }

        public CityKeyInfo(string info)
        {
            var infos = info.Trim().Split(',');

            if (infos.Length != 4)
                return;

            CityKey = infos[0];
            City = infos[1];
            Area = infos[2];
            Province = infos[3];
        }

        public override string ToString()
        {
            return $"{City},{Area},{Province}({CityKey})";
        }
    }

    /// <summary>
    /// 天气辅助类
    /// </summary>
    public class WeatherHelper
    {
        private static WeatherHelper _instance;

        /// <summary>
        /// 天气辅助类实例
        /// </summary>
        public static WeatherHelper Instance => _instance ?? (_instance = new WeatherHelper());

        public List<CityKeyInfo> CityKeyInfos = new List<CityKeyInfo>();

        private WeatherHelper()
        {
            using (var sr = new StringReader(Properties.Resources.CityKeys))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var citykey = new CityKeyInfo(line);
                        if (!string.IsNullOrEmpty(citykey.CityKey) && !string.IsNullOrEmpty(citykey.City))
                            CityKeyInfos.Add(citykey);
                    }
                }
            }
        }

        /// <summary>
        /// 天气更新事件
        /// </summary>
        public event Action WeatherInfoUpdated;

        /// <summary>
        /// 
        /// </summary>
        public WeatherInfo WeatherInfo { get; private set; }

        /// <summary>
        /// 更新天气信息
        /// </summary>
        /// <param name="city">城市代码</param>
        /// <returns></returns>
        public WeatherInfo UpdateWeather(CityKeyInfo city)
        {
            try
            {
                if (string.IsNullOrEmpty(city?.CityKey))
                    return null;

                HttpWebRequest httpWeather = WebRequest.CreateHttp($"http://zhwnlapi.etouch.cn/Ecalender/api/v2/weather?app_key=99817882&citykey={city.CityKey}");

                httpWeather.Timeout = 5000;
                httpWeather.Method = WebRequestMethods.Http.Get;
                using (var responseStream = httpWeather.GetResponse().GetResponseStream())
                {
                    if (responseStream == null)
                        return null;

                    var weatherData = new StreamReader(new GZipStream(responseStream, CompressionMode.Decompress));
                    var weatherString = weatherData.ReadToEnd();

                    var result = JsonConvert.DeserializeObject<WeatherInfo>(weatherString);

                    WeatherInfo = result;

                    WeatherInfoUpdated?.Invoke();

                    return WeatherInfo;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
