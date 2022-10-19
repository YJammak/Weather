using System;

namespace WeatherCalendar.Utils;

/// <summary>
/// 数九三伏计算
/// </summary>
public static class ChineseAlmanac
{
    private static readonly int[] JqData = { 0, 21208, 43467, 63836, 85337, 107014, 128867, 150921, 173149, 195551, 218072, 240693, 263343, 285989, 308563, 331033, 353350, 375494, 397447, 419210, 440795, 462224, 483532, 504758 };

    public static string GetShuJiuInfo(DateTime dt)
    {
        var dtBase = new DateTime(1900, 1, 6, 2, 3, 57);

        if (dt < dtBase)
            return "";

        var y = dt.Year;

        var num = 525948.76 * (y - 1900) + JqData[23];
        var dongZhi = dtBase.AddMinutes(num);
        num = 525948.76 * (y - 1 - 1900) + JqData[23];
        var donZhiLastYear = dtBase.AddMinutes(num);

        const string suZi = "一二三四五六七八九";

        var days = (dt.Date - dongZhi.Date).Days;
        switch (days)
        {
            case 0:
                return "一九";
            case 9:
                return "二九";
            case 18:
                return "三九";
            case 27:
                return "四九";
            case 36:
                return "五九";
            case 45:
                return "六九";
            case 54:
                return "七九";
            case 63:
                return "八九";
            case 72:
                return "九九";
        }

        if (days < 81 && days > 0)
            return $"{suZi.Substring(days / 9, 1)}九 第{days % 9 + 1}天";

        days = (dt.Date - donZhiLastYear.Date).Days;
        switch (days)
        {
            case 0:
                return "一九";
            case 9:
                return "二九";
            case 18:
                return "三九";
            case 27:
                return "四九";
            case 36:
                return "五九";
            case 45:
                return "六九";
            case 54:
                return "七九";
            case 63:
                return "八九";
            case 72:
                return "九九";
        }

        if (days < 81 && days > 0)
            return $"{suZi.Substring(days / 9, 1)}九 第{days % 9 + 1}天";

        return "";
    }

    public static string GetSanFuInfo(DateTime dt)
    {
        var dtBase = new DateTime(1900, 1, 6, 2, 3, 57);

        if (dt < dtBase)
            return "";

        var y = dt.Year;

        var num = 525948.76 * (y - 1900) + JqData[11];
        var xiaZhi = dtBase.AddMinutes(num);

        num = 525948.76 * (y - 1900) + JqData[14];
        var liQiu = dtBase.AddMinutes(num);

        if (dt.Date <= xiaZhi.Date || dt.Date >= liQiu.Date.AddDays(20))
            return "";

        var t = ((xiaZhi.Date - dtBase.Date).Days + 6) % 10;
        var addDay = 7 - t;
        var chuFu = xiaZhi.Date.AddDays(addDay > 0 ? addDay + 20 : addDay + 30);

        if (dt.Date == chuFu.Date)
            return "初伏";

        var zhongFu = chuFu.Date.AddDays(10);

        if (dt.Date == zhongFu.Date)
            return "中伏";

        t = ((liQiu.Date - dtBase.Date).Days + 6) % 10;
        addDay = 7 - t;
        var moFu = liQiu.Date.AddDays(addDay > 0 ? addDay : addDay + 10);

        if (dt.Date == moFu.Date)
            return "末伏";

        if (dt.Date <= chuFu.Date)
            return "";

        if (dt.Date < zhongFu.Date)
        {
            return $"初伏 第{(dt.Date - chuFu.Date).Days + 1}天";
        }
        if (dt.Date < moFu.Date)
        {
            return $"中伏 第{(dt.Date - zhongFu.Date).Days + 1}天";
        }

        return (dt.Date - moFu.Date).Days < 10 ? $"末伏 第{(dt.Date - moFu.Date).Days + 1}天" : "";
    }

    /// <summary>
    /// 三伏数九
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static string GetSanFuShuJiuString(DateTime dt)
    {
        var dtBase = new DateTime(1900, 1, 6, 2, 3, 57);

        if (dt < dtBase)
            return "";

        var y = dt.Year;

        #region 三伏计算

        var num = 525948.76 * (y - 1900) + JqData[11];
        var xiaZhi = dtBase.AddMinutes(num);

        num = 525948.76 * (y - 1900) + JqData[14];
        var liQiu = dtBase.AddMinutes(num);

        if (dt.Date > xiaZhi.Date && dt.Date < liQiu.Date.AddDays(10))
        {
            var t = ((xiaZhi.Date - dtBase.Date).Days + 6) % 10;
            var addDay = 7 - t;
            var chuFu = xiaZhi.Date.AddDays(addDay > 0 ? addDay + 20 : addDay + 30);

            if (dt.Date == chuFu.Date)
                return "初伏";

            if (dt.Date == chuFu.Date.AddDays(10))
                return "中伏";

            t = ((liQiu.Date - dtBase.Date).Days + 6) % 10;
            addDay = 7 - t;
            var moFu = liQiu.Date.AddDays(addDay > 0 ? addDay : addDay + 10);

            if (dt.Date == moFu.Date)
                return "末伏";
        }

        #endregion

        #region 数九计算

        num = 525948.76 * (y - 1900) + JqData[23];
        var dongZhi = dtBase.AddMinutes(num);
        num = 525948.76 * (y - 1 - 1900) + JqData[23];
        var donZhiLastYear = dtBase.AddMinutes(num);

        var days = (dt.Date - dongZhi.Date).Days;
        switch (days)
        {
            case 0:
                return "一九";
            case 9:
                return "二九";
            case 18:
                return "三九";
            case 27:
                return "四九";
            case 36:
                return "五九";
            case 45:
                return "六九";
            case 54:
                return "七九";
            case 63:
                return "八九";
            case 72:
                return "九九";
        }

        days = (dt.Date - donZhiLastYear.Date).Days;

        return days switch
        {
            0 => "一九",
            9 => "二九",
            18 => "三九",
            27 => "四九",
            36 => "五九",
            45 => "六九",
            54 => "七九",
            63 => "八九",
            72 => "九九",
            _ => ""
        };

        #endregion
    }
}
