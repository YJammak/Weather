using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.IO;

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

    public class CityKeyInfo
    {
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市代码
        /// </summary>
        public string CityKey { get; set; }

        public CityKeyInfo(string info)
        {
            var infos = info.Trim().Split(',');

            if (infos.Length != 4)
                return;

            CityKey = infos[0];
            City = infos[1];
            District = infos[2];
            Province = infos[3];
        }

        public override string ToString()
        {
            return $"{City},{District},{Province}({CityKey})";
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
            using (var sr = new StringReader(Resource.CityKeys))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var cityKey = new CityKeyInfo(line);
                        if (!string.IsNullOrEmpty(cityKey.CityKey) && !string.IsNullOrEmpty(cityKey.City))
                            CityKeyInfos.Add(cityKey);
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
        public WeatherForecast WeatherForecast { get; private set; }

        /// <summary>
        /// 更新天气信息
        /// </summary>
        /// <param name="city">城市代码</param>
        /// <returns></returns>
        public WeatherForecast UpdateWeather(CityKeyInfo city)
        {
            try
            {
                if (string.IsNullOrEmpty(city?.CityKey))
                    return null;

                var client = new RestClient("http://zhwnlapi.etouch.cn");
                client.UseNewtonsoftJson();

                var request = new RestRequest("Ecalender/api/v2/weather");
                request.AddParameter("app_key", "99817882");
                request.AddParameter("citykey", city.CityKey);

                var forecast = client.Get<WeatherForecast>(request);

                if (forecast == null)
                    return null;

                WeatherForecast = forecast;

                WeatherInfoUpdated?.Invoke();

                return WeatherForecast;
            }
            catch
            {
                return null;
            }
        }
    }
}
