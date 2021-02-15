using System.Windows.Media;

namespace WeatherCalendar.Themes
{
    public interface ITheme
    {
        public Brush DayNameNormalForeground { get; }

        public Brush DayNameWeekendForeground { get; }

        public Brush DayNameAnotherMonthForeground { get; }

        public Brush DayViewBackground { get; }

        public Brush DayViewCurrentDayBackground { get; }

        public Brush DayViewMouseOverDayBackground { get; }

        public Brush LunarDayNormalForeground { get; }

        public Brush LunarDayWeekendForeground { get; }

        public Brush LunarDayAnotherMonthForeground { get; }

        public Brush SolarTermNormalForeground { get; }

        public Brush SolarTermWeekendForeground { get; }

        public Brush SolarTermAnotherMonthForeground { get; }

        public Brush ChineseFestivalNormalForeground { get; }

        public Brush ChineseFestivalWeekendForeground { get; }

        public Brush FestivalNormalForeground { get; }

        public Brush FestivalWeekendForeground { get; }

        public Brush FestivalAnotherMonthForeground { get; }

        public Brush HolidayRestDayForeground { get; }

        public Brush HolidayWorkDayForeground { get; }

        public Brush BackgroundMonthForeground { get; }

        public double BackgroundMonthOpacity { get; }
    }
}
