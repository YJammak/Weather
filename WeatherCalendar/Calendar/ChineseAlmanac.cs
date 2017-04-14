using System;
using System.Collections;
using System.Collections.Generic;

namespace WeatherCalendar
{
    public class ChineseAlmanac : System.Globalization.ChineseLunisolarCalendar
    {
        private readonly System.Globalization.ChineseLunisolarCalendar netCalendar = new System.Globalization.ChineseLunisolarCalendar();

        #region 加强

        private static readonly string[] JQ = { "小寒", "大寒", "立春", "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏", "小满", "芒种", "夏至", "小暑", "大暑", "立秋", "处暑", "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪", "冬至" };
        private static readonly int[] JQData = { 0, 21208, 43467, 63836, 85337, 107014, 128867, 150921, 173149, 195551, 218072, 240693, 263343, 285989, 308563, 331033, 353350, 375494, 397447, 419210, 440795, 462224, 483532, 504758 };

        public string GetShuJiuInfo(DateTime dt)
        {
            DateTime dtBase = new DateTime(1900, 1, 6, 2, 3, 57);
            DateTime dtNew;
            double num;
            int y;

            y = dt.Year;
            num = 525948.76 * (y - 1900) + JQData[23];
            var dongzhi = dtBase.AddMinutes(num);
            num = 525948.76 * (y - 1 - 1900) + JQData[23];
            var donzhiLastYear = dtBase.AddMinutes(num);

            var suzi = "一二三四五六七八九";

            var days = (dt.Date - dongzhi.Date).Days;
            switch (days)
            {
                case 0:
                    return "一九";
                case 9:
                    return "二九";
                case 18:
                    return "三九";
                case 27:
                    return "四九";
                case 36:
                    return "五九";
                case 45:
                    return "六九";
                case 54:
                    return "七九";
                case 63:
                    return "八九";
                case 72:
                    return "九九";
            }

            if (days < 81 && days > 0)
                return $"{suzi.Substring(days / 9, 1)}九 第{days % 9 + 1}天";

            days = (dt.Date - donzhiLastYear.Date).Days;
            switch (days)
            {
                case 0:
                    return "一九";
                case 9:
                    return "二九";
                case 18:
                    return "三九";
                case 27:
                    return "四九";
                case 36:
                    return "五九";
                case 45:
                    return "六九";
                case 54:
                    return "七九";
                case 63:
                    return "八九";
                case 72:
                    return "九九";
            }

            if (days < 81 && days > 0)
                return $"{suzi.Substring(days / 9, 1)}九 第{days % 9 + 1}天";

            return "";
        }

        public string GetSanFuInfo(DateTime dt)
        {
            DateTime dtBase = new DateTime(1900, 1, 6, 2, 3, 57);
            DateTime dtNew;
            double num;
            int y;

            y = dt.Year;

            num = 525948.76 * (y - 1900) + JQData[11];
            var xiazhi = dtBase.AddMinutes(num);

            num = 525948.76 * (y - 1900) + JQData[14];
            var liqiu = dtBase.AddMinutes(num);

            if (dt.Date > xiazhi.Date && dt.Date < liqiu.Date.AddDays(20))
            {
                var t = ((xiazhi.Date - dtBase.Date).Days + 6) % 10;
                var addDay = 7 - t;
                var chufu = xiazhi.Date.AddDays(addDay > 0 ? addDay + 20 : addDay + 30);

                if (dt.Date == chufu.Date)
                    return "初伏";

                var zhongfu = chufu.Date.AddDays(10);

                if (dt.Date == zhongfu.Date)
                    return "中伏";

                t = ((liqiu.Date - dtBase.Date).Days + 6) % 10;
                addDay = 7 - t;
                var mofu = liqiu.Date.AddDays(addDay > 0 ? addDay : addDay + 10);

                if (dt.Date == mofu.Date)
                    return "末伏";

                if (dt.Date > chufu.Date)
                {
                    if (dt.Date < zhongfu.Date)
                    {
                        return $"初伏 第{(dt.Date - chufu.Date).Days + 1}天";
                    }
                    if (dt.Date < mofu.Date)
                    {
                        return $"中伏 第{(dt.Date - zhongfu.Date).Days + 1}天";
                    }
                    if((dt.Date - mofu.Date).Days < 10)
                    {
                        return $"末伏 第{(dt.Date - mofu.Date).Days + 1}天";
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// 获取节气
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string GetSanFuShuJiuString(DateTime dt)
        {
            DateTime dtBase = new DateTime(1900, 1, 6, 2, 3, 57);
            DateTime dtNew;
            double num;
            int y;

            y = dt.Year;

            #region 三伏计算

            num = 525948.76 * (y - 1900) + JQData[11];
            var xiazhi = dtBase.AddMinutes(num);

            num = 525948.76 * (y - 1900) + JQData[14];
            var liqiu = dtBase.AddMinutes(num);

            if (dt.Date > xiazhi.Date && dt.Date < liqiu.Date.AddDays(10))
            {
                var t = ((xiazhi.Date - dtBase.Date).Days + 6) % 10;
                var addDay = 7 - t;
                var chufu = xiazhi.Date.AddDays(addDay > 0 ? addDay  + 20 : addDay + 30);

                if (dt.Date == chufu.Date)
                    return "初伏";

                if (dt.Date == chufu.Date.AddDays(10))
                    return "中伏";

                t = ((liqiu.Date - dtBase.Date).Days + 6) % 10;
                addDay = 7 - t;
                var mofu = liqiu.Date.AddDays(addDay > 0 ? addDay: addDay + 10);

                if (dt.Date == mofu.Date)
                    return "末伏";
            }

            #endregion

            #region 数九计算

            num = 525948.76 * (y - 1900) + JQData[23];
            var dongzhi = dtBase.AddMinutes(num);
            num = 525948.76 * (y - 1 - 1900) + JQData[23];
            var donzhiLastYear = dtBase.AddMinutes(num);

            var days = (dt.Date - dongzhi.Date).Days;
            switch (days)
            {
                case 9:
                    return "二九";
                case 18:
                    return "三九";
            }

            days = (dt.Date - donzhiLastYear.Date).Days;
            switch (days)
            {
                case 9:
                    return "二九";
                case 18:
                    return "三九";
            }

            #endregion

            return "";
        }

        #endregion

        #region 节日纪念日

        /// <summary>
        /// 获取节日字符串。
        /// </summary>
        public string GetHoliday(DateTime solarDateTime)
        {
            return GetHoliday(solarDateTime, holiday);
        }

        /// <summary>
        /// 获取节日字符串。
        /// </summary>
        public string GetHoliday(DateTime solarDateTime, IEnumerable<string> holidays)
        {
            var re = "";
            foreach (var s in holidays)
            {
                if (solarDateTime.Month == Convert.ToInt32(s.Substring(0, 2)))
                {
                    if (solarDateTime.Day == Convert.ToInt32(s.Substring(2, 2)))
                    {
                        return s.Substring(4);
                    }
                }
            }

            if (solarDateTime.Month == 5 && solarDateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                int count = 0;
                for (int i = 1; solarDateTime.AddDays(-i).Month == solarDateTime.Month; i++)
                {
                    if (solarDateTime.AddDays(-i).DayOfWeek == DayOfWeek.Sunday)
                        count++;
                }

                if (count == 1)
                    return "母亲节";
            }

            if (solarDateTime.Month == 6 && solarDateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                int count = 0;
                for (int i = 1; solarDateTime.AddDays(-i).Month == solarDateTime.Month; i++)
                {
                    if (solarDateTime.AddDays(-i).DayOfWeek == DayOfWeek.Sunday)
                        count++;
                }

                if (count == 2)
                    return "父亲节";
            }

            if (solarDateTime.Month == 11 && solarDateTime.DayOfWeek == DayOfWeek.Thursday)
            {
                int count = 0;
                for (int i = 1; solarDateTime.AddDays(-i).Month == solarDateTime.Month; i++)
                {
                    if (solarDateTime.AddDays(-i).DayOfWeek == DayOfWeek.Thursday)
                        count++;
                }

                if (count == 3)
                    return "感恩节";
            }

            return "";
        }

        /// <summary>
        /// 获取名人纪念日字符串。
        /// </summary>
        public string GetCommemoration(DateTime solarDateTime)
        {
            var re = "";
            foreach (var s in celebrity)
            {
                if (solarDateTime.Month == Convert.ToInt32(s.Substring(0, 2)))
                {
                    if (solarDateTime.Day == Convert.ToInt32(s.Substring(2, 2)))
                    {
                        re = s.Substring(4);
                        break;
                    }
                }
            }
            return re;
        }

        /// <summary>
        /// 获取中国农历节日字符串。
        /// </summary>
        public string GetChineseHoliday(DateTime solarDateTime)
        {
            var re = "";
            int year = netCalendar.GetYear(solarDateTime);
            int mon = netCalendar.GetMonth(solarDateTime);
            int leapMonth = netCalendar.GetLeapMonth(year);
            int iDay = netCalendar.GetDayOfMonth(solarDateTime);
            if (netCalendar.GetDayOfYear(solarDateTime) == netCalendar.GetDaysInYear(year))
            {
                return "除夕";
            }

            if (leapMonth != mon)
            {
                if (leapMonth != 0 && mon >= leapMonth)
                {
                    mon--;
                }

                foreach (var s in chineseHoliday)
                {
                    if (mon == Convert.ToInt32(s.Substring(0, 2)))
                    {
                        if (netCalendar.GetDayOfMonth(solarDateTime) == Convert.ToInt32(s.Substring(2, 2)))
                        {
                            re = s.Substring(4);
                            break;
                        }
                    }
                }
            }

            return re;
        }

        /// <summary>
        /// 获取中国农历节日字符串。
        /// </summary>
        public string GetChineseHoliday(DateTime solarDateTime, IEnumerable<string> holidays)
        {
            var re = "";
            int year = netCalendar.GetYear(solarDateTime);
            int mon = netCalendar.GetMonth(solarDateTime);
            int leapMonth = netCalendar.GetLeapMonth(year);
            int iDay = netCalendar.GetDayOfMonth(solarDateTime);
            if (netCalendar.GetDayOfYear(solarDateTime) == netCalendar.GetDaysInYear(year))
            {
                return "除夕";
            }
            else if (leapMonth != mon)
            {
                if (leapMonth != 0 && mon >= leapMonth)
                {
                    mon--;
                }

                foreach (var s in holidays)
                {
                    if (mon == Convert.ToInt32(s.Substring(0, 2)))
                    {
                        if (netCalendar.GetDayOfMonth(solarDateTime) == Convert.ToInt32(s.Substring(2, 2)))
                        {
                            re = s.Substring(4);
                            break;
                        }
                    }
                }
            }

            return re;
        }

        #region 节日变量
        private string[] holiday ={
            "0101元旦",
            "0202湿地日",
            "0207援南非日",
            "0210气象节",
            "0214情人节",
            "0301海豹日",
            "0303爱耳日",
            "0305学雷锋日",
            "0308妇女节",
            "0312植树节",
            "0314警察日",
            "0315消权日",
            "0317航海日",
            "0321随眠日",
            "0322世界水日",
            "0323气象日",
            "0324防结核日",
            "0401愚人节",
            "0407卫生日",
            "0422地球日",
            "0501劳动节",
            "0504青年节",
            "0531无烟日",
            "0601儿童节",
            "0605环境日",
            "0606爱眼日",
            "0623奥林匹克",
            "0626反毒品日",
            "0701香港回归",
            "0711人口日",
            "0801建军节",
            "0815日本投降",
            "0910教师节",
            "0920爱牙日",
            "1001国庆节",
            "1013保健日",
            "1014世界标准",
            "1016粮食日",
            "1031万圣节前",
            "1111光棍节",
            "1224平安夜",
            "1220澳门回归",
            "1225圣诞节",
        };
        private string[] chineseHoliday ={
            "0101春节",
            "0103天庆节",
            "0105财神日",
            "01139散花灯",
            "0115元宵节",
            "0116馄饨节",
            "0120补天日",
            "0125填仓节",
            "0202龙头节",
            "0208插花节",
            "0210彩蛋节",
            "0212花朝节",
            "0228寒潮节",
            "0303踏青节",
            "0310撒种节",
            "0315龙华会",
            "0318中岳节",
            "0401清和节",
            "0402公输般日",
            "0408跳月节",
            "0411孔子祭",
            "0414菖蒲日",
            "0419浣花日",
            "0501女儿节",
            "0504采花节",
            "0505端午节",
            "0522曹娥日",
            "0529祖娘节",
            "0606天贶节",
            "0615捕鱼祭",
            "0616爬坡节",
            "0619太阳日",
            "0624观莲节",
            "0707七夕",
            "0712地狱开门",
            "0715中元节",
            "0722财神节",
            "0801天医节",
            "0815中秋节",
            "0818观潮节",
            "0824稻节",
            "0909重阳节",
            "0913钉鞋日",
            "0930采参节",
            "1001祭祖节",
            "1015下元节",
            "1016盘古节",
            "1208腊八节",
            "1212百福日",
            "1223小年(北方)",
            "1224小年(南方)",
            "1225上帝下界"
        };
        private string[] celebrity ={
            "0104雅各布·格林诞辰",
            "0108周恩来逝世纪念日",
            "0106圣女贞德诞辰",
            "0112杰克·伦敦诞辰",
            "0115莫里哀诞辰",
            "0117富兰克林诞辰",
            "0119瓦特诞辰",
            "0122培根诞辰",
            "0123郎之万诞辰",
            "0127莫扎特诞辰",
            "0129罗曼·罗兰诞辰",
            "0130甘地诞辰",
            "0131舒柏特诞辰",
            "0203门德尔松诞辰",
            "0207门捷列夫诞辰",
            "0211爱迪生诞辰，狄更斯诞辰",
            "0212林肯，达尔文诞辰",
            "0217布鲁诺诞辰",
            "0218伏打诞辰",
            "0219哥白尼诞辰",
            "0222赫兹，叔本华，华盛顿诞辰",
            "0226雨果诞辰",
            "0302斯美塔那诞辰",
            "0304白求恩诞辰",
            "0305周恩来诞辰",
            "0306布朗宁，米开朗琪罗诞辰",
            "0307竺可桢诞辰",
            "0314爱因斯坦诞辰",
            "0321巴赫，穆索尔斯基诞辰",
            "0322贺龙诞辰",
            "0328高尔基诞辰",
            "0401海顿，果戈理诞辰",
            "0415达·芬奇诞辰",
            "0416卓别林诞辰",
            "0420祖冲之诞辰",
            "0422列宁，康德，奥本海默诞辰",
            "0423普朗克，莎士比亚诞辰",
            "0430高斯诞辰",
            "0505马克思诞辰",
            "0507柴可夫斯基，泰戈尔诞辰",
            "0511冼星海诞辰",
            "0511李比希诞辰",
            "0520巴尔扎克诞辰",
            "0522瓦格纳诞辰",
            "0531惠特曼诞辰",
            "0601杜威诞辰",
            "0602哈代诞辰",
            "0608舒曼诞辰",
            "0715伦勃朗诞辰",
            "0805阿贝尔诞辰",
            "0808狄拉克诞辰",
            "0826陈毅诞辰",
            "0828歌德诞辰",
            "0909***逝世纪念日",
            "0925鲁迅诞辰",
            "0926巴甫洛夫诞辰",
            "0928孔子诞辰",
            "0929奥斯特洛夫斯基诞辰",
            "1011伯辽兹诞辰",
            "1021诺贝尔诞辰",
            "1022李斯特诞辰",
            "1026伽罗瓦诞辰",
            "1029李大钊诞辰",
            "1007居里夫人诞辰",
            "1108哈雷诞辰",
            "1112孙中山诞辰",
            "1124刘少奇诞辰",
            "1128恩格斯诞辰",
            "1201朱德诞辰",
            "1205海森堡诞辰",
            "1211玻恩诞辰",
            "1213海涅诞辰",
            "1216贝多芬诞辰",
            "1221斯大林诞辰",
            "1225牛顿诞辰",
            "1226***诞辰",
            "1229阿·托尔斯泰诞辰"
        };
        #endregion

        #endregion
    }
}
