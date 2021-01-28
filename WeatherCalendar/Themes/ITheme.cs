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
        
        public Brush LunarDayNormalForeground { get; }

        public Brush LunarDayWeekendForeground { get; }

        public Brush LunarDayAnotherMonthForeground { get; }

        public Brush SolarTermNormalForeground { get; }

        public Brush SolarTermWeekendForeground { get; }

        public Brush SolarTermAnotherMonthForeground { get; }

        public Brush ChineseHolidayNormalForeground { get; }

        public Brush ChineseHolidayWeekendForeground { get; }

        public Brush HolidayNormalForeground { get; }

        public Brush HolidayWeekendForeground { get; }

        public Brush HolidayAnotherMonthForeground { get; }
    }
}
