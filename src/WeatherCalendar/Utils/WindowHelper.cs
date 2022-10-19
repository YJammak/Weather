using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WeatherCalendar.Utils;

/// <summary>窗口辅助类</summary>
public static class WindowHelper
{
    [DllImport("user32")]
    private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);

    [DllImport("user32")]
    private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

    [DllImport("user32")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32")]
    private static extern IntPtr FindWindowEx(IntPtr par1, IntPtr par2, string par3, string par4);

    [DllImport("user32")]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    /// <summary>窗口穿透</summary>
    /// <param name="w"></param>
    /// <param name="canPenetrate"></param>
    public static void SetWindowCanPenetrate(this Window w, bool canPenetrate)
    {
        var handle = new WindowInteropHelper(w).Handle;
        var windowLong = GetWindowLong(handle, -20);

        if (canPenetrate)
            SetWindowLong(handle, -20, windowLong | 32);
        else
            SetWindowLong(handle, -20, windowLong & -33);
    }

    /// <summary>设置窗口为工具窗口</summary>
    /// <param name="window"></param>
    public static void SetWindowToolWindow(this Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;
        SetWindowLong(handle, -20, GetWindowLong(handle, -20) | 128);
    }

    /// <summary>设置窗口到最底层）</summary>
    /// <param name="window"></param>
    public static void SetWindowBottom(this Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;
        SetParent(handle, GetDesktopPtr());
    }

    /// <summary>
    /// 获取桌面句柄
    /// </summary>
    /// <returns></returns>
    private static IntPtr GetDesktopPtr()
    {
        var hwndWorkerW = IntPtr.Zero;
        IntPtr hShellDefView;
        var hwndDesktop = IntPtr.Zero;
        var hProgMan = FindWindow("Progman", "Program Manager");

        if (hProgMan != IntPtr.Zero)
        {
            hShellDefView = FindWindowEx(hProgMan, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (hShellDefView != IntPtr.Zero)
            {
                hwndDesktop = FindWindowEx(hShellDefView, IntPtr.Zero, "SysListView32", null);
            }
        }

        if (hwndDesktop != IntPtr.Zero)
            return hwndDesktop;

        while (hwndDesktop == IntPtr.Zero)
        {
            hwndWorkerW = FindWindowEx(IntPtr.Zero, hwndWorkerW, "WorkerW", null);

            if (hwndWorkerW == IntPtr.Zero)
                break;
            hShellDefView = FindWindowEx(hwndWorkerW, IntPtr.Zero, "SHELLDLL_DefView", null);

            if (hShellDefView == IntPtr.Zero)
                continue;

            hwndDesktop = FindWindowEx(hShellDefView, IntPtr.Zero, "SysListView32", null);
        }

        return hwndDesktop;
    }
}
