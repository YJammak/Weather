using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;

namespace WeatherCalendar.Utils
{
    /// <summary>窗口辅助类</summary>
    public static class WindowHelper
    {
        private static readonly IntPtr HwndBottom = new IntPtr(1);
        
        [DllImport("user32")]
        private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);

        [DllImport("user32")]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        /// <summary>设置窗口位置</summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndInsertAfter"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            uint uFlags);

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

        /// <summary>设置窗口到最底层</summary>
        /// <param name="window"></param>
        public static void SetWindowBottom(this Window window) => 
            SetWindowPos(new WindowInteropHelper(window).Handle, HwndBottom, 0, 0, 0, 0, 19U);

    }
}
