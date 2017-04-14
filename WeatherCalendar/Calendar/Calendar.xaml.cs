using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SharpSxwnl;
using Weather;

namespace WeatherCalendar
{
    public class DateInfo: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "no pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DateTime _dateTime;

        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                _dateTime = value;
                UpdateProperty();
            }
        }

        private OneDayWeather _weather;

        public OneDayWeather Weather
        {
            get => _weather;
            set
            {
                _weather = value;
                UpdateProperty();
            }
        }

        private DateTime _currentDate;
        /// <summary>
        /// 当前日期
        /// </summary>
        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                UpdateProperty();
            }
        }

        private WorkType _workType = WorkType.Unknown;
        /// <summary>
        /// 工作类型
        /// </summary>
        public WorkType WorkType
        {
            get => _workType;
            set
            {
                _workType = value;
                UpdateProperty();
            }
        }

        private OB _ob;

        public OB OB
        {
            get => _ob;
            set
            {
                _ob = value;
                UpdateProperty();
            }
        }

        private Lunar _lunar;

        public Lunar Lunar
        {
            get => _lunar;
            set
            {
                _lunar = value;
                UpdateProperty();
            }
        }
    }

    /// <summary>
    /// Calendar.xaml 的交互逻辑
    /// </summary>
    public partial class Calendar : UserControl, INotifyPropertyChanged
    {
        public event Action<object, int> CalendarRowsChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "no pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty WeatherInfoProperty =
            DependencyProperty.Register("WeatherInfo", typeof(WeatherInfo), typeof(Calendar), 
                new PropertyMetadata((o, args) =>
                {
                    var control = o as Calendar;
                    var weather = args.NewValue as WeatherInfo;

                    if (control == null)
                        return;

                    control.UpdateDateTimes(control.CurrentDate, weather);
                }));

        /// <summary>
        /// 天气信息
        /// </summary>
        public WeatherInfo WeatherInfo
        {
            get => (WeatherInfo) GetValue(WeatherInfoProperty);
            set => SetValue(WeatherInfoProperty, value);
        }

        public static readonly DependencyProperty CurrentDateProperty =
            DependencyProperty.Register("CurrentDate", typeof(DateTime), typeof(Calendar), 
                new PropertyMetadata((o, args) =>
                {
                    var control = o as Calendar;
                    var date = (DateTime)args.NewValue;

                    if (control == null)
                        return;

                    control.UpdateDateTimes(date, control.WeatherInfo);
                }));

        /// <summary>
        /// 当前日期
        /// </summary>
        public DateTime CurrentDate
        {
            get => (DateTime)GetValue(CurrentDateProperty);
            set => SetValue(CurrentDateProperty, value);
        }


        public static readonly DependencyProperty MouthMarginProperty =
            DependencyProperty.Register("MouthMargin", typeof(Thickness), typeof(Calendar), new PropertyMetadata());

        /// <summary>
        /// 月份位置
        /// </summary>
        public Thickness MouthMargin
        {
            get => (Thickness)GetValue(MouthMarginProperty);
            set => SetValue(MouthMarginProperty, value);
        }


        public static readonly DependencyProperty AllowAnimationProperty = DependencyProperty.Register(
            "AllowAnimation", typeof(bool), typeof(Calendar), new PropertyMetadata(false, (o, args) =>
            {
                try
                {
                    var control = o as Calendar;
                    var newAllowAnimation = (bool)args.NewValue;
                    var oldAllowAnimation = (bool)args.OldValue;

                    if (control == null)
                        return;

                    if (newAllowAnimation != oldAllowAnimation)
                        control.SetAllowAnimation(newAllowAnimation);
                }
                catch (Exception)
                {
                    // ignored
                }
            }));

        public bool AllowAnimation
        {
            get => (bool)GetValue(AllowAnimationProperty);
            set => SetValue(AllowAnimationProperty, value);
        }

        public static readonly DependencyProperty IsExtendedMonthProperty = DependencyProperty.Register(
            "IsExtendedMonth", typeof(bool), typeof(Calendar), new PropertyMetadata(false));

        public bool IsExtendedMonth
        {
            get => (bool)GetValue(IsExtendedMonthProperty);
            set => SetValue(IsExtendedMonthProperty, value);
        }

        public static readonly DependencyProperty MonthColorProperty = DependencyProperty.Register(
            "MonthColor", typeof(Color), typeof(Calendar), new PropertyMetadata(Colors.Transparent));

        public Color MonthColor
        {
            get => (Color)GetValue(MonthColorProperty);
            set => SetValue(MonthColorProperty, value);
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle", typeof(double), typeof(Calendar), new PropertyMetadata(0d));

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly DependencyProperty AxisProperty = DependencyProperty.Register(
            "Axis", typeof(Vector3D), typeof(Calendar), new PropertyMetadata(new Vector3D(0, 1, 0)));

        public Vector3D Axis
        {
            get => (Vector3D)GetValue(AxisProperty);
            set => SetValue(AxisProperty, value);
        }

        public static readonly DependencyProperty CameraPoint3DProperty = DependencyProperty.Register(
            "CameraPoint3D", typeof(Point3D), typeof(Calendar), new PropertyMetadata(new Point3D(0, 0, 560)));

        public Point3D CameraPoint3D
        {
            get => (Point3D)GetValue(CameraPoint3DProperty);
            set => SetValue(CameraPoint3DProperty, value);
        }

        /// <summary>
        /// 正常5行日期
        /// </summary>
        private readonly Thickness NormalMargin = new Thickness(0, -50, 0, 0);
        /// <summary>
        /// 扩展6行日期
        /// </summary>
        private readonly Thickness ExtendMargin = new Thickness(0);

        private List<DateInfo> _currentDateInfos;

        public List<DateInfo> CurrentDateInfos
        {
            get => _currentDateInfos;
            set
            {
                _currentDateInfos = value;
                UpdateProperty();
            }
        }

        private int _calendarRows;

        /// <summary>
        /// 日历行数
        /// </summary>
        public int CalendarRows
        {
            get => _calendarRows;
            set
            {
                _calendarRows = value;
                CalendarRowsChanged?.Invoke(this, value);
            }
        }

        public Calendar()
        {
            InitializeComponent();

            AllowAnimation = false;
        }

        private double angel = 0;
        public void PlayAnimation(bool random = true)
        {
            var storyboard = new Storyboard();

            var temp = AnimationHelper.GetNextInterval(1, 4);
            angel += AnimationHelper.GetNextBool() ? temp*90d : -temp*90d;
            var da = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(1000 + 800 * temp)),
                To = angel
            };

            storyboard.Children.Add(da);
            Storyboard.SetTarget(da, this);
            Storyboard.SetTargetProperty(da, new PropertyPath(AngleProperty));

            var paStart = new Point3DAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                To = new Point3D(0, 0, 1000)
            };

            storyboard.Children.Add(paStart);
            Storyboard.SetTarget(paStart, this);
            Storyboard.SetTargetProperty(paStart, new PropertyPath(CameraPoint3DProperty));

            var color = new ColorAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                To = Color.FromArgb(0x11, 0, 0 ,0)
            };

            storyboard.Children.Add(color);
            Storyboard.SetTarget(color, this);
            Storyboard.SetTargetProperty(color, new PropertyPath(MonthColorProperty));

            var axis = new Vector3DAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(500 + 400 * temp)),
                To = AnimationHelper.GetNextVector3D()
            };

            storyboard.Children.Add(axis);
            Storyboard.SetTarget(axis, this);
            Storyboard.SetTargetProperty(axis, new PropertyPath(AxisProperty));

            var paEnd = new Point3DAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                To = new Point3D(0, 0, 560),
                BeginTime = TimeSpan.FromMilliseconds(800 * temp)
            };


            storyboard.Children.Add(paEnd);
            Storyboard.SetTarget(paEnd, this);
            Storyboard.SetTargetProperty(paEnd, new PropertyPath(CameraPoint3DProperty));

            var colorBack = new ColorAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                To = Colors.Transparent,
                BeginTime = TimeSpan.FromMilliseconds(800 * temp)
            };


            storyboard.Children.Add(colorBack);
            Storyboard.SetTarget(colorBack, this);
            Storyboard.SetTargetProperty(colorBack, new PropertyPath(MonthColorProperty));


            var axisBack = new Vector3DAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(500 + 400 * temp)),
                To = new Vector3D(0, 1, 0),
                BeginTime = TimeSpan.FromMilliseconds(500 + 400 * temp)
            };

            storyboard.Children.Add(axisBack);
            Storyboard.SetTarget(axisBack, this);
            Storyboard.SetTargetProperty(axisBack, new PropertyPath(AxisProperty));

            if (random)
            {
                da.EasingFunction = AnimationHelper.GetEasingFunction();
                paStart.EasingFunction = AnimationHelper.GetEasingFunction();
                color.EasingFunction = paStart.EasingFunction;
                paEnd.EasingFunction = AnimationHelper.GetEasingFunction();
                colorBack.EasingFunction = paEnd.EasingFunction;
            }
            else
            {
                var e = new BackEase() { EasingMode = EasingMode.EaseInOut, Amplitude = 0.8 };
                da.EasingFunction = e;
                paStart.EasingFunction = e;
                paEnd.EasingFunction = e;
                color.EasingFunction = e;
                colorBack.EasingFunction = e;
            }

            storyboard.Begin();
        }

        private readonly DispatcherTimer animationTimer = new DispatcherTimer();
        private void Calendar_OnLoaded(object sender, RoutedEventArgs e)
        {
            CurrentDate = DateTime.Now;

            UpdateDateTimes(CurrentDate);

            HolidayHelper.Instance.HolidayChanged += HolidayInfo_HolidayChanged;

            animationTimer.Interval = TimeSpan.FromMilliseconds(AnimationHelper.GetNextInterval(5200, 1000 * 60));
            animationTimer.Tick += (o, args) =>
            {
                PlayAnimation();

                animationTimer.Interval = TimeSpan.FromMilliseconds(AnimationHelper.GetNextInterval(5200, 1000 * 60));
            };
        }

        private void SetAllowAnimation(bool allow)
        {
            if (allow)
                animationTimer.Start();
            else
                animationTimer.Stop();
        }

        private void HolidayInfo_HolidayChanged()
        {
            UpdateDateTimes(CurrentDate, WeatherInfo);
        }

        private void UpdateDateTimes(DateTime dateTime, WeatherInfo weatherInfo = null)
        {
            CurrentDateInfos = null;

            var tempDateInfos = new List<DateInfo>();

            if (dateTime.Date != DateTime.Now.Date)
            {
                dateTime = new DateTime(dateTime.Year, dateTime.Month, 1);
            }

            Lunar lastMouth= new Lunar(dateTime.AddMonths(-1));
            Lunar currentMouth = new Lunar(dateTime);
            Lunar nextMouth = new Lunar(dateTime.AddMonths(1));

            DateTime firstDayOfMouth = dateTime.AddDays(-dateTime.Day + 1);
            DateTime lastDayOfLastMouth = dateTime.AddDays(-dateTime.Day);

            int startDay = (int)firstDayOfMouth.DayOfWeek;
            int daysOfMounth = (firstDayOfMouth.AddMonths(1) - firstDayOfMouth).Days;

            if (startDay == 0)
                startDay = 7;

            DateTime startDate = firstDayOfMouth.AddDays(1 - startDay);
            DateTime endDate = startDate.AddDays(41);

            for (int i = 0; i < 35; i++)
            {
                var day = startDate.AddDays(i);
                Lunar mouth = null;
                var ob = lastMouth.GetOBOfDay(day);
                if (ob != null)
                    mouth = lastMouth;
                else
                {
                    ob = currentMouth.GetOBOfDay(day);
                    if (ob != null)
                        mouth = currentMouth;
                    else
                    {
                        ob = nextMouth.GetOBOfDay(day);
                        if (ob != null)
                            mouth = nextMouth;
                    }
                }
                var dateInfo = new DateInfo()
                {
                    DateTime = day,
                    CurrentDate = dateTime,
                    OB = ob,
                    Lunar = mouth
                };

                dateInfo.Weather = weatherInfo?.ForecastFifteenDays != null ? weatherInfo.ForecastFifteenDays.FirstOrDefault(w => w.DateTime.Date == dateInfo.DateTime.Date) : weatherInfo?.Forecast?.FirstOrDefault(w => w.DateTime.Date == dateInfo.DateTime.Date);

                tempDateInfos.Add(dateInfo);
            }

            if (startDate.AddDays(35).Month == dateTime.Month)
            {
                MouthMargin = ExtendMargin;
                for (int i = 0; i < 7; i++)
                {
                    var day = firstDayOfMouth.AddDays(i + 36 - startDay);
                    Lunar mouth = null;
                    var ob = lastMouth.GetOBOfDay(day);
                    if (ob != null)
                        mouth = lastMouth;
                    else
                    {
                        ob = currentMouth.GetOBOfDay(day);
                        if (ob != null)
                            mouth = currentMouth;
                        else
                        {
                            ob = nextMouth.GetOBOfDay(day);
                            if (ob != null)
                                mouth = nextMouth;
                        }
                    }
                    var dateInfo = new DateInfo()
                    {
                        DateTime = day,
                        CurrentDate = dateTime,
                        OB = ob,
                        Lunar = mouth
                    };
                    
                    dateInfo.Weather = weatherInfo?.ForecastFifteenDays.FirstOrDefault(w => w.DateTime.Date == dateInfo.DateTime.Date);
                    tempDateInfos.Add(dateInfo);
                    CalendarRows = 6;
                }

                IsExtendedMonth = true;
            }
            else
            {
                MouthMargin = NormalMargin;
                CalendarRows = 5;
                IsExtendedMonth = false;
            }

            CurrentDateInfos = tempDateInfos;
        }
    }

    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return new SolidColorBrush((Color)value);
            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
