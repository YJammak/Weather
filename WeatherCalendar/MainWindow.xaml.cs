using SharpSxwnl;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Weather;
using YUI.Controls;
using Timer = System.Timers.Timer;

namespace WeatherCalendar
{
    public enum PeriodOfDay
    {
        Day,

        Night,
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "no pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty PeriodOfDayProperty = DependencyProperty.Register(
            "PeriodOfDay", typeof(PeriodOfDay), typeof(MainWindow), new PropertyMetadata(default(PeriodOfDay)));

        public PeriodOfDay PeriodOfDay
        {
            get => (PeriodOfDay) GetValue(PeriodOfDayProperty);
            set => SetValue(PeriodOfDayProperty, value);
        }

        private bool _isEarlyMorning;
        

        public bool IsEarlyMorning
        {
            get => _isEarlyMorning;
            set
            {
                _isEarlyMorning = value;
                UpdateProperty();
            }
        }

        private WeatherInfo _weatherInfo;

        /// <summary>
        /// 当前天气信息
        /// </summary>
        public WeatherInfo CurrentWeatherInfo
        {
            get => _weatherInfo;
            set
            {
                _weatherInfo = value;
                UpdateProperty();
            }
        }

        private DateTime _dateTime;

        public DateTime CurrentDateTime
        {
            get => _dateTime;
            set
            {
                _dateTime = value;
                UpdateProperty();
            }
        }

        private DateTime _showDate;

        /// <summary>
        /// 显示时间
        /// </summary>
        public DateTime ShowDate
        {
            get => _showDate;
            set
            {
                _showDate = value;
                UpdateProperty();
            }
        }

        private DateTime _updateDateTime;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDateTime
        {
            get => _updateDateTime;
            set
            {
                _updateDateTime = value;
                UpdateProperty();
            }
        }

        private bool _showUpdateTime;

        /// <summary>
        /// 是否显示更新时间
        /// </summary>
        public bool ShowUpdateTime
        {
            get => _showUpdateTime;
            set
            {
                _showUpdateTime = value;
                UpdateProperty();
            }
        }

        private bool _isUpdatingWeather;

        /// <summary>
        /// 是否在更新天气信息
        /// </summary>
        public bool IsUpdatingWeather
        {
            get => _isUpdatingWeather;
            set
            {
                _isUpdatingWeather = value;
                UpdateProperty();
            }
        }

        private CountDown[] _countDowns;

        /// <summary>
        /// 倒计时
        /// </summary>
        public CountDown[] CountDowns
        {
            get => _countDowns;
            set
            {
                _countDowns = value;
                UpdateProperty();
            }
        }

        private Note[] _notes;

        public Note[] Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                UpdateProperty();
            }
        }

        private float _cpuLoad;

        public float CpuLoad
        {
            get => _cpuLoad;
            set
            {
                _cpuLoad = value;
                UpdateProperty();
            }
        }

        private float _memoryLoad;

        public float MemoryLoad
        {
            get => _memoryLoad;
            set
            {
                _memoryLoad = value;
                UpdateProperty();
            }
        }

        private bool _allowAnimation;

        public bool AllowAnimation
        {
            get => _allowAnimation;
            set
            {
                if (_allowAnimation != value)
                {
                    if (value)
                        animationTimer.Start();
                    else
                        animationTimer.Stop();
                }
                _allowAnimation = value;
                UpdateProperty();
            }
        }

        private NetWorkSpeedInfo _netWorkSpeed = new NetWorkSpeedInfo();

        public NetWorkSpeedInfo NetWorkSpeed
        {
            get => _netWorkSpeed;
            set
            {
                _netWorkSpeed = value;
                UpdateProperty();
            }
        }

        private ConfigInfo _config;

        public ConfigInfo Config
        {
            get => _config;
            set
            {
                _config = value;
                UpdateProperty();
            }
        }

        private TimeSpan _workTimeSpan;

        public TimeSpan WorkTimeSpan
        {
            get => _workTimeSpan;
            set
            {
                _workTimeSpan = value;
                UpdateProperty();
            }
        }

        private string _workTimeSpanFlag;

        public string WorkTimeSpanFlag
        {
            get => _workTimeSpanFlag;
            set
            {
                _workTimeSpanFlag = value;
                UpdateProperty();
            }
        }

        private bool _showWorkTime;

        public bool ShowWorkTime
        {
            get => _showWorkTime;
            set
            {
                _showWorkTime = value;
                UpdateProperty();
            }
        }

        public MainWindow()
        {
            InitializeComponent();


            ShowUpdateTime = false;

            LoadConfig();

            foreach (var holidayInfoRestHoliday in HolidayHelper.Instance.HolidayInfo.RestHolidays)
            {
                CountDownHelper.Instance.AddCountDown(new CountDown
                {
                    Date = holidayInfoRestHoliday.FirstDay,
                    Remarks = holidayInfoRestHoliday.Name
                });
            }
            CountDownHelper.Instance.CountDownsChanged += UpdateCountDowns;
            NotesHelper.Instance.NotesChanged += UpdateNotes;
            Task.Factory.StartNew(UpdateWeather);
            CurrentDateTime = DateTime.Now;
            ShowDate = DateTime.Now;
            UpdateCountDowns();
            UpdateNotes();

            Config = ConfigHelper.Instance.Config;

            UpdateWorkTimeSpan();

            AllowAnimation = true;

            Timer t = new Timer();
            t.AutoReset = true;
            t.Interval = 1000;
            t.Elapsed += (sender, args) =>
            {
                var LastDateTime = CurrentDateTime;
                CurrentDateTime = DateTime.Now;

                var updateWeather = false;

                UpdateSystenInfo();

                ConfigAnimation();

                if (CurrentDateTime.Date != LastDateTime.Date)
                {
                    ShowDate = CurrentDateTime;
                    Dispatcher.Invoke(() =>
                    {
                        calendar.CurrentDate = ShowDate;
                    });
                    updateWeather = true;
                    UpdateWorkTimeSpan();
                }
                else if (CurrentDateTime.Hour != LastDateTime.Hour)
                {
                    UpdateCountDowns();
                    UpdateNotes();
                    updateWeather = true;
                    UpdateWorkTimeSpan();
                }
                else if (CurrentDateTime.Minute != LastDateTime.Minute)
                {
                    if (CurrentDateTime.Minute % 15 == 0)
                        updateWeather = true;

                    UpdateWorkTimeSpan();
                }

                if (updateWeather)
                {
                    UpdatePeriod();
                    UpdateWeather();
                }
            };
            t.Start();

            InitalNotifyIcon();
        }

        private void UpdatePeriod()
        {
            Dispatcher.Invoke(() =>
            {
                if (CurrentDateTime.Hour >= 8 && CurrentDateTime.Hour < 20)
                    PeriodOfDay = PeriodOfDay.Day;
                else
                    PeriodOfDay = PeriodOfDay.Night;

                IsEarlyMorning = CurrentDateTime.Hour < 8;
            });
        }

        private void ConfigAnimation()
        {
            //if (CpuLoad > 70)
            //{
            //    AllowAnimation = false;
            //}
            //else if (CpuLoad < 30)
            //{
            //    AllowAnimation = true;
            //}
        }

        private void UpdateWorkTimeSpan()
        {
            if (Config?.WorkTime == null)
                return;

            var current = DateTime.Now;

            var isWorkDay = !(current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday);

            foreach (var holiday in HolidayHelper.Instance.HolidayInfo.RestHolidays)
            {
                if (holiday.WorkDay != null && holiday.WorkDay.Any(work => current.Date == work.Date))
                {
                    isWorkDay = true;
                    break;
                }

                if (holiday.RestDay == null) continue;

                if (holiday.RestDay.Any(work => current.Date == work.Date))
                {
                    isWorkDay = false;
                    break;
                }
            }

            var isShow = isWorkDay && Config.IsShowWorkTime;

            try
            {
                if (!isShow)
                    return;

                if (current.TimeOfDay < Config.WorkTime.StartTime.TimeOfDay)
                {
                    WorkTimeSpanFlag = "上班";
                    WorkTimeSpan = Config.WorkTime.StartTime.TimeOfDay - current.TimeOfDay + new TimeSpan(0, 1, 0);
                    return;
                }

                if (current.TimeOfDay < Config.WorkTime.EndTime.TimeOfDay)
                {
                    WorkTimeSpanFlag = "下班";
                    WorkTimeSpan = Config.WorkTime.EndTime.TimeOfDay - current.TimeOfDay + new TimeSpan(0, 1, 0);
                    return;
                }

                isShow = false;
            }
            finally 
            {
                ShowWorkTime = isShow;
            }
        }

        private void LoadConfig()
        {
            HolidayHelper.Instance.LoadHolidayInfo();
            CountDownHelper.Instance.Load();
            NotesHelper.Instance.Load();
            ConfigHelper.Instance.Load();
        }

        private AboutWindow aw;

        private RestHolidaySetting rh;

        private GetCityWindow gc;

        private StopWatchWindow sw;

        private void InitalNotifyIcon()
        {
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;

            var notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = @"天气日历";
            notifyIcon.Visible = true;
            notifyIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "NotifyIcon.ico");

            var notifyIconUpdateMenu = new System.Windows.Forms.MenuItem("更新天气");
            notifyIconUpdateMenu.Click += (sender, args) =>
            {
                LoadConfig();
                Task.Factory.StartNew(UpdateWeather);
            };
            var notifyIconTodayMenu = new System.Windows.Forms.MenuItem("今天");
            notifyIconTodayMenu.Click += (sender, args) =>
            {
                ShowDate = DateTime.Now;
                calendar.CurrentDate = ShowDate;
            };
            var notifyIconLastMouthMenu = new System.Windows.Forms.MenuItem("上个月");
            notifyIconLastMouthMenu.Click += (sender, args) =>
            {
                ShowDate = ShowDate.AddMonths(-1);
                calendar.CurrentDate = ShowDate;
            };
            var notifyIconNextMouthMenu = new System.Windows.Forms.MenuItem("下个月");
            notifyIconNextMouthMenu.Click += (sender, args) =>
            {
                ShowDate = ShowDate.AddMonths(1);
                calendar.CurrentDate = ShowDate;
            };
            var notifyIconChangeCityMenu = new System.Windows.Forms.MenuItem("更换城市");
            notifyIconChangeCityMenu.Click += (sender, args) =>
            {
                ChangeCity();
            };

            var notifyIconRestHolidayMenu = new System.Windows.Forms.MenuItem("日历详情");
            notifyIconRestHolidayMenu.Click += (sender, args) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (rh == null)
                    {
                        rh = new RestHolidaySetting();

                        rh.UpdateWeather += () =>
                        {
                            LoadConfig();
                            Task.Factory.StartNew(UpdateWeather);
                        };

                        rh.ChangeCity += ChangeCity;

                        rh.StopWatchOpened += ShowStopWatchWindow;
                    }

                    rh.Show();
                    rh.Activate();
                });
            };

            notifyIconRestHolidayMenu.DefaultItem = true;

            var notifyIconStopWatchMenu = new System.Windows.Forms.MenuItem("秒表");

            notifyIconStopWatchMenu.Click += (sender, args) =>
            {
                ShowStopWatchWindow();
            };

            var notifyIconAboutMenu = new System.Windows.Forms.MenuItem("关于");
            notifyIconAboutMenu.Click += (sender, args) =>
            {
                if (aw == null)
                    aw = new AboutWindow();

                aw.Show();
                aw.Activate();
            };

            var notifyIconCloseMenu = new System.Windows.Forms.MenuItem("退出");
            notifyIconCloseMenu.Click += (sender, args) =>
            {
                notifyIcon.Visible = false;
                Environment.Exit(0);
            };

            var notifyIconMemoryClearMenu = new System.Windows.Forms.MenuItem("内存整理");
            notifyIconMemoryClearMenu.Click += async (sender, args) =>
            {
                notifyIconMemoryClearMenu.Enabled = false;
                await MemoryClear.ClearAsync();
                notifyIconMemoryClearMenu.Enabled = true;
            };

            var notifyIconWorkClearMenu = new System.Windows.Forms.MenuItem("上下班设置");
            notifyIconWorkClearMenu.Click += (sender, args) =>
            {
                var sww = new SetWorkTimeWindow();
                sww.ConfigChanged += () =>
                {
                    Config = ConfigHelper.Instance.Config;
                    UpdateWorkTimeSpan();
                };
                sww.ShowDialog();
            };

            var notifyIconCanMoveMenu = new System.Windows.Forms.MenuItem("锁定位置");
            notifyIconCanMoveMenu.Checked = !Config.CanMove;
            notifyIconCanMoveMenu.Click += (sender, args) =>
            {
                notifyIconCanMoveMenu.Checked = !notifyIconCanMoveMenu.Checked;
                Config.CanMove = !notifyIconCanMoveMenu.Checked;
                ConfigHelper.Instance.Save();
            };

            var notifyIconCanPenetrateMenu = new System.Windows.Forms.MenuItem("鼠标穿透");
            notifyIconCanPenetrateMenu.Checked = Config.CanPenetrate;
            notifyIconCanPenetrateMenu.Click += (sender, args) =>
            {
                notifyIconCanPenetrateMenu.Checked = !notifyIconCanPenetrateMenu.Checked;
                Config.CanPenetrate = notifyIconCanPenetrateMenu.Checked;
                this.SetWindowCanPenetrate(Config.CanPenetrate);
                ConfigHelper.Instance.Save();
            };

            var notifyIconTransparentMenu = new System.Windows.Forms.MenuItem("背景透明");
            notifyIconTransparentMenu.Checked = Config.TransParent;
            notifyIconTransparentMenu.Click += (sender, args) =>
            {
                notifyIconTransparentMenu.Checked = !notifyIconTransparentMenu.Checked;
                Config.TransParent = notifyIconTransparentMenu.Checked;

                if (Config.TransParent)
                    mainBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                else
                    mainBorder.Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));

                ConfigHelper.Instance.Save();
            };

            var notifyIconMenu = new[]
            {
                notifyIconRestHolidayMenu,
                notifyIconCanPenetrateMenu,
                notifyIconTransparentMenu,
                notifyIconStopWatchMenu,
                new System.Windows.Forms.MenuItem("-"),
                notifyIconMemoryClearMenu,
                new System.Windows.Forms.MenuItem("-"),
                notifyIconUpdateMenu,
                notifyIconChangeCityMenu,
                notifyIconWorkClearMenu,
                new System.Windows.Forms.MenuItem("-"),
                notifyIconTodayMenu,
                notifyIconLastMouthMenu,
                notifyIconNextMouthMenu,
                new System.Windows.Forms.MenuItem("-"),
                notifyIconAboutMenu,
                new System.Windows.Forms.MenuItem("-"),
                notifyIconCloseMenu
            };

            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(notifyIconMenu);
            notifyIcon.MouseClick += (sender, args) =>
            {
                if (args.Button == MouseButtons.Right)
                    return;

                notifyIconRestHolidayMenu.PerformClick();
            };

            notifyIcon.DoubleClick += (sender, args) =>
            {
                LoadConfig();

                Task.Factory.StartNew(UpdateWeather);
                notifyIconRestHolidayMenu.PerformClick();
            };
        }

        private void ShowStopWatchWindow()
        {
            if (sw == null)
            {
                sw = new StopWatchWindow();
            }

            sw.Show();
            sw.Activate();
        }

        private void ChangeCity()
        {
            if (gc == null)
            {
                gc = new GetCityWindow();
                gc.CityChanged += async s =>
                {
                    try
                    {
                        await Task.Factory.StartNew(UpdateWeatherWithException, s);
                        HolidayHelper.Instance.HolidayInfo.CityKey = s.CityKey;
                        HolidayHelper.Instance.SaveHolidayInfo();
                    }
                    catch (Exception ex)
                    {
                        MessageWindow.Show(ex.Message, "更新天气");
                    }
                };
            }
            gc.Text = "";
            gc.Show();
            gc.Activate();
        }

        private void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //if (Math.Abs(Left - (SystemParameters.WorkArea.Width - ActualWidth)) > 1)
                //    Left = SystemParameters.WorkArea.Width - ActualWidth;
                //if (Math.Abs(Top - SystemParameters.WorkArea.Top) > 1)
                //    Top = SystemParameters.WorkArea.Top;

                // ReSharper disable once ConvertClosureToMethodGroup
                this.SetWindowBottom();
            });
        }

        public void UpdateWeatherWithException(object city)
        {
            if (IsUpdatingWeather)
                throw new Exception("正在更新天气，请稍后再试。");

            try
            {
                var cityinfo = city as CityKeyInfo;
                if (cityinfo == null)
                    throw new Exception("城市信息不正确");

                IsUpdatingWeather = true;
                WeatherHelper.Instance.UpdateWeather(cityinfo);

                if (!WeatherHelper.Instance.WeatherInfo.IsValid)
                {
                    throw new Exception($"更新天气数据失败!\r\n请确定选择的城市名（{cityinfo.City}）正确。");
                }

                CurrentWeatherInfo = WeatherHelper.Instance.WeatherInfo;

                UpdateDateTime = DateTime.Now;

                if (Math.Abs((ShowDate.Date - DateTime.Now).Days) < 20)
                    ShowDate = DateTime.Now;
            }
            finally
            {
                IsUpdatingWeather = false;
            }
        }

        public void UpdateCountDowns()
        {
            //CountDowns = CountDownHelper.Instance.GetTopCountDowns(2);
        }

        public void UpdateNotes()
        {
            Notes = NotesHelper.Instance.GetNotes(DateTime.Now);
        }

        public void UpdateSystenInfo()
        {
            CpuLoad = SystemInfo.CpuLoad;
            MemoryLoad = SystemInfo.MemoryLoad;
            NetWorkSpeed = SystemInfo.NetWpSpeed;
        }

        public void UpdateWeather()
        {
            if (IsUpdatingWeather)
                return;

            try
            {
                UpdateCountDowns();

                IsUpdatingWeather = true;
                WeatherHelper.Instance.UpdateWeather(HolidayHelper.Instance.HolidayInfo.City);

                CurrentWeatherInfo = WeatherHelper.Instance.WeatherInfo.IsValid ? WeatherHelper.Instance.WeatherInfo : null;

                UpdateDateTime = DateTime.Now;

                if (Math.Abs((ShowDate.Date - DateTime.Now).Days) < 20)
                    ShowDate = DateTime.Now;
            }
            finally
            {
                IsUpdatingWeather = false;
            }
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Config.CanMove)
                DragMove();
        }


        private void MainWindow_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Config.Top = Top;
            Config.Left = Left;
            ConfigHelper.Instance.Save();
        }

        private double angle = 0;
        private readonly DispatcherTimer animationTimer = new DispatcherTimer();

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Config.TransParent)
                mainBorder.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
            else
                mainBorder.Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));

            Left = Config.Left;
            Top = Config.Top;
            this.SetWindowCanPenetrate(Config.CanPenetrate);
            this.SetWindowToolWindow();
            this.SetWindowBottom();

            animationTimer.Interval =
                TimeSpan.FromMilliseconds(AnimationHelper.GetNextInterval(AnimationHelper.AnimationSpeed.Fast));
            animationTimer.Tick += (o, args) =>
            {
                angle += AnimationHelper.GetNextBool() ? 180 : -180;
                var da = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(AnimationHelper.GetNextInterval(1000, 5000))),
                    To = angle,
                    EasingFunction = AnimationHelper.GetEasingFunction()
                };

                aar.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);

                animationTimer.Interval = TimeSpan.FromMilliseconds(AnimationHelper.GetNextInterval(5200, 10000));
            };

            animationTimer.Start();
        }


        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Left = SystemParameters.WorkArea.Width - ActualWidth;
            //Top = SystemParameters.WorkArea.Top;
            this.SetWindowBottom();
        }

        private void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Normal)
                WindowState = WindowState.Normal;

            this.SetWindowBottom();
        }
    }

    /// <summary>
    /// 日期转生肖
    /// </summary>
    public class DateTimeToAnimalSignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var dt = (DateTime) value;

                Lunar lunar = new Lunar(dt);
                return lunar.GetOBOfDay(dt).LShX2;
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

    /// <summary>
    /// 日期转生肖字体
    /// </summary>
    public class DateTimeToAnimalSignFontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var dt = (DateTime) value;

                Lunar lunar = new Lunar(dt);
                var a = lunar.GetOBOfDay(dt).LShX2;

                switch (a)
                {
                    case "鼠":
                        return "\ue800";
                    case "牛":
                        return "\ue801";
                    case "虎":
                        return "\ue802";
                    case "兔":
                        return "\ue803";
                    case "龙":
                        return "\ue804";
                    case "蛇":
                        return "\ue805";
                    case "马":
                        return "\ue806";
                    case "羊":
                        return "\ue807";
                    case "猴":
                        return "\ue808";
                    case "鸡":
                        return "\ue809";
                    case "狗":
                        return "\ue80a";
                    case "猪":
                        return "\ue80b";
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


    /// <summary>
    /// 日期转农历日期
    /// </summary>
    public class DateTimeToLunarDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var dt = (DateTime) value;

                Lunar lunar = new Lunar(dt);
                var ob = lunar.GetOBOfDay(dt);

                return ob.LMouthInfo;
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

    /// <summary>
    /// 日期转天干地支
    /// </summary>
    public class DateTimeToChineseEraConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var dt = (DateTime) value;

                Lunar lunar = new Lunar(dt);
                var ob = lunar.GetOBOfDay(dt);

                switch (parameter.ToString())
                {
                    case "年":
                        return ob.Lyear3;
                    case "月":
                        return ob.Lmonth2;
                    case "日":
                        return ob.Lday2;
                    case "时":
                        return ob.Ltime2;
                }

                return ob.Lyear3;
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

    public class FloatToIntRateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var f = (float) value;

                return $"{(int) f:D} %";
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

    public class NetWorkSpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var f = (double) value;

                if (f < 0)
                    f = 0d;

                var units = new[] {"KB/s", "MB/s", "TB/s"};

                var unit = units[0];

                for (int i = 0; i < 2; i++)
                {
                    if (f >= 999.5)
                    {
                        f /= 1024d;
                        unit = units[i + 1];
                    }
                }

                return $"{GetValue(f)} {unit}";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetValue(double value)
        {
            if (value <= 0)
                return "0";

            if (value < 9.5)
                return $"{value:F2}";

            if (value < 99.5)
                return $"{value:F1}";

            return $"{value:F0}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return GetValue((double) value);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetValue(double value)
        {
            if (value < 9.5)
                return $"{value:F2}";

            if (value < 99.5)
                return $"{value:F1}";

            return $"{value:F0}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CountDownTimerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var ts = (TimeSpan) value;

                var p = parameter as string;

                if (string.IsNullOrEmpty(p))
                    return ts.Days;

                if (p.ToUpper() == "D")
                {
                    if (ts.Hours == 23)
                        return ts.Days + 1;

                    return ts.Days;
                }

                if (p.ToUpper() == "H")
                {
                    if (ts.Hours == 23)
                        return 0;

                    return ts.Hours + 1;
                }

                return ts.Days;
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
}
