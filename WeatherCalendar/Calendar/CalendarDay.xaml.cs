using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Weather;

namespace WeatherCalendar
{
    /// <summary>
    /// CalendarDay.xaml 的交互逻辑
    /// </summary>
    public partial class CalendarDay : UserControl, INotifyPropertyChanged
    {
        public event Action HolidayChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateProperty([CallerMemberName] string propertyName = "no pass")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty DateInfoProperty =
            DependencyProperty.Register("DateInfo", typeof(DateInfo), typeof(CalendarDay),
                new PropertyMetadata(null, (o, args) =>
                {
                    var control = o as CalendarDay;
                    var weather = args.NewValue as DateInfo;

                    if (control == null)
                        return;

                    control.IsEnabled = weather != null;

                    control.UpdateMark();

                    control.UpdateNotes();

                    try
                    {
                        //control.Image = weather?.Weather == null ? null : new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Weathers/{weather.Weather.Weather}.png", UriKind.RelativeOrAbsolute));
                        if (weather?.Weather == null)
                            control.DayImage = null;
                        else
                        {
                            BitmapImage bitmap = new BitmapImage();
                            var imagePath =
                                $"{AppDomain.CurrentDomain.BaseDirectory}Weathers/{WeatherConvert.Convert(weather.Weather.DayWeather.Weather)}.png";
                            if (File.Exists(imagePath))
                            {
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                                using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                                {
                                    bitmap.StreamSource = ms;
                                    bitmap.EndInit();
                                    bitmap.Freeze();
                                }

                                control.DayImage = bitmap;
                            }
                            else
                            {
                                control.DayImage = null;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        control.DayImage = null;
                    }


                    try
                    {
                        //control.Image = weather?.Weather == null ? null : new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}Weathers/{weather.Weather.Weather}.png", UriKind.RelativeOrAbsolute));
                        if (weather?.Weather == null)
                            control.NightImage = null;
                        else
                        {
                            BitmapImage bitmap = new BitmapImage();
                            var imagePath =
                                $"{AppDomain.CurrentDomain.BaseDirectory}Weathers/{WeatherConvert.Convert(weather.Weather.NightWeather.Weather)}_夜.png";
                            if (File.Exists(imagePath))
                            {
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                                using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                                {
                                    bitmap.StreamSource = ms;
                                    bitmap.EndInit();
                                    bitmap.Freeze();
                                }

                                control.NightImage = bitmap;
                            }
                            else
                            {
                                control.NightImage = null;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        control.NightImage = null;
                    }
                }));

        /// <summary>
        /// 时间
        /// </summary>
        public DateInfo DateInfo
        {
            get => (DateInfo) GetValue(DateInfoProperty);
            set => SetValue(DateInfoProperty, value);
        }

        public static readonly DependencyProperty DayImageProperty = DependencyProperty.Register(
            "DayImage", typeof(ImageSource), typeof(CalendarDay), new PropertyMetadata(default(ImageSource)));

        public ImageSource DayImage
        {
            get => (ImageSource) GetValue(DayImageProperty);
            set => SetValue(DayImageProperty, value);
        }

        public static readonly DependencyProperty NightImageProperty = DependencyProperty.Register(
            "NightImage", typeof(ImageSource), typeof(CalendarDay), new PropertyMetadata(default(ImageSource)));

        public ImageSource NightImage
        {
            get => (ImageSource) GetValue(NightImageProperty);
            set => SetValue(NightImageProperty, value);
        }

        public static readonly DependencyProperty AllowAnimationProperty = DependencyProperty.Register(
            "AllowAnimation", typeof(bool), typeof(CalendarDay), new PropertyMetadata(false, (o, args) =>
            {
                try
                {
                    var control = o as CalendarDay;
                    var allowAnimation = (bool) args.NewValue;

                    if (control == null)
                        return;

                    control.SetAllowAnimation(allowAnimation);
                }
                catch (Exception)
                {
                    // ignored
                }
            }));

        public bool AllowAnimation
        {
            get => (bool) GetValue(AllowAnimationProperty);
            set => SetValue(AllowAnimationProperty, value);
        }


        public static readonly DependencyProperty CameraPoint3DProperty = DependencyProperty.Register(
            "CameraPoint3D", typeof(Point3D), typeof(CalendarDay), new PropertyMetadata(new Point3D(0, 0, 500)));

        public Point3D CameraPoint3D
        {
            get => (Point3D) GetValue(CameraPoint3DProperty);
            set => SetValue(CameraPoint3DProperty, value);
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle", typeof(double), typeof(CalendarDay), new PropertyMetadata(0d));

        public double Angle
        {
            get => (double) GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly DependencyProperty AxisProperty = DependencyProperty.Register(
            "Axis", typeof(Vector3D), typeof(CalendarDay), new PropertyMetadata(new Vector3D(0, 1, 0)));

        public Vector3D Axis
        {
            get => (Vector3D) GetValue(AxisProperty);
            set => SetValue(AxisProperty, value);
        }

        private bool _hasMark;

        public bool HasMark
        {
            get => _hasMark;
            set
            {
                _hasMark = value;
                UpdateProperty();
            }
        }

        private List<Note> _notes;

        public List<Note> Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                UpdateProperty();
            }
        }

        public CalendarDay()
        {
            InitializeComponent();

            AllowAnimation = false;

            MarksHelper.Instance.MarksChanged += UpdateMark;

            UpdateMark();

            NotesHelper.Instance.NotesChanged += UpdateNotes;

            UpdateNotes();
        }

        private void UpdateNotes()
        {
            if (DateInfo == null)
                return;

            var notes = NotesHelper.Instance.GetNotes(DateInfo.DateTime);

            if (notes == null)
            {
                Notes = null;
                return;
            }

            var notesList = notes.ToList();
            notesList.Sort();

            Notes = notesList;
        }

        private void UpdateMark()
        {
            if (DateInfo != null)
                HasMark = MarksHelper.Instance.GetMark(DateInfo.DateTime) != null;
            else
                HasMark = false;
        }

        private void RestHolidayMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var holiday = HolidayHelper.Instance.HolidayInfo.GetRestHoliday(DateInfo.DateTime);
            var info = new RestHolidayInfo
            {
                Name = holiday == null ? "" : holiday.Name,
                Date = DateInfo.DateTime,
                IsRest = holiday?.RestDay?.Contains(DateInfo.DateTime, new DateTimeComparer()) ?? false
            };
            SetRestHolidayWindow sh = new SetRestHolidayWindow(info);
            sh.ShowDialog();
        }

        private void ChineseHolidayMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            SetHolidayWindow sh = new SetHolidayWindow(DateInfo.DateTime, true);
            sh.ShowDialog();
        }

        private void HolidayMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            SetHolidayWindow sh = new SetHolidayWindow(DateInfo.DateTime, false);
            sh.ShowDialog();
        }

        private void Border_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //border.ContextMenu.IsOpen = true;
        }

        private void SetNotesMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            SetNoteWindow snw = new SetNoteWindow(DateInfo.DateTime);
            snw.ShowDialog();
        }

        private readonly DispatcherTimer animationTimer = new DispatcherTimer();

        private void CalendarDay_OnLoaded(object sender, RoutedEventArgs e)
        {
            animationTimer.Interval = TimeSpan.FromMilliseconds(AnimationHelper.GetNextInterval(3200, 1000*600));
            animationTimer.Tick += (o, args) =>
            {
                PlayAnimation();

                animationTimer.Interval = TimeSpan.FromMilliseconds(AnimationHelper.GetNextInterval(3200, 1000*600));
            };

            SetAllowAnimation(AllowAnimation);
        }

        private double angle = 0;

        public void PlayAnimation(bool random = true)
        {
            if (!AllowAnimation)
                return;

            var storyboard = new Storyboard();

            var pa = new Point3DAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(600),
                To = new Point3D(0, 0, 800)
            };

            Storyboard.SetTarget(pa, this);
            Storyboard.SetTargetProperty(pa, new PropertyPath(CameraPoint3DProperty));

            storyboard.Children.Add(pa);

            var aa = new Vector3DAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(1500)),
                To = AnimationHelper.GetNextVector3D()
            };

            Storyboard.SetTarget(aa, this);
            Storyboard.SetTargetProperty(aa, new PropertyPath(AxisProperty));

            storyboard.Children.Add(aa);

            angle += AnimationHelper.GetNextBool() ? 360d : -360d;
            //angle += AnimationHelper.GetNextAngle();
            var da = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(3000)),
                To = angle
            };

            Storyboard.SetTarget(da, this);
            Storyboard.SetTargetProperty(da, new PropertyPath(AngleProperty));

            storyboard.Children.Add(da);

            var paEnd = new Point3DAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(600),
                To = new Point3D(0, 0, 500),
                BeginTime = TimeSpan.FromMilliseconds(2400)
            };

            Storyboard.SetTarget(paEnd, this);
            Storyboard.SetTargetProperty(paEnd, new PropertyPath(CameraPoint3DProperty));

            storyboard.Children.Add(paEnd);

            var aaEnd = new Vector3DAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(1500)),
                To = new Vector3D(0, 1, 0),
                BeginTime = TimeSpan.FromMilliseconds(1500)
            };

            Storyboard.SetTarget(aaEnd, this);
            Storyboard.SetTargetProperty(aaEnd, new PropertyPath(AxisProperty));

            storyboard.Children.Add(aaEnd);

            if (random)
            {
                da.EasingFunction = AnimationHelper.GetEasingFunction();
                pa.EasingFunction = AnimationHelper.GetEasingFunction();
                paEnd.EasingFunction = AnimationHelper.GetEasingFunction();
                aa.EasingFunction = AnimationHelper.GetEasingFunction();
                aaEnd.EasingFunction = AnimationHelper.GetEasingFunction();
            }
            else
            {
                var easing = new BackEase() {EasingMode = EasingMode.EaseInOut, Amplitude = 0.8};
                da.EasingFunction = easing;
                pa.EasingFunction = easing;
                paEnd.EasingFunction = easing;
                aa.EasingFunction = easing;
                aaEnd.EasingFunction = easing;
            }

            storyboard.Begin(this);
        }

        private void SetAllowAnimation(bool allow)
        {
            if (allow)
                animationTimer.Start();
            else
                animationTimer.Stop();
        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            //SetAllowAnimation(false);
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            //SetAllowAnimation(AllowAnimation);
        }

        private void CountDownMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var countDown = CountDownHelper.Instance.GetCountDown(DateInfo.DateTime);

            SetCountDownWindow sw = new SetCountDownWindow();

            sw.RemarksChanged += (r) =>
            {
                if (string.IsNullOrEmpty(r))
                    CountDownHelper.Instance.RemoveCountDown(DateInfo.DateTime);
                else
                    CountDownHelper.Instance.AddCountDown(DateInfo.DateTime, r);

                CountDownHelper.Instance.Save();
            };

            if (countDown != null)
                sw.Remarks = countDown.Remarks;

            sw.ShowDialog();
        }

        private void ContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            countDownMenuItem.IsEnabled = DateInfo?.DateTime.Date >= DateTime.Today;
            setmarkMenuItem.Header = HasMark ? "取消标记" : "设置标记";
        }

        private void SetMarkMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (HasMark)
                MarksHelper.Instance.RemoveMark(DateInfo.DateTime);
            else
                MarksHelper.Instance.AddMark(DateInfo.DateTime);

            MarksHelper.Instance.Save();
        }
    }

    /// <summary>
    /// 日期转换为颜色
    /// </summary>
    public class DateInfoToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var d = (DateInfo) value;

                if (d.DateTime.Month != d.CurrentDate.Month)
                    return new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));

                if (d.DateTime.DayOfWeek == DayOfWeek.Saturday || d.DateTime.DayOfWeek == DayOfWeek.Sunday)
                    return new SolidColorBrush(Color.FromRgb(0xd4, 0x88, 0x7a));

                return new SolidColorBrush(Colors.White);
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 日期转换为背景颜色
    /// </summary>
    public class DateInfoToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var d = (DateInfo) value;

                if (d.DateTime.Date == DateTime.Now.Date)
                    return new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));

                return new SolidColorBrush(Colors.Transparent);
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 转农历日期
    /// </summary>
    public class DateInfoToLunisolarDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var d = (DateInfo) value;

                var result = d.OB.Ldc;

                if (result == "初一")
                    return $"{d.OB.Lleap}{d.OB.Lmc}{d.OB.Ldns}";

                return result;
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
    /// 转24节气
    /// </summary>
    public class DateInfoToDivisionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var d = (DateInfo) value;

                if (d.OB.Ljq != "")
                    return d.OB.Ljq;

                ChineseAlmanac ca = new ChineseAlmanac();

                return ca.GetSanFuShuJiuString(d.DateTime);
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
    /// 日期转换为颜色
    /// </summary>
    public class DateInfoToHolidayFontBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var d = (DateInfo) value;

                if (d.DateTime.Month != d.CurrentDate.Month)
                    return new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));

                ChineseAlmanac ca = new ChineseAlmanac();
                string holiday = "";

                if (HolidayHelper.Instance.HolidayInfo?.ChineseHolidays != null)
                    holiday = ca.GetChineseHoliday(d.DateTime, HolidayHelper.Instance.HolidayInfo.ChineseHolidays);

                if (holiday != "")
                    return new SolidColorBrush(Colors.OrangeRed);

                return new SolidColorBrush(Colors.DarkOrange);
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 转节假日
    /// </summary>
    public class DateInfoToHolidayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var d = (DateInfo) value;

                ChineseAlmanac ca = new ChineseAlmanac();
                string holiday = "";

                if (HolidayHelper.Instance.HolidayInfo?.ChineseHolidays != null)
                    holiday = ca.GetChineseHoliday(d.DateTime, HolidayHelper.Instance.HolidayInfo.ChineseHolidays);

                if (holiday != "")
                    return holiday;

                if (HolidayHelper.Instance.HolidayInfo?.Holidays != null)
                    holiday = ca.GetHoliday(d.DateTime, HolidayHelper.Instance.HolidayInfo.Holidays);

                return holiday;
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
    /// 日期转换为颜色
    /// </summary>
    public class DateInfoToDivisionFontBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var d = (DateInfo) value;

                if (d.DateTime.Month != d.CurrentDate.Month)
                    return new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));

                return new SolidColorBrush(Color.FromArgb(255, 0, 150, 200));
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 日期转换为颜色
    /// </summary>
    public class DateInfoToDayBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var d = (DateInfo) value;

                if (d.DateTime.Month != d.CurrentDate.Month)
                    return new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));

                var result = d.OB.Ldc;

                if (result == "初一")
                    return new SolidColorBrush(Color.FromRgb(0xFF, 0xAF, 0xB2));

                return new SolidColorBrush(Colors.White);
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 日期转假期
    /// </summary>
    public class DateInfoToRestHolidayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var d = (DateInfo) value;

                var par = parameter?.ToString() ?? "";

                if (d.WorkType != WorkType.Unknown)
                {
                    if (d.DateTime.DayOfWeek == DayOfWeek.Saturday || d.DateTime.DayOfWeek == DayOfWeek.Sunday)
                    {
                        switch (d.WorkType)
                        {
                            case WorkType.WorkDay:
                                return par == "D" ? "上班" : "班";

                            case WorkType.Holiday:
                                return par == "D" ? "休息" : "休";
                            case WorkType.Unknown:
                            case WorkType.RestDay:
                                return par == "D" ? "休息" : "";
                            default:
                                break;
                                ;
                        }
                    }
                    else
                    {
                        switch (d.WorkType)
                        {
                            case WorkType.WorkDay:
                                return par == "D" ? "上班" : "";
                            case WorkType.Holiday:
                                return par == "D" ? "休息" : "休";
                            case WorkType.Unknown:
                            case WorkType.RestDay:
                                return par == "D" ? "休息" : "休";
                            default:
                                break;
                                ;
                        }
                    }
                }
                else
                {
                    foreach (var holiday in HolidayHelper.Instance.HolidayInfo.RestHolidays)
                    {
                        if (holiday.RestDay != null)
                            if (holiday.RestDay.Any(rest => d.DateTime.Date == rest.Date))
                            {
                                return par == "D" ? "休息" : "休";
                            }

                        if (holiday.WorkDay == null) continue;

                        if (holiday.WorkDay.Any(work => d.DateTime.Date == work.Date))
                        {
                            return par == "D" ? "上班" : "班";
                        }
                    }

                    if (d.DateTime.DayOfWeek == DayOfWeek.Saturday || d.DateTime.DayOfWeek == DayOfWeek.Sunday)
                        return par == "D" ? "休息" : "";

                    return par == "D" ? "上班" : "";
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
    /// 日期转假期
    /// </summary>
    public class DateInfoToHolidayBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var d = (DateInfo) value;

                if (d.WorkType != WorkType.Unknown)
                {
                    if (d.DateTime.DayOfWeek == DayOfWeek.Saturday || d.DateTime.DayOfWeek == DayOfWeek.Sunday)
                    {
                        switch (d.WorkType)
                        {
                            case WorkType.WorkDay:
                                return new SolidColorBrush(Colors.Red);
                            case WorkType.Holiday:
                                return new SolidColorBrush(Colors.LawnGreen);
                            case WorkType.Unknown:
                            case WorkType.RestDay:
                            default:
                                break;
                                ;
                        }
                    }
                    else
                    {
                        switch (d.WorkType)
                        {
                            case WorkType.Holiday:
                                return new SolidColorBrush(Colors.LawnGreen);
                            case WorkType.Unknown:
                            case WorkType.RestDay:
                                return new SolidColorBrush(Colors.LawnGreen);
                            case WorkType.WorkDay:
                            default:
                                break;
                                ;
                        }
                    }
                }
                else
                {
                    foreach (var holiday in HolidayHelper.Instance.HolidayInfo.RestHolidays)
                    {
                        if (holiday.RestDay != null)
                            if (holiday.RestDay.Any(rest => d.DateTime.Date == rest.Date))
                            {
                                return new SolidColorBrush(Colors.LawnGreen);
                            }

                        if (holiday.WorkDay != null)
                            if (holiday.WorkDay.Any(work => d.DateTime.Date == work.Date))
                            {
                                return new SolidColorBrush(Colors.Red);
                            }
                    }
                }

                return new SolidColorBrush(Colors.Transparent);
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateInfoToToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = value as DateInfo;

            if (d == null)
                return "";

            return $"{d.DateTime:M月d日 ddd}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateInfoToDaysInfoOfYearConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = value as DateInfo;

            if (d == null)
                return "";
            GregorianCalendar gc = new GregorianCalendar();

            ChineseAlmanac ca = new ChineseAlmanac();

            var jieqi = d.OB.Ljq;
            var sanfu = ca.GetSanFuInfo(d.DateTime);
            var shujiu = ca.GetShuJiuInfo(d.DateTime);
            string holiday = "";
            string chineseHoliday = "";

            if (HolidayHelper.Instance.HolidayInfo?.ChineseHolidays != null)
                holiday = ca.GetChineseHoliday(d.DateTime, HolidayHelper.Instance.HolidayInfo.ChineseHolidays);

            if (HolidayHelper.Instance.HolidayInfo?.Holidays != null)
                chineseHoliday = ca.GetHoliday(d.DateTime, HolidayHelper.Instance.HolidayInfo.Holidays);

            var result =
                $"第{d.DateTime.DayOfYear}天 第{gc.GetWeekOfYear(d.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}周";

            var info = "";

            if (jieqi != "")
                info += jieqi;

            if (sanfu != "")
                info += info == "" ? sanfu : " " + sanfu;

            if (shujiu != "")
                info += info == "" ? shujiu : " " + shujiu;

            if (holiday != "")
                info += info == "" ? holiday : " " + holiday;

            if (chineseHoliday != "")
                info += info == "" ? chineseHoliday : " " + chineseHoliday;

            result += $"{(info == "" ? "" : $" ({info})")}";

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateInfoToDateInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = value as DateInfo;

            if (d == null)
                return "";

            return $"{d.DateTime:yyyy年M月d日} ({d.OB.LMouthInfo})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateInfoToChineseDateInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = value as DateInfo;

            if (d == null)
                return "";

            ChineseAlmanac ca = new ChineseAlmanac();

            return $"{d.OB.Lyear3}{d.OB.LShX2}年 {d.OB.Lmonth2}月 {d.OB.Lday2}日";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateInfoToDaysConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = value as DateInfo;

            if (d == null)
                return "";

            var days = (d.DateTime.Date - DateTime.Now.Date).Days;

            switch (days)
            {
                case -2:
                    return "前天";
                case -1:
                    return "昨天";
                case 0:
                    return "今天";
                case 1:
                    return "明天";
                case 2:
                    return "后天";
            }

            return $"{Math.Abs(days)}天{(days > 0 ? "后" : "前")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateInfoToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = value as DateInfo;

            if (d == null)
                return Visibility.Collapsed;

            return d.Weather == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HasMarkToSetingTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var hasMark = (bool) value;

                return hasMark ? "取消标记" : "设置标记";
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

    public class NotesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var notes = value as List<Note>;

            if (notes == null)
                return "";

            string result = string.Empty;

            foreach (var note in notes)
            {
                result += $"{note.TypeInfo}{(string.IsNullOrEmpty(note.TypeInfo) ? "" : "  ")}{note.Content}\n";
            }

            return result.Trim('\n');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
