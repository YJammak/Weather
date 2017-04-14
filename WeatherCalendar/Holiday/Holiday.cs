using SharpSxwnl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Weather;

namespace WeatherCalendar
{
    public class Holiday
    {
        public string Name { get; set; }

        public List<DateTime> RestDay { get; set; }

        public List<DateTime> WorkDay { get; set; }

        public DateTime FirstDay => RestDay.FirstOrDefault(d => d.Date >= DateTime.Today);

        public override string ToString()
        {
            return Name;
        }
    }

    public class HolidayInfo
    {
        public event Action HolidayChanged;

        private string _cityKey;

        [JsonProperty(propertyName: "City")]
        public string CityKey
        {
            get => _cityKey;
            set
            {
                _cityKey = value;
                City = WeatherHelper.Instance.CityKeyInfos.FirstOrDefault(info => info.CityKey == value);
            }
        }

        [JsonIgnore]
        public CityKeyInfo City { get; private set; }

        public List<Holiday> RestHolidays { get; set; }

        public List<string> ChineseHolidays { get; set; }

        public List<string> Holidays { get; set; }

        public HolidayInfo()
        {
            CityKey = "";
            RestHolidays = new List<Holiday>();
            ChineseHolidays = new List<string>();
            Holidays = new List<string>();
        }

        public Holiday GetHoliday(string name)
        {
            foreach (var restHoliday in RestHolidays)
            {
                if (restHoliday.Name == name)
                    return restHoliday;
            }

            return null;
        }
        
        private readonly ChineseAlmanac chineseAlmanac = new ChineseAlmanac();

        public Holiday GetRestHoliday(DateTime date)
        {
            var comparer = new DateTimeComparer();
            var r = from h in RestHolidays
                where (h.RestDay != null && h.RestDay.Contains(date, comparer)) ||
                      (h.WorkDay != null && h.WorkDay.Contains(date, comparer))
                select h;

            return r.Any() ? r.First() : null;
        }

        public void AddRestHoliday(RestHolidayInfo info)
        {
            RemoveRestHoliday(info.Date , false);

            var h = GetHoliday(info.Name);

            if (h == null)
            {
                var holiday = new Holiday();
                holiday.Name = info.Name;

                if (info.IsRest)
                {
                    holiday.RestDay = new List<DateTime>() {info.Date};
                }
                else
                {
                    holiday.WorkDay = new List<DateTime>() {info.Date};
                }

                RestHolidays.Add(holiday);
            }
            else
            {
                if (info.IsRest)
                {
                    if (h.RestDay == null)
                        h.RestDay = new List<DateTime>() {info.Date};
                    else
                        h.RestDay.Add(info.Date);
                }
                else
                {
                    if (h.WorkDay == null)
                        h.WorkDay = new List<DateTime>() {info.Date};
                    else
                        h.WorkDay.Add(info.Date);
                }
            }

            HolidayChanged?.Invoke();
        }

        public bool RemoveRestHoliday(DateTime date, bool sendEnvent = true)
        {
            var h = GetRestHoliday(date);

            if (h == null)
                return false;

            int count = 0;
            try
            {
                count = h.RestDay?.RemoveAll(time => time.Date == date.Date) ?? 0;
                count = count == 0 ? h.WorkDay?.RemoveAll(time => time.Date == date.Date) ?? 0 : count;

                if (!(h.RestDay?.Count > 0 || h.WorkDay?.Count > 0))
                    RestHolidays.Remove(h);

                return count > 0;
            }
            finally 
            {
                if (count > 0 && sendEnvent)
                    HolidayChanged?.Invoke();
            }
        }

        public bool RemoveRestHoliday(RestHolidayInfo info, bool sendEnvent = true)
        {
            var h = GetHoliday(info.Name);

            if (h == null)
                return false;

            int count = 0;
            try
            {
                count = h.RestDay?.RemoveAll(time => time.Date == info.Date.Date) ?? 0;
                count = count == 0 ? h.WorkDay?.RemoveAll(time => time.Date == info.Date.Date) ?? 0 : count;

                if (!(h.RestDay?.Count > 0 || h.WorkDay?.Count > 0))
                    RestHolidays.Remove(h);

                return count > 0;
            }
            finally
            {
                if (count > 0 && sendEnvent)
                    HolidayChanged?.Invoke();
            }
        }

        public bool AddHoliday(string holiday)
        {
            if (string.IsNullOrEmpty(holiday))
                return false;

            try
            {
                var d = new DateTime(2016, int.Parse(holiday.Substring(0, 2)), int.Parse(holiday.Substring(2, 2)));

                return AddHoliday(holiday.Substring(4), d);
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool AddHoliday(string name, DateTime date)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            try
            {
                RemoveHoliday(date, false);

                var holiday = date.ToString("MMdd") + name;
                if (Holidays == null)
                    Holidays = new List<string>() {holiday};
                else
                    Holidays.Add(holiday);

                HolidayChanged?.Invoke();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveHoliday(DateTime date, bool sendEvent = true)
        {
            List<string> removes = new List<string>();
            foreach (var holiday in Holidays)
            {
                var d = new DateTime(2016, int.Parse(holiday.Substring(0, 2)), int.Parse(holiday.Substring(2, 2)));
                if (d.Month == date.Month && d.Day == date.Day)
                    removes.Add(holiday);
            }

            foreach (var remove in removes)
            {
                Holidays.RemoveAll(d => d == remove);
            }

            if (removes.Count > 0 && sendEvent)
                HolidayChanged?.Invoke();

            return removes.Count > 0;
        }

        public string GetHoliday(DateTime date)
        {
            foreach (var holiday in Holidays)
            {
                var d = new DateTime(2016, int.Parse(holiday.Substring(0, 2)), int.Parse(holiday.Substring(2, 2)));
                if (d.Month == date.Month && d.Day == date.Day)
                    return holiday.Substring(4);
            }

            return "";
        }

        public bool AddChineseHoliday(string holiday)
        {
            if (string.IsNullOrEmpty(holiday))
                return false;

            try
            {
                var d = new DateTime(2016, int.Parse(holiday.Substring(0, 2)), int.Parse(holiday.Substring(2, 2)));

                return AddChineseHoliday(holiday.Substring(4), d);
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool AddChineseHoliday(string name, DateTime date)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            System.Globalization.ChineseLunisolarCalendar netCalendar = new System.Globalization.ChineseLunisolarCalendar();
            
            var mouth = $"{netCalendar.GetMonth(date):D2}{netCalendar.GetDayOfMonth(date):D2}";

            try
            {
                RemoveChineseHoliday(date, false);

                var holiday = mouth + name;
                if (ChineseHolidays == null)
                    ChineseHolidays = new List<string>() { holiday };
                else
                    ChineseHolidays.Add(holiday);

                HolidayChanged?.Invoke();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveChineseHoliday(DateTime date, bool sendEvent = true)
        {
            System.Globalization.ChineseLunisolarCalendar netCalendar = new System.Globalization.ChineseLunisolarCalendar();
            var mouth = $"{netCalendar.GetMonth(date):D2}{netCalendar.GetDayOfMonth(date):D2}";

            List<string> removes = new List<string>();
            foreach (var holiday in ChineseHolidays)
            {
                if (mouth == holiday.Substring(0, 4))
                    removes.Add(holiday);
            }

            foreach (var remove in removes)
            {
                ChineseHolidays.RemoveAll(d => d == remove);
            }

            if (removes.Count > 0 && sendEvent)
                HolidayChanged?.Invoke();

            return removes.Count > 0;
        }

        public string GetChineseHoliday(DateTime date)
        {
            return chineseAlmanac.GetChineseHoliday(date, ChineseHolidays);
        }
    }

    public class DateTimeComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y)
        {
            return x.Date == y.Date;
        }

        public int GetHashCode(DateTime obj)
        {
            return obj.GetHashCode();
        }
    }
}
