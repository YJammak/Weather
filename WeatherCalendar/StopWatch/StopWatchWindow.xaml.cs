using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WeatherCalendar
{
    public class StopWatchInfo : INotifyPropertyChanged
    {
        public event Action<int> RecordsChanged;

        public event Action Reseted;

        public enum StopWatchStatus
        {
            Reset,

            Start,

            Stop,
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "No Pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private StopWatchStatus _status;

        public StopWatchStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                UpdateProperty();
            }
        }

        private int _stopWatchTime;

        public int StopWatchTime
        {
            get => _stopWatchTime;
            set
            {
                _stopWatchTime = value;
                UpdateProperty();
            }
        }

        public ObservableCollection<int> TimeRecords = new ObservableCollection<int>();

        private int startTime;
        private readonly Timer timer;
        private int lastTime = 0;

        public StopWatchInfo()
        {
            Status = StopWatchStatus.Reset;
            StopWatchTime = 0;
            timer = new Timer(10) {AutoReset = true};
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            StopWatchTime = Environment.TickCount - startTime + lastTime;
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            lastTime = StopWatchTime;
            startTime = Environment.TickCount;

            timer.Start();
            Status = StopWatchStatus.Start;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Stop()
        {
            timer.Stop();
            Status = StopWatchStatus.Stop;
        }

        /// <summary>
        /// 复位
        /// </summary>
        public void Reset()
        {
            Status = StopWatchStatus.Reset;
            lastTime = 0;
            StopWatchTime = 0;
            TimeRecords.Clear();
            Reseted?.Invoke();
        }

        /// <summary>
        /// 记录
        /// </summary>
        public void Record()
        {
            if (Status == StopWatchStatus.Start)
            {
                TimeRecords.Add(StopWatchTime);
                RecordsChanged?.Invoke(StopWatchTime);
            }
        }
    }

    /// <summary>
    /// StopWatchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StopWatchWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "No Pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private StopWatchInfo _stopWatch;

        public StopWatchInfo StopWatch
        {
            get => _stopWatch;
            set
            {
                _stopWatch = value;
                UpdateProperty();
            }
        }

        private StopWatchDataWindow swd;

        public StopWatchWindow()
        {
            InitializeComponent();
            StopWatch = new StopWatchInfo();
            StopWatch.RecordsChanged += (data) =>
            {
                swd?.AddRecord(data);
            };

            StopWatch.Reseted += () =>
            {
                swd?.ClearRecords();
            };
        }

        private void StopWatchWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Hide();
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            switch (StopWatch.Status)
            {
                case StopWatchInfo.StopWatchStatus.Reset:
                case StopWatchInfo.StopWatchStatus.Stop:
                    StopWatch.Start();
                    break;
                case StopWatchInfo.StopWatchStatus.Start:
                    StopWatch.Stop();
                    break;
            }
        }

        private void TimeButton_OnClick(object sender, RoutedEventArgs e)
        {
            switch (StopWatch.Status)
            {
                case StopWatchInfo.StopWatchStatus.Reset:
                case StopWatchInfo.StopWatchStatus.Stop:
                    StopWatch.Reset();
                    HideStopWatchDataWindow();
                    break;
                case StopWatchInfo.StopWatchStatus.Start:
                    ShowStopWatchDataWindow();
                    StopWatch.Record();
                    break;
            }
        }

        private void StopWatchWindow_OnLocationChanged(object sender, EventArgs e)
        {
            if (swd != null)
            {
                swd.Left = Left + Width - 9;
                swd.Top = Top;
            }
        }

        private void ShowStopWatchDataWindow()
        {
            if (swd == null)
            {
                swd = new StopWatchDataWindow();
            }

            swd.Left = Left + Width - 9;
            swd.Top = Top;

            swd.Show();
        }

        private void HideStopWatchDataWindow()
        {
            swd?.Hide();
        }

        private void StopWatchWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                switch (StopWatch.Status)
                {
                    case StopWatchInfo.StopWatchStatus.Reset:
                    case StopWatchInfo.StopWatchStatus.Stop:
                        StopWatch.Start();
                        break;
                    case StopWatchInfo.StopWatchStatus.Start:
                        ShowStopWatchDataWindow();
                        StopWatch.Record();
                        this.Activate();
                        break;
                }
            }
            else if (e.Key == Key.Space)
            {
                switch (StopWatch.Status)
                {
                    case StopWatchInfo.StopWatchStatus.Reset:
                    case StopWatchInfo.StopWatchStatus.Stop:
                        StopWatch.Start();
                        break;
                    case StopWatchInfo.StopWatchStatus.Start:
                        StopWatch.Stop();
                        break;
                }
            }
        }
    }

    public class StopWatchTimeToTime : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var time = (int) value/1000;

                return $"{time/60:D02}:{time%60:D02}";
            }
            catch (Exception)
            {
                return "00:00";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class StopWatchTimeToMillisecond : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var time = (int) value%1000;

                return $"{time/10:D02}";
            }
            catch (Exception)
            {
                return "00";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StopWatchInfoToStartButton : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                switch ((StopWatchInfo.StopWatchStatus)value)
                {
                    case StopWatchInfo.StopWatchStatus.Reset:
                    case StopWatchInfo.StopWatchStatus.Stop:
                        return "开始";
                    case StopWatchInfo.StopWatchStatus.Start:
                        return "停止";
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StopWatchInfoToRecordButton : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                switch ((StopWatchInfo.StopWatchStatus)value)
                {
                    case StopWatchInfo.StopWatchStatus.Reset:
                        return "";
                    case StopWatchInfo.StopWatchStatus.Stop:
                        return "复位";
                    case StopWatchInfo.StopWatchStatus.Start:
                        return "计次";
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class StopWatchInfoToAngle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var time = (int)value % 1000;

                return time / 1000.0 * 360;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
