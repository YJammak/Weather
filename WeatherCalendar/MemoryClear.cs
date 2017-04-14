using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCalendar
{
    public static class MemoryClear
    {
        [DllImport("psapi.dll")]
        private static extern int EmptyWorkingSet(IntPtr hwProc);

        public static void Clear()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                //以下系统进程没有权限，所以跳过，防止出错影响效率。  
                if ((process.ProcessName == "System") && (process.ProcessName == "Idle"))
                    continue;
                try
                {
                    EmptyWorkingSet(process.Handle);
                }
                catch
                {
                    //
                }
            }
        }

        public static async Task ClearAsync()
        {
            await Task.Run(() =>
            {
                Clear();
            });
        }
    }
}
