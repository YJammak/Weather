using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;

namespace WeatherCalendar.Utils
{
    /// <summary>窗口辅助类</summary>
    public static class WindowHelper
    {
        [DllImport("user32")]
        private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);

        [DllImport("user32")]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

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
    }
}
