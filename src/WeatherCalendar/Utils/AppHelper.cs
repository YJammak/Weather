using Anotar.NLog;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows;

namespace WeatherCalendar.Utils;

public static class AppHelper
{
    private static string AppName => "WeatherCalendar";

    private static string AppFullName => Process.GetCurrentProcess()!.MainModule!.FileName;

    public static bool IsAutoStart()
    {
        return IsExistKey(AppName);
    }

    public static bool SetAutoStart(bool isAutoStart)
    {
        if (IsAdministrator())
            return SetAutoStart(isAutoStart, AppName, AppFullName);

        RequiredAdministrator();
        return false;
    }

    public static void Restart()
    {
        try
        {
            var process = new ProcessStartInfo
            {
                FileName = AppFullName,
                UseShellExecute = true
            };

            if (IsAdministrator())
                process.Verb = "runas";

            Process.Start(process);
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            LogTo.ErrorException("重启失败", ex);
        }
    }

    public static void RequiredAdministrator()
    {
        if (IsAdministrator())
            return;

        try
        {
            var process = new ProcessStartInfo
            {
                FileName = AppFullName,
                Verb = "runas",
                UseShellExecute = true
            };

            Process.Start(process);
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            LogTo.ErrorException("请求管理员权限失败", ex);
        }
    }

    public static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    /// <summary>
    /// 判断注册键值对是否存在，即是否处于开机启动状态
    /// </summary>
    /// <param name="keyName">键值名</param>
    /// <returns></returns>
    private static bool IsExistKey(string keyName)
    {
        try
        {
            var local = Registry.LocalMachine;
            var runs = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (runs == null)
            {
                var key2 = local.CreateSubKey("SOFTWARE");
                var key3 = key2!.CreateSubKey("Microsoft");
                var key4 = key3!.CreateSubKey("Windows");
                var key5 = key4!.CreateSubKey("CurrentVersion");
                var key6 = key5!.CreateSubKey("Run");
                runs = key6;
            }
            var runsName = runs!.GetValueNames();
            return runsName.Any(strName => string.Equals(strName, keyName, StringComparison.CurrentCultureIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 写入或删除注册表键值对,即设为开机启动或开机不启动
    /// </summary>
    /// <returns></returns>
    private static bool SetAutoStart(bool isAutoStart, string appName, string appFullName)
    {
        try
        {
            var local = Registry.LocalMachine;
            var key = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true) ??
                      local.CreateSubKey("SOFTWARE//Microsoft//Windows//CurrentVersion//Run");

            //若开机自启动则添加键值对
            if (isAutoStart)
            {
                key!.SetValue(appName, appFullName);
                key.Close();
            }
            else//否则删除键值对
            {
                var keyNames = key!.GetValueNames();
                foreach (var keyName in keyNames)
                {
                    if (!string.Equals(keyName, appName, StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    key.DeleteValue(appName);
                    key.Close();
                }
            }
        }
        catch (Exception ex)
        {
            LogTo.ErrorException("修改开启自启注册表失败", ex);
            return false;
        }

        return true;
    }
}
