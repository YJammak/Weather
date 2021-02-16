﻿namespace WeatherCalendar.Models
{
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
        /// 是否锁定位置
        /// </summary>
        public bool IsLockedPosition { get; set; }
        
        /// <summary>
        /// 窗口左边缘
        /// </summary>
        public int WindowLeft { get; set; }
        
        /// <summary>
        /// 窗口上边缘
        /// </summary>
        public int WindowTop { get; set; }
    }
}