using System;

namespace WeatherCalendar.Models;

public class AppConfig
{
    /// <summary>
    /// 天气城市
    /// </summary>
    public string CityKey { get; set; }

    /// <summary>
    /// 是否鼠标穿透
    /// </summary>
    public bool IsMousePenetrate { get; set; }

    /// <summary>
    /// 是否背景透明
    /// </summary>
    public bool IsBackgroundTransparent { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool IsTopmost { get; set; }

    /// <summary>
    /// 是否锁定位置
    /// </summary>
    public bool IsLockedPosition { get; set; }

    /// <summary>
    /// 是否开机自启
    /// </summary>
    public bool IsAutoStart { get; set; }

    /// <summary>
    /// 窗口左边缘
    /// </summary>
    public int WindowLeft { get; set; }

    /// <summary>
    /// 窗口上边缘
    /// </summary>
    public int WindowTop { get; set; }

    /// <summary>
    /// 是否显示工作倒计时
    /// </summary>
    public bool IsShowWorkTimer { get; set; }

    /// <summary>
    /// 开始工作时间
    /// </summary>
    public TimeSpan WorkStartTime { get; set; }

    /// <summary>
    /// 结束工作时间
    /// </summary>
    public TimeSpan WorkEndTime { get; set; }

    /// <summary>
    /// 是否为自定义天气图标
    /// </summary>
    public bool IsCustomWeatherIcon { get; set; }

    /// <summary>
    /// 自定义天气图标路径
    /// </summary>
    public string CustomWeatherIconPath { get; set; }
}
