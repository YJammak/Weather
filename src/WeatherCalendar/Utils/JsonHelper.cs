using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace WeatherCalendar.Utils;

/// <summary>
/// Json辅助类（此类依赖Newtonsoft.Json库）
/// </summary>
public static class JsonHelper
{
    private static MethodInfo _serializeObjectMethodInfo;
    private static MethodInfo _deserializeObjectMethodInfo;
    private static MethodInfo _jsonParseMethodInfo;
    private static MethodInfo _jsonToStringMethodInfo;
    private static Type _isoDateTimeConverterType;
    private static Type _jsonConverterType;

    static JsonHelper()
    {
        try
        {
            LoadAssembly("Newtonsoft.Json.dll");
        }
        catch
        {
            Console.WriteLine(@"未加载【Newtonsoft.Json.dll】");
        }
    }

    /// <summary>
    /// 加载Newtonsoft.Json.dll
    /// </summary>
    /// <param name="assemblyPath"></param>
    /// <returns></returns>
    public static bool LoadAssembly(string assemblyPath)
    {
        Assembly assembly;
        try
        {
            assembly = Assembly.Load("Newtonsoft.Json");
        }
        catch
        {
            assembly = Assembly.LoadFrom(assemblyPath);
        }

        var type = assembly.GetType("Newtonsoft.Json.JsonConvert");
        if (type == null)
            return false;
        _jsonConverterType = assembly.GetType("Newtonsoft.Json.JsonConverter");
        if (_jsonConverterType == null)
            return false;
        var jsonFormattingType = assembly.GetType("Newtonsoft.Json.Formatting");
        _serializeObjectMethodInfo = type.GetMethod("SerializeObject", new[] { typeof(object), _jsonConverterType.MakeArrayType() });
        _isoDateTimeConverterType = assembly.GetType("Newtonsoft.Json.Converters.IsoDateTimeConverter");

        _deserializeObjectMethodInfo = type.GetMethod("DeserializeObject", new[] { typeof(string), _jsonConverterType.MakeArrayType() });

        type = assembly.GetType("Newtonsoft.Json.Linq.JToken");
        if (type == null) return false;
        _jsonParseMethodInfo = type.GetMethod("Parse", new[] { typeof(string) });
        _jsonToStringMethodInfo = type.GetMethod("ToString", new[] { jsonFormattingType, _jsonConverterType.MakeArrayType() });

        return true;
    }

    /// <summary>
    /// 将对象序列化为JSON格式
    /// </summary>
    /// <param name="o">对象</param>
    /// <param name="dateTimeFormat">时间格式</param>
    /// <returns>json字符串</returns>
    public static string SerializeObject(object o, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
    {
        return _serializeObjectMethodInfo.Invoke(null, new[] { o, GetIsoDateTimeConverterArray(dateTimeFormat) }) as string;
    }

    /// <summary>
    /// 将对象序列化为格式化的JSON
    /// </summary>
    /// <param name="o"></param>
    /// <param name="dateTimeFormat"></param>
    /// <returns></returns>
    public static string SerializeObjectToFormatJson(object o, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
    {
        return FormatJson(SerializeObject(o, dateTimeFormat));
    }

    /// <summary>
    /// 反序列化为对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
    /// <param name="dateTimeFormat">时间格式</param>
    /// <returns>对象实体</returns>
    public static T DeserializeToObject<T>(string json, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss") where T : class
    {
        return _deserializeObjectMethodInfo.MakeGenericMethod(typeof(T))
            .Invoke(null, new object[] { json, GetIsoDateTimeConverterArray(dateTimeFormat) }) as T;
    }

    /// <summary>
    /// 从文件读取并反序列化为对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="dateTimeFormat"></param>
    /// <returns></returns>
    public static T LoadFromFileToObject<T>(string filePath, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss") where T : class
    {
        var data = File.ReadAllText(filePath);
        return DeserializeToObject<T>(data, dateTimeFormat);
    }

    /// <summary>
    /// 反序列化为对象集合
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
    /// <param name="dateTimeFormat">时间格式</param>
    /// <returns>对象实体集合</returns>
    public static List<T> DeserializeToList<T>(string json, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss") where T : class
    {
        return _deserializeObjectMethodInfo.MakeGenericMethod(typeof(List<T>)).Invoke(null, new object[] { json, GetIsoDateTimeConverterArray(dateTimeFormat) }) as List<T>;
    }

    /// <summary>
    /// 从文件读取并反序列化为对象集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="dateTimeFormat"></param>
    /// <returns></returns>
    public static List<T> LoadFromFileToList<T>(string filePath, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss") where T : class
    {
        var data = File.ReadAllText(filePath);
        return DeserializeToList<T>(data, dateTimeFormat);
    }

    /// <summary>
    /// 格式化Json
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static string FormatJson(string json)
    {
        var jt = _jsonParseMethodInfo.Invoke(null, new object[] { json });
        return _jsonToStringMethodInfo.Invoke(jt, new object[] { 1, null }) as string;
    }

    /// <summary>
    /// 压缩Json
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static string UnFormatJson(string json)
    {
        var jt = _jsonParseMethodInfo.Invoke(null, new object[] { json });
        return _jsonToStringMethodInfo.Invoke(jt, new object[] { 0, null }) as string;
    }

    private static Array GetIsoDateTimeConverterArray(string dateTimeFormat)
    {
        if (_jsonConverterType == null)
            throw new Exception("未加载【Newtonsoft.Json.dll】,请确保添加了该动态库");

        dynamic instance = Activator.CreateInstance(_isoDateTimeConverterType);

        if (instance == null)
            throw new Exception("创建实例失败");

        instance.DateTimeFormat = dateTimeFormat;

        var convertArray = Array.CreateInstance(_jsonConverterType, 1);
        convertArray.SetValue(instance, 0);
        return convertArray;
    }
}
