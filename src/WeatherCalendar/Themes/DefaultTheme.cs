using System.Windows.Media;

namespace WeatherCalendar.Themes;

public sealed class DefaultTheme : ITheme
{
    public Brush MainWindowBackground =>
        new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));

    public Brush DayNameNormalForeground => Brushes.White;

    public Brush DayNameWeekendForeground
        => new SolidColorBrush(Color.FromRgb(0xd4, 0x88, 0x7a));

    public Brush DayNameAnotherMonthForeground
        => new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));

    public Brush DayViewBackground => Brushes.Transparent;

    public Brush DayViewCurrentDayBackground
        => new SolidColorBrush(Color.FromArgb(0x22, 0, 0, 0));

    public Brush DayViewMouseOverDayBackground
        => new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF));

    public Brush LunarDayNormalForeground => Brushes.White;

    public Brush LunarDayWeekendForeground => Brushes.White;

    public Brush LunarDayAnotherMonthForeground
        => new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));

    public Brush SolarTermNormalForeground
        => new SolidColorBrush(Color.FromArgb(255, 0, 150, 200));

    public Brush SolarTermWeekendForeground
        => new SolidColorBrush(Color.FromArgb(255, 0, 150, 200));

    public Brush SolarTermAnotherMonthForeground
        => new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));

    public Brush ChineseFestivalNormalForeground => Brushes.OrangeRed;

    public Brush ChineseFestivalWeekendForeground => Brushes.OrangeRed;

    public Brush FestivalNormalForeground => Brushes.DarkOrange;

    public Brush FestivalWeekendForeground => Brushes.DarkOrange;

    public Brush FestivalAnotherMonthForeground
        => new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));

    public Brush HolidayRestDayForeground => Brushes.LawnGreen;

    public Brush HolidayWorkDayForeground => Brushes.Red;

    public Brush BackgroundMonthForeground => Brushes.White;

    public double BackgroundMonthOpacity => 0.2;

    public Brush WorkTimerNormalForeground => Brushes.White;

    public Brush WorkTimerTimeForeground => Brushes.OrangeRed;

    public Brush WeatherIconForeground
        => new SolidColorBrush(Color.FromRgb(14, 176, 201));
}
