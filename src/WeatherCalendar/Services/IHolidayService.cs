using System;
using WeatherCalendar.Models;

namespace WeatherCalendar.Services;

/// <summary>
/// 假期服务
/// </summary>
public interface IHolidayService
{
    /// <summary>
    /// 所有假期
    /// </summary>
    public Holiday[] Holidays { get; }

    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="file">文件</param>
    /// <returns></returns>
    public Holiday[] Load(string file);

    /// <summary>
    /// 获取指定日期的假期
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public Holiday GetHoliday(DateTime date);

    /// <summary>
    /// 添加假期
    /// </summary>
    /// <param name="year">年</param>
    /// <param name="name">名称</param>
    /// <param name="date">日期</param>
    /// <param name="isRestDay">是否为休息日</param>
    public void Add(int year, string name, DateTime date, bool isRestDay);

    /// <summary>
    /// 删除假期
    /// </summary>
    /// <param name="year">年</param>
    /// <param name="name">名称</param>
    /// <param name="date">日期</param>
    /// <param name="isRestDay">是否为休息日</param>
    public void Remove(int year, string name, DateTime date, bool isRestDay);

    /// <summary>
    /// 删除假期
    /// </summary>
    /// <param name="year">年</param>
    /// <param name="name">名称</param>
    /// <param name="date">日期</param>
    public void Remove(int year, string name, DateTime date);
}
