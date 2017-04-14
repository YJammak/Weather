using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpSxwnl
{
    /// <summary>
    /// 日历计算类
    /// </summary>
    public class Lunar
    {
        #region 公共属性(注: 初始转换时为公共字段, 已改写)

        /// <summary>
        /// 月对象, 保存若干个日对象(OB)
        /// </summary>
        //--------------------------------------------------------------------------------------
        // 更改为公共属性, 但需要在构造函数中创建对象的实例. 保留初次转换时的语句如下:
        // public LunarInfoListT<OB> lun = new LunarInfoListT<OB>();    // 存储 OB 类的实例
        //--------------------------------------------------------------------------------------
        public LunarInfoListT<OB> lun { get; set; }    // 存储 OB 类的实例(日对象)

        /// <summary>
        /// 本月第一天的星期
        /// </summary>
        public double w0 { get; set; }    // = 0;    // 本月第一天的星期

        /// <summary>
        /// 公历年份
        /// </summary>
        public double y { get; set; }    // = 0;     // 公历年份

        /// <summary>
        /// 公历月分
        /// </summary>
        public double m { get; set; }    // = 0;     // 公历月分

        /// <summary>
        /// 月首的J2000.0起算的儒略日数
        /// </summary>
        public double d0 { get; set; }    // = 0;

        /// <summary>
        /// 本月的天数
        /// </summary>
        public double dn { get; set; }    // = 0;

        /// <summary>
        /// 该年的干支纪年
        /// </summary>
        public string Ly { get; set; }    // = "";        // 干支纪年

        /// <summary>
        /// 该年的生肖
        /// </summary>
        public string ShX { get; set; }    // = "";       // 该年对应的生肖

        /// <summary>
        /// 年号
        /// </summary>
        public string nianhao { get; set; }    // = "";   // 年号

        /// <summary>
        /// 保存生成的网页内容: 朝代名, 年号, 纪年干支, 属相
        /// </summary>
        public string pg0 { get; set; }    // = "";       // C#: 属性 pg*, 用于生成网页内容

        /// <summary>
        /// 保存生成的网页内容: 月历
        /// </summary>
        public string pg1 { get; set; }    // = "";

        /// <summary>
        /// 保存生成的网页内容: 月相, 交节信息
        /// </summary>
        public string pg2 { get; set; }    // = "";

        #endregion


        #region 构造函数

        /// <summary>
        /// 构造函数, 为月对象添加 31 个日对象
        /// </summary>
        public Lunar()
        {
            this.lun = new LunarInfoListT<OB>();    // C#: 转换为自动实现的公共属性时添加本句, 创建实例

            for (int i = 0; i < 31; i++) 
                this.lun.Add(new OB());
            this.lun.dn = 0;
        }

        public Lunar(DateTime date):this()
        {
            yueLiCalc(date.Year, date.Month);
        }

        #endregion



        #region 公共方法

        public OB GetOBOfDay(DateTime date)
        {
            foreach (var ob in lun)
            {
                if (ob.y == date.Year && ob.m == date.Month && ob.d == date.Day)
                    return ob;
            }

            return null;
        }

        /// <summary>
        /// 截串(网页设计对过长的文字做截处理)
        /// </summary>
        /// <param name="s">被截取的字符串</param>
        /// <param name="n">要截取的长度</param>
        /// <param name="end">对超长字符串, 在截取的子串尾部添加的内容</param>
        /// <returns></returns>
        public string substr2(string s, int n, string end)
        {
            Regex regToReplace = new Regex(@"(^\s*)|(\s*$)", RegexOptions.ECMAScript);   // 去除首尾空白字符的正则表达式模板
            s = regToReplace.Replace(s, "");
            if (s.Length > n + 1) return s.Substring(0, n) + end;
            return s;
        }




        /// <summary>
        /// 计算公历某一个月的"公农回"三合历, 并把相关信息保存到月对象 lun, 以及日对象 lun[?] 中
        /// </summary>
        /// <param name="By">要计算月历的年</param>
        /// <param name="Bm">要计算月历的月</param>
        public void yueLiCalc(int By, int Bm)
        {
            int i, j;
            double c, Bd0, Bdn;

            // 日历物件初始化
            JD.h = 12; JD.m = 0; JD.s = 0.1;
            JD.Y = By; JD.M = Bm; JD.D = 1; Bd0 = LunarHelper.int2(JD.toJD()) - LunarHelper.J2000;  // 公历某年的月首,中午
            JD.M++; 
            if (JD.M > 12) { JD.Y++; JD.M = 1; }     // C#: 如果月份大于 12, 则年数 + 1, 月数取 1
            Bdn = LunarHelper.int2(JD.toJD()) - LunarHelper.J2000 - Bd0; // 本月天数(公历)

            this.w0 = (Bd0 + LunarHelper.J2000 + 1) % 7; //本月第一天的星期
            this.y = By;   // 公历年份
            this.m = Bm;   // 公历月分
            this.d0 = Bd0;
            this.dn = Bdn;

            // 所属公历年对应的农历干支纪年
            c = By - 1984 + 9000;
            this.Ly = obb.Gan[(int)(c % 10)] + obb.Zhi[(int)(c % 12)];  //干支纪年
            this.ShX = obb.ShX[(int)(c % 12)]; // 该年对应的生肖
            this.nianhao = obb.getNH(By);

            double D, w;
            OB ob, ob2;

            // 循环提取各日信息
            for (i = 0, j = 0; i < Bdn; i++)
            {
                ob = (this.lun[i]);
                ob.d0 = Bd0 + i;   // 儒略日,北京时12:00
                ob.di = i;         // 公历月内日序数
                ob.y = By;         // 公历年
                ob.m = Bm;         // 公历月
                ob.dn = Bdn;       // 公历月天数
                ob.week0 = this.w0;     // 月首的星期
                ob.week = (this.w0 + i) % 7;     // 当前日的星期
                ob.weeki = LunarHelper.int2((this.w0 + i) / 7);    // 本日所在的周序号
                ob.weekN = LunarHelper.int2((this.w0 + Bdn - 1) / 7) + 1;    // 本月的总周数
                JD.setFromJD(ob.d0 + LunarHelper.J2000); ob.d = (int)JD.D;   //公历日名称

                // 农历月历
                if (SSQ.ZQ.Count == 0 || ob.d0 < SSQ.ZQ[0] || ob.d0 >= SSQ.ZQ[24])   // 如果d0已在计算农历范围内则不再计算
                    SSQ.calcY(ob.d0);
                int mk = (int)LunarHelper.int2((ob.d0 - SSQ.HS[0]) / 30); if (mk < 13 && SSQ.HS[mk + 1] <= ob.d0) mk++; // 农历所在月的序数

                ob.Ldi = (int)(ob.d0 - SSQ.HS[mk]);   // 距农历月首的编移量,0对应初一
                ob.Ldc = obb.rmc[(int)(ob.Ldi)];      // 农历日名称
                ob.cur_dz = ob.d0 - SSQ.ZQ[0];     // 距冬至的天数
                ob.cur_xz = ob.d0 - SSQ.ZQ[12];    // 距夏至的天数
                ob.cur_lq = ob.d0 - SSQ.ZQ[15];    // 距立秋的天数
                ob.cur_mz = ob.d0 - SSQ.ZQ[11];    // 距芒种的天数
                ob.cur_xs = ob.d0 - SSQ.ZQ[13];    // 距小暑的天数
                if (ob.d0 == SSQ.HS[mk] || ob.d0 == Bd0)
                {   
                    // 月的信息
                    var yue = SSQ.ym[mk];
                    if (yue == "十一")
                        yue = "冬";
                    if (yue == "十二")
                        yue = "腊";

                    ob.Lmc = yue + "月";  // 月名称
                    ob.Ldn = SSQ.dx[mk];  // 月大小
                    ob.Lleap = (SSQ.leap != 0 && SSQ.leap == mk) ? "闰" : "";   // 闰状况
                    ob.Lmc2 = mk < 13 ? SSQ.ym[mk + 1] : "未知";   // 下个月名称,判断除夕时要用到
                }
                else
                {
                    ob2 = (this.lun[i - 1]);
                    ob.Lmc = ob2.Lmc; ob.Ldn = ob2.Ldn;
                    ob.Lleap = ob2.Lleap; ob.Lmc2 = ob2.Lmc2;
                }
                int qk = (int)LunarHelper.int2((ob.d0 - SSQ.ZQ[0] - 7) / 15.2184); if (qk < 23 && ob.d0 >= SSQ.ZQ[qk + 1]) qk++; //节气的取值范围是0-23
                if (ob.d0 == SSQ.ZQ[qk]) ob.Ljq = obb.jqmc[qk];
                else ob.Ljq = "";

                ob.yxmc = ob.yxjd = ob.yxsj = "";    // 月相名称,月相时刻(儒略日),月相时间串
                ob.jqmc = ob.jqjd = ob.jqsj = "";    // 定气名称,节气时刻(儒略日),节气时间串

                // 干支纪年处理
                // 以立春为界定年首
                D = SSQ.ZQ[3] + (ob.d0 < SSQ.ZQ[3] ? -365 : 0) + 365.25 * 16 - 35; //以立春为界定纪年
                ob.Lyear = Math.Floor(D / 365.2422 + 0.5); //农历纪年(10进制,1984年起算)

                // 以下几行以正月初一定年首
                D = SSQ.HS[2];     // 一般第3个月为春节
                for (j = 0; j < 14; j++)
                { 
                    // 找春节
                    if (SSQ.ym[j] != "正") continue;
                    D = SSQ.HS[j];
                    if (ob.d0 < D) { D -= 365; break; }   // 无需再找下一个正月
                }
                D = D + 5810;    // 计算该年春节与1984年平均春节(立春附近)相差天数估计
                ob.Lyear0 = Math.Floor(D / 365.2422 + 0.5);   // 农历纪年(10进制,1984年起算)

                D = ob.Lyear + 9000; 
                ob.Lyear2 = obb.Gan[(int)(D % 10)] + obb.Zhi[(int)(D % 12)];    // 干支纪年(立春)
                D = ob.Lyear0 + 9000; 
                ob.Lyear3 = obb.Gan[(int)(D % 10)] + obb.Zhi[(int)(D % 12)];   // 干支纪年(正月)
                ob.Lyear4 = ob.Lyear0 + 1984 + 2698;    // 黄帝纪年

                var zhiList = new List<string>(obb.Zhi);
                ob.LShX1 = obb.ShX[zhiList.IndexOf(ob.Lyear2.Substring(1, 1))];
                ob.LShX2 = obb.ShX[zhiList.IndexOf(ob.Lyear3.Substring(1, 1))];

                // 纪月处理,1998年12月7(大雪)开始连续进行节气计数,0为甲子
                mk = (int)LunarHelper.int2((ob.d0 - SSQ.ZQ[0]) / 30.43685); 
                if (mk < 12 && ob.d0 >= SSQ.ZQ[2 * mk + 1]) mk++;  //相对大雪的月数计算,mk的取值范围0-12

                D = mk + LunarHelper.int2((SSQ.ZQ[12] + 390) / 365.2422) * 12 + 900000; //相对于1998年12月7(大雪)的月数,900000为正数基数
                ob.Lmonth = D % 12;
                ob.Lmonth2 = obb.Gan[(int)(D % 10)] + obb.Zhi[(int)(D % 12)];

                // 纪日,2000年1月7日起算
                D = ob.d0 - 6 + 9000000;
                ob.Lday2 = obb.Gan[(int)(D % 10)] + obb.Zhi[(int)(D % 12)];

                // 星座
                mk = (int)LunarHelper.int2((ob.d0 - SSQ.ZQ[0] - 15) / 30.43685); if (mk < 11 && ob.d0 >= SSQ.ZQ[2 * mk + 2]) mk++; //星座所在月的序数,(如果j=13,ob.d0不会超过第14号中气)
                ob.XiZ = obb.XiZ[(int)((mk + 12) % 12)] + "座";

                // 回历
                oba.getHuiLi(ob.d0, ob);

                // 节日
                ob.A = ob.B = ob.C = ""; ob.Fjia = 0;
                oba.getDayName(ob, ob);   //公历
                obb.getDayName(ob, ob);   //农历
            }

            // 以下是月相与节气的处理
            double d, jd2 = Bd0 + JD.deltatT2(Bd0) - 8d / 24d;
            int xn;

            // 月相查找
            w = XL.MS_aLon(jd2 / 36525, 10, 3);
            w = LunarHelper.int2((w - 0.78) / Math.PI * 2) * Math.PI / 2;
            do
            {
                d = obb.so_accurate(w);
                D = LunarHelper.int2(d + 0.5);
                xn = (int)LunarHelper.int2(w / LunarHelper.pi2 * 4 + 4000000.01) % 4;
                w += LunarHelper.pi2 / 4;
                if (D >= Bd0 + Bdn) break;
                if (D < Bd0) continue;
                ob = (this.lun[(int)(D - Bd0)]);
                ob.yxmc = obb.yxmc[xn];     // 取得月相名称
                ob.yxjd = d.ToString();
                ob.yxsj = JD.timeStr(d);
            } while (D + 5 < Bd0 + Bdn);

            // 节气查找
            w = XL.S_aLon(jd2 / 36525, 3);
            w = LunarHelper.int2((w - 0.13) / LunarHelper.pi2 * 24) * LunarHelper.pi2 / 24;
            do
            {
                d = obb.qi_accurate(w);
                D = LunarHelper.int2(d + 0.5);
                xn = (int)LunarHelper.int2(w / LunarHelper.pi2 * 24 + 24000006.01) % 24;
                w += LunarHelper.pi2 / 24;
                if (D >= Bd0 + Bdn) break;
                if (D < Bd0) continue;
                ob = (this.lun[(int)(D - Bd0)]);
                ob.jqmc = obb.jqmc[xn];     // 取得节气名称
                ob.jqjd = d.ToString();
                ob.jqsj = JD.timeStr(d);
            } while (D + 12 < Bd0 + Bdn);


            // C#: 转换时新增的代码行
            this.CalcRiJianThisMonth();    // 计算本月所有日期的日十二建信息

            foreach (var o in lun)
            {
                o.Lunar = this;
            }
        }


        /// <summary>
        /// html月历生成,结果返回在lun中,curJD为当前日期(用于设置今日标识)
        /// </summary>
        /// <param name="By">要生成月历的年</param>
        /// <param name="Bm">要生成月历的月</param>
        /// <param name="curJD">当前日期(用于设置今日标识)</param>
        public void yueLiHTML(int By, int Bm, double curJD)
        {
            string sty_head = " style='font-family: 宋体; font-size: 14px; text-align: center; background-color: #E0E0FF; color: #000000; font-weight: bold' ";
            string sty_body = " style='font-family: 宋体; font-size: 12px; text-align: center ' ";
            string sty_date = " style='font-family: Arial Black; text-align: center;font-size: 20px' ";
            string sty_date2 = " style='font-family: Arial Black; text-align: center;font-size: 20px; color: #FF0000' ";
            string sty_cur = " style='background-color:#90D050' ";

            int i, j;
            string c, c2;
            StringBuilder cr = new StringBuilder();    // C#: 为提高字符串处理效率, 使用 StringBuilder
            string isM;
            OB ob;     // 日历物件

            this.yueLiCalc(By, Bm);    // 农历计算

            // 年份处理
            c = this.nianhao + " 农历" + this.Ly + "年【" + this.ShX + "年】";       // 干支纪年
            if (c.Length > 33) c = "<span style='font-size:12px'>" + c + "</span>";
            else c = "<span style='font-size:16px;font-weight:bold'>" + c + "</span>";
            this.pg0 = c;
            
            //月历处理
            StringBuilder ta0 = new StringBuilder("<tr>"
              + "<td" + sty_head + "width='%14'>日</td>"
              + "<td" + sty_head + "width='%14'>一</td>"
              + "<td" + sty_head + "width='%14'>二</td>"
              + "<td" + sty_head + "width='%14'>三</td>"
              + "<td" + sty_head + "width='%14'>四</td>"
              + "<td" + sty_head + "width='%14'>五</td>"
              + "<td" + sty_head + "width='%14'>六</td><tr>");    // C#: 为提高字符串处理效率, 使用 StringBuilder
            for (i = 0; i < this.dn; i++)
            { 
                // 遍历本月各日(公历), 生成第 i 日的日历页面
                ob = (this.lun[i]);
                if (i == 0)
                {
                    for (j = 0; j < this.w0; j++)     // 首行前面的空单元格(依据: 本月首日的星期)
                        cr.Append("<td" + sty_body + "></td>");
                }

                c = ""; isM = "";  // 文字格式控制项
                if (ob.A.Length > 0) 
                    c += "<font color=red>" + this.substr2(ob.A, 4, "..") + "</font>";
                if (c.Length <= 0 && ob.B.Length > 0) 
                    c = "<font color=blue>" + this.substr2(ob.B, 4, "..") + "</font>";
                if (c.Length <= 0 && ob.Ldc == "初一") 
                    c = ob.Lleap + ob.Lmc + "月" + (ob.Ldn == 30 ? "大" : "小");   // 农历历月(闰月及大小等)
                if (c.Length <= 0) 
                    c = ob.Ldc;   // 取农历日名称

                if (ob.yxmc == "朔") isM = "<font color=#505000>●</font>";           // 取月相
                if (ob.yxmc == "望") isM = "<font color=#F0B000>●</font>";           // 取月相
                if (ob.yxmc == "上弦") isM = "<font color=#F0B000><b>∪</b></font>";
                if (ob.yxmc == "下弦") isM = "<font color=#F0B000><b>∩</b></font>";

                if (ob.jqmc.Length > 0) 
                    isM += "<font color=#00C000>◆</font>";  // 定气标记

                if (ob.Fjia != 0) 
                    c2 = sty_date2; //节日置红色
                else c2 = sty_date;

                //c2 += " onmouseover='showMessD(" + i + ")'";     // C#: 注掉鼠标事件, 下同
                //c2 += " onmouseout ='showMessD(-1)'";

                c2 = "<span" + c2 + ">" + ob.d + "</span>"; //公历的日名称

                if (ob.d0 == curJD) 
                    c2 = "<span" + sty_cur + ">" + c2 + "</span>";   // 今日标识


                //cr += "<td" + sty_body + "width='14%'>" + c2 + "<br>" + isM + c + "</td>";      // C#: 注释, 改写如下:
                cr.Append("<td" + sty_body + "width='14%' onmouseover='changeBackcolor(this,1)' " +
                              "onmouseout='changeBackcolor(this,0)'>" + 
                              c2 + "<br>" + isM + c + "</td>");               // C#: 新改写的语句, 增加鼠标事件处理

                if (i == this.dn - 1)
                {
                    for (j = 0; j < 6 - ob.week; j++)      // 末行后面的空单元格(依据: 本月末日的星期)
                        cr.Append("<td" + sty_body + "></td>");
                }
                if (i == this.dn - 1 || ob.week == 6)
                {
                    ta0.Append("<tr>" + cr.ToString() + "</tr>"); 
                    cr.Remove(0, cr.Length);
                }
            }
            this.pg1 = "<table border=0 cellpadding=3 cellspacing=1 width='100%'>" + ta0.ToString() + "</table>";

            string b2 = "", b3 = "", b4 = "";
            int c__;
            for (i = 0; i < this.dn; i++)
            {
                ob = (this.lun[i]);
                c__ = i + 1;
                if (c__ < 10)
                    c = "&nbsp;" + c__;
                else
                    c = c__.ToString();
                //if(ob.Ldc =="初一") b1 += c +"日 "+ob.Lleap+ob.Lmc+"月" + (ob.Ldn==30?"大":"小")+" &nbsp;";
                if (ob.yxmc == "朔" || ob.yxmc == "望") b2 += c + "日 " + ob.yxsj + ob.yxmc + "月 &nbsp;";
                if (ob.yxmc == "上弦" || ob.yxmc == "下弦") b3 += c + "日 " + ob.yxsj + ob.yxmc + " &nbsp;";
                if (ob.jqmc.Length > 0) b4 += c + "日 " + ob.jqsj + ob.jqmc + " &nbsp;";
            }
            this.pg2 = b2 + "<br>" + b3 + "<br>" + b4;

            this.yueLiText(By, Bm, curJD);   // C#: 转换时新增功能而增加的语句
        }



        #region 初次转换的 yueLiHTML() 方法, 因涉及大量的字符串操作, 故拟使用 StringBuiler 改写以提高效率, 但保留原代码如下

        //public void yueLiHTML(int By, int Bm, double curJD)
        //{
        //    string sty_head = " style='font-family: 宋体; font-size: 14px; text-align: center; background-color: #E0E0FF; color: #000000; font-weight: bold' ";
        //    string sty_body = " style='font-family: 宋体; font-size: 12px; text-align: center ' ";
        //    string sty_date = " style='font-family: Arial Black; text-align: center;font-size: 20px' ";
        //    string sty_date2 = " style='font-family: Arial Black; text-align: center;font-size: 20px; color: #FF0000' ";
        //    string sty_cur = " style='background-color:#90D050' ";

        //    int i, j;
        //    string c, c2;
        //    string cr = "", isM;
        //    OB ob;     // 日历物件

        //    this.yueLiCalc(By, Bm);    // 农历计算

        //    // 年份处理
        //    c = this.nianhao + " 农历" + this.Ly + "年【" + this.ShX + "年】";       // 干支纪年
        //    if (c.Length > 33) c = "<span style='font-size:12px'>" + c + "</span>";
        //    else c = "<span style='font-size:16px;font-weight:bold'>" + c + "</span>";
        //    this.pg0 = c;

        //    //月历处理
        //    string ta0 = "<tr>"
        //      + "<td" + sty_head + "width='%14'>日</td>"
        //      + "<td" + sty_head + "width='%14'>一</td>"
        //      + "<td" + sty_head + "width='%14'>二</td>"
        //      + "<td" + sty_head + "width='%14'>三</td>"
        //      + "<td" + sty_head + "width='%14'>四</td>"
        //      + "<td" + sty_head + "width='%14'>五</td>"
        //      + "<td" + sty_head + "width='%14'>六</td><tr>";
        //    for (i = 0; i < this.dn; i++)
        //    {
        //        // 遍历本月各日(公历), 生成第 i 日的日历页面
        //        ob = (this.lun[i]);
        //        if (i == 0)
        //        {
        //            for (j = 0; j < this.w0; j++)     // 首行前面的空单元格(依据: 本月首日的星期)
        //                cr += "<td" + sty_body + "></td>";
        //        }

        //        c = ""; isM = "";  // 文字格式控制项
        //        if (ob.A.Length > 0)
        //            c += "<font color=red>" + this.substr2(ob.A, 4, "..") + "</font>";
        //        if (c.Length <= 0 && ob.B.Length > 0)
        //            c = "<font color=blue>" + this.substr2(ob.B, 4, "..") + "</font>";
        //        if (c.Length <= 0 && ob.Ldc == "初一")
        //            c = ob.Lleap + ob.Lmc + "月" + (ob.Ldn == 30 ? "大" : "小");   // 农历历月(闰月及大小等)
        //        if (c.Length <= 0)
        //            c = ob.Ldc;   // 取农历日名称

        //        if (ob.yxmc == "朔") isM = "<font color=#505000>●</font>";           // 取月相
        //        if (ob.yxmc == "望") isM = "<font color=#F0B000>●</font>";           // 取月相
        //        if (ob.yxmc == "上弦") isM = "<font color=#F0B000><b>∪</b></font>";
        //        if (ob.yxmc == "下弦") isM = "<font color=#F0B000><b>∩</b></font>";

        //        if (ob.jqmc.Length > 0)
        //            isM += "<font color=#00C000>◆</font>";  // 定气标记

        //        if (ob.Fjia != 0)
        //            c2 = sty_date2; //节日置红色
        //        else c2 = sty_date;

        //        //c2 += " onmouseover='showMessD(" + i + ")'";     // C#: 注掉鼠标事件, 下同
        //        //c2 += " onmouseout ='showMessD(-1)'";

        //        c2 = "<span" + c2 + ">" + ob.d + "</span>"; //公历的日名称

        //        if (ob.d0 == curJD)
        //            c2 = "<span" + sty_cur + ">" + c2 + "</span>";   // 今日标识


        //        //cr += "<td" + sty_body + "width='14%'>" + c2 + "<br>" + isM + c + "</td>";      // C#: 注释, 改写如下:
        //        cr += "<td" + sty_body + "width='14%' onmouseover='changeBackcolor(this,1)' " +
        //                      "onmouseout='changeBackcolor(this,0)'>" +
        //                      c2 + "<br>" + isM + c + "</td>";               // C#: 新改写的语句, 增加鼠标事件处理

        //        if (i == this.dn - 1)
        //        {
        //            for (j = 0; j < 6 - ob.week; j++)      // 末行后面的空单元格(依据: 本月末日的星期)
        //                cr += "<td" + sty_body + "></td>";
        //        }
        //        if (i == this.dn - 1 || ob.week == 6)
        //        {
        //            ta0 += "<tr>" + cr + "</tr>";
        //            cr = "";
        //        }
        //    }
        //    this.pg1 = "<table border=0 cellpadding=3 cellspacing=1 width='100%'>" + ta0 + "</table>";

        //    string b2 = "", b3 = "", b4 = "";
        //    int c__;
        //    for (i = 0; i < this.dn; i++)
        //    {
        //        ob = (this.lun[i]);
        //        c__ = i + 1;
        //        if (c__ < 10)
        //            c = "&nbsp;" + c__;
        //        else
        //            c = c__.ToString();
        //        //if(ob.Ldc =="初一") b1 += c +"日 "+ob.Lleap+ob.Lmc+"月" + (ob.Ldn==30?"大":"小")+" &nbsp;";
        //        if (ob.yxmc == "朔" || ob.yxmc == "望") b2 += c + "日 " + ob.yxsj + ob.yxmc + "月 &nbsp;";
        //        if (ob.yxmc == "上弦" || ob.yxmc == "下弦") b3 += c + "日 " + ob.yxsj + ob.yxmc + " &nbsp;";
        //        if (ob.jqmc.Length > 0) b4 += c + "日 " + ob.jqsj + ob.jqmc + " &nbsp;";
        //    }
        //    this.pg2 = b2 + "<br>" + b3 + "<br>" + b4;

        //    this.yueLiText(By, Bm, curJD);   // C#: 转换时新增功能而增加的语句
        //}
        
        #endregion



        #region 原 Lunar.js 中的独立函数转换到类中


        /// <summary>
        /// 按交节时刻生成 html 年历
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public string nianLiHTML(double y)
        {
            int i, j;
            StringBuilder s = new StringBuilder();    // C#: 为提高字符串处理效率, 使用 StringBuilder
            StringBuilder s1 = new StringBuilder();    // C#: 为提高字符串处理效率, 使用 StringBuilder
            string s2;
            double v, qi = 0;

            SSQ.calcY(LunarHelper.int2((y - 2000) * 365.2422 + 180));

            for (i = 0; i < 14; i++)
            {
                if (SSQ.HS[i + 1] > SSQ.ZQ[24]) break;    // 已包含下一年的冬至
                if (SSQ.leap != 0 && i == SSQ.leap) s1.Append("闰"); 
                else s1.Append("·");
                s1.Append(SSQ.ym[i]); 
                if (s1.ToString().Length < 3) s1.Append("月");
                s1.Append(SSQ.dx[i] > 29 ? "大" : "小");
                s1.Append(" " + JD.setFromJD_str(SSQ.HS[i] + LunarHelper.J2000).Substring(6, 5));    // C#: 取实历初一的时间

                v = obb.so_accurate2(SSQ.HS[i]);
                s2 = "(" + JD.setFromJD_str(v + LunarHelper.J2000).Substring(9, 11) + ")";    // C#: 取每月朔的时间(即初一)
                if (LunarHelper.int2(v + 0.5) != SSQ.HS[i]) s2 = "<font color=red>" + s2 + "</font>";
                //v=(v+0.5+LunarHelper.J2000)%1; if(v>0.5) v=1-v; if(v<8/1440) s2 = "<u>"+s2+"</u>"; //对靠近0点的加注
                s1.Append(s2);

                for (j = -2; j < 24; j++)
                {
                    if (j >= 0) qi = SSQ.ZQ[j];
                    if (j == -1) qi = SSQ.ZQ.pe1;
                    if (j == -2) qi = SSQ.ZQ.pe2;

                    if (qi < SSQ.HS[i] || qi >= SSQ.HS[i + 1]) continue;
                    s1.Append(" " + obb.jqmc[(j + 24) % 24] + JD.setFromJD_str(qi + LunarHelper.J2000).Substring(6, 5));    // C#: 取节气名称和实历交节日期

                    v = obb.qi_accurate2(qi);
                    s2 = "(" + JD.setFromJD_str(v + LunarHelper.J2000).Substring(9, 11) + ")";    // C#: 取节气时间(上年大雪-本年大雪)
                    if (LunarHelper.int2(v + 0.5) != qi) s2 = "<font color=red>" + s2 + "</font>";
                    //v=(v+0.5+LunarHelper.J2000)%1; if(v>0.5) v=1-v; if(v<8/1440) s2 = "<u>"+s2+"</u>"; //对靠近0点的加注
                    s1.Append(s2);
                }
                s.Append(s1.ToString() + "<br>");
                s1.Remove(0, s1.Length);     // C#: 在转换时将原来的字符串 s1 改写为 StringBuiler, 因此添加本句
            }
            return s.ToString();
        }


        /// <summary>
        /// 按交节干支生成 html 年历
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public string nianLi2HTML(double y)
        {
            int i, j;
            StringBuilder s = new StringBuilder();     // C#: 为提高字符串处理效率, 使用 StringBuilder
            StringBuilder s1 = new StringBuilder();    // C#: 为提高字符串处理效率, 使用 StringBuilder
            double v, v2, qi = 0;
            SSQ.calcY(LunarHelper.int2((y - 2000) * 365.2422 + 180));
            for (i = 0; i < 14; i++)
            {
                if (SSQ.HS[i + 1] > SSQ.ZQ[24]) break; //已包含下一年的冬至
                if (SSQ.leap != 0 && i == SSQ.leap) s1.Append("闰"); 
                else s1.Append("·");
                s1.Append(SSQ.ym[i]); 
                if (s1.ToString().Length < 3) s1.Append("月");
                s1.Append(SSQ.dx[i] > 29 ? "大" : "小");
                v = SSQ.HS[i] + LunarHelper.J2000;
                s1.Append(" " + obb.Gan[(int)((v + 9) % 10)] + obb.Zhi[(int)((v + 1) % 12)]);
                s1.Append(" " + JD.setFromJD_str(v).Substring(6, 5));


                for (j = -2; j < 24; j++)
                {
                    if (j >= 0) qi = SSQ.ZQ[j];
                    if (j == -1) qi = SSQ.ZQ.pe1;
                    if (j == -2) qi = SSQ.ZQ.pe2;

                    if (qi < SSQ.HS[i] || qi >= SSQ.HS[i + 1]) continue;
                    v2 = qi + LunarHelper.J2000;
                    s1.Append(" " + obb.rmc[(int)(v2 - v)] + obb.Gan[(int)((v2 + 9) % 10)] + obb.Zhi[(int)((v2 + 1) % 12)]);
                    s1.Append(obb.jqmc[(j + 24) % 24] + JD.setFromJD_str(qi + LunarHelper.J2000).Substring(6, 5));
                }
                s.Append(s1.ToString() + "<br>");
                s1.Remove(0, s1.Length);     // C#: 在转换时将原来的字符串 s1 改写为 StringBuiler, 因此添加本句
            }
            return s.ToString();
        }


        #endregion 原 Lunar.js 中的独立函数转换到类中
        #endregion 公共方法



        #region 转换时新增的属性

        /// <summary>
        /// 文本年号信息
        /// </summary>
        public string pg0_text { get; set; }    // = "";       // C#: 属性 pg*_text, 与 pg* 属性基本对应(有删减), 但使用纯文本格式
        
        /// <summary>
        /// 文本月历, 第1列: 日期, 第2列: 放假标识, 第3列: 星期几, 第4列: 农历名称(含节气名,纪念日名等)
        ///           第5列以后: 月相符号或节气标识
        /// </summary>
        public string pg1_text { get; set; }    // = "";
        
        /// <summary>
        /// 本月的月相与节气文本信息
        /// </summary>
        public string pg2_text { get; set; }    // = "";

        #endregion 转换时新增的属性



        #region 转换时新增的方法

        /// <summary>
        /// 文本月历生成
        /// </summary>
        /// <param name="By"></param>
        /// <param name="Bm"></param>
        /// <param name="curJD"></param>
        public void yueLiText(int By, int Bm, double curJD)
        {
            int i, j;
            string c;
            string isM;
            OB ob; //日历物件

            //年份处理
            this.pg0_text = this.nianhao + " 农历" + this.Ly + "年【" + this.ShX + "年】"; ;

            //月历处理
            StringBuilder sb = new StringBuilder();
            for (i = 0; i < this.dn; i++)
            { 
                //遍历本月各日(公历), 生成第 i 日的日历页面
                ob = (this.lun[i]);
                if (i == 0) { for (j = 0; j < this.w0; j++) sb.AppendLine(); } //首行前面的空单元格

                sb.Append(ob.d.ToString() + "\t");
                sb.Append(ob.Fjia + "\t");    // C#: 添加放假的标识
                sb.Append(ob.week + "\t");    // C#: 添加星期的标识

                c = ""; isM = "";    // 文字格式控制项
                if (ob.A.Length > 0) c = this.substr2(ob.A, 4, "..");
                if (c.Length <= 0 && ob.B.Length > 0) c = this.substr2(ob.B, 4, "..");
                if (c.Length <= 0 && ob.Ldc == "初一")
                    c = ob.Lleap + ob.Lmc + "月" + (ob.Ldn == 30 ? "大" : "小");    // 农历历月(闰月及大小等)
                if (c.Length <= 0) c = ob.Ldc;      // 取农历日名称

                if (ob.yxmc == "朔") isM = "●";    // 取月相
                if (ob.yxmc == "望") isM = "○";    // 取月相
                if (ob.yxmc == "上弦") isM = "∪";
                if (ob.yxmc == "下弦") isM = "∩";

                if (ob.jqmc.Length > 0) isM += "◆";    // 定气标记

                sb.Append(c + "\t" + isM);    // 公历的日名称

                sb.AppendLine();
            }
            this.pg1_text = sb.ToString();

            this.pg2_text = this.pg2.Replace("&nbsp;", " ").Replace("<br>", "\r\n");

        }



        /// <summary>
        /// 指定某日, 计算出它的所属节(气), 上一节(气), 下一节(气)信息, 并把计算结果保存在日对象中
        /// </summary>
        /// <param name="ob"></param>
        /// <returns>计算成功返回 true, 否则返回 false </returns>
        public bool CalcJieQiInfo(OB ob, CalcJieQiType calcType)
        {
            const int JiQiInfo_Name = 0;
            const int JiQiInfo_JDTime = 1;
            const int JiQiInfo_Time = 2;
            const int JiQiInfo_HistoricalJDTime = 3;
            const int JiQiInfo_HistoricalTime = 4;
            const int JiQiInfo_DifferentTime = 5;
            const int JiQiInfo_JieOrQi = 6;
            const int JiQiInfo_YueJian = 7;
            const int JiQiInfo_DayDifference = 8;

            const int PrevJieQiFlag = 0;
            const int ThisJieQiFlag = 1;
            const int NextJieQiFlag = 2;
            
            double y = ob.y;
            int j, counter = 0, thisJie = -1, jieQiPos = 0;
            JieQiInfo jieqiInfo;
            int[] jieqiInfoPos = new int[3];
            object[,] jieqiList = new object[31, 9];

            double v, qi = 0;

            int num = SSQ.calcJieQi(LunarHelper.int2((y - 2000) * 365.2422 + 180), true);    // 计算节气, 以"霜降"开始

            for (j = 0; j < num; j++)    // 建表
            {
                if (calcType == CalcJieQiType.CalcJie)   // 只计算节
                    if (((j + 24) % 24) % 2 == 0)           // 气(跳过, 只使用节)
                        continue;

                if (calcType == CalcJieQiType.CalcQi)    // 只计算气
                    if (((j + 24) % 24) % 2 == 1)           // 节(跳过, 只使用气)
                        continue;

                qi = SSQ.ZQ[j];

                v = obb.qi_accurate2(qi);
                jieqiList[counter, JiQiInfo_Time] = JD.setFromJD_str(v + LunarHelper.J2000);

                jieQiPos = (j + 24) % 24 + 20;      // 与 obb.jqmc 对应, "霜降"距首位"冬至"的距离为 20
                if(jieQiPos >= 24)
                    jieQiPos -= 24;

                jieqiList[counter, JiQiInfo_JieOrQi] = true;
                jieqiList[counter, JiQiInfo_Name] = obb.jqmc[jieQiPos];
                jieqiList[counter, JiQiInfo_YueJian] = obb.JieQiYueJian[jieQiPos];
                jieqiList[counter, JiQiInfo_JDTime] = v + LunarHelper.J2000;

                jieqiList[counter, JiQiInfo_HistoricalTime] = JD.setFromJD_str(qi + LunarHelper.J2000);
                jieqiList[counter, JiQiInfo_HistoricalJDTime] = qi + LunarHelper.J2000;
                jieqiList[counter, JiQiInfo_DifferentTime] = (LunarHelper.int2(v + 0.5) != qi ? true : false);

                jieqiList[counter, JiQiInfo_DayDifference] = (int)(LunarHelper.int2(v + 0.5) - qi);

                counter ++;
            }

            for(j = 0; j < SSQ.ZQ.Count; j++)    // △重要: 由于调用了 SSQ.calcJieQi 方法, 计算了 31 个节气(超出年周期)数据, 故应清零
                SSQ.ZQ[j] = 0;

            if (ob.y >= 0)
            {
                int ymd = ob.y * 10000 + ob.m * 100 + ob.d;     // 转换成为数值表示的日期: 如 20090422
                for (j = counter - 1; j >= 0; j--)    // 逆序查表(表中的交节日期数据是由小到大排列的)
                {
                    string jieqiTime = (string)jieqiList[j, JiQiInfo_Time];
                    int jieqiYear = int.Parse(jieqiTime.Substring(0, 5));
                    int jieqiMonth = int.Parse(jieqiTime.Substring(6, 2));
                    int jieqiDay = int.Parse(jieqiTime.Substring(9, 2));
                    if (jieqiYear * 10000 + jieqiMonth * 100 + jieqiDay <= ymd)    // 找到所属的节气
                    {
                        thisJie = j;
                        break;
                    }
                }
            }
            else
            {
                int ymd = ob.y * 10000 - (99 - ob.m) * 100 - (99 - ob.d);
                for (j = 0; j < counter; j++)    // 顺序查表(表中的交节日期数据是由小到大排列的, 计算为数值时也要严格递增)
                {
                    string jieqiTime = (string)jieqiList[j, JiQiInfo_Time];
                    int jieqiYear = int.Parse(jieqiTime.Substring(0, 5));
                    int jieqiMonth = int.Parse(jieqiTime.Substring(6, 2));
                    int jieqiDay = int.Parse(jieqiTime.Substring(9, 2));
                    int jieqiYmd = jieqiYear * 10000 - (99 - jieqiMonth) * 100 - (99 - jieqiDay);
                    if (jieqiYmd >= ymd)
                    {
                        if (jieqiYmd > ymd)
                            thisJie = j - 1;
                        else
                            thisJie = j;
                        break;
                    }
                }
            }


            if (thisJie > 0 && thisJie < counter)
            {
                jieqiInfoPos[ThisJieQiFlag] = thisJie;
                jieqiInfoPos[PrevJieQiFlag] = jieqiInfoPos[ThisJieQiFlag] - 1;
                jieqiInfoPos[NextJieQiFlag] = jieqiInfoPos[ThisJieQiFlag] + 1;

                for (j = 0; j < jieqiInfoPos.Length; j++)
                {
                    switch (j)
                    {
                        case PrevJieQiFlag:
                            jieqiInfo = ob.PreviousJieQi;
                            break;

                        case ThisJieQiFlag:
                            jieqiInfo = ob.ThisJieQi;
                            break;
                        
                        case NextJieQiFlag:
                            jieqiInfo = ob.NextJieQi;
                            break;
                        
                        default:
                            jieqiInfo = null;
                            break;
                    }

                    if (jieqiInfo != null)
                    {
                        jieqiInfo.JieOrQi = (bool)jieqiList[jieqiInfoPos[j], JiQiInfo_JieOrQi];
                        jieqiInfo.Name = (string)jieqiList[jieqiInfoPos[j], JiQiInfo_Name];
                        jieqiInfo.YueJian = (string)jieqiList[jieqiInfoPos[j], JiQiInfo_YueJian];
                        jieqiInfo.Time = (string)jieqiList[jieqiInfoPos[j], JiQiInfo_Time];
                        jieqiInfo.JDTime = (double)jieqiList[jieqiInfoPos[j], JiQiInfo_JDTime];

                        jieqiInfo.HistoricalTime = (string)jieqiList[jieqiInfoPos[j], JiQiInfo_HistoricalTime];
                        jieqiInfo.HistoricalJDTime = (double)jieqiList[jieqiInfoPos[j], JiQiInfo_HistoricalJDTime];
                        jieqiInfo.DifferentTime = (bool)jieqiList[jieqiInfoPos[j], JiQiInfo_DifferentTime];

                        jieqiInfo.DayDifference = (int)jieqiList[jieqiInfoPos[j], JiQiInfo_DayDifference];
                    }
                }

                return true;
            }

            return false;
        }



        /// <summary>
        /// 根据指定的月建(地支), 查找并返回指定日(地支)的日十二建
        /// </summary>
        /// <param name="yueJian">月建(地支)</param>
        /// <param name="riZhi">要计算日十二建的指定日(地支)</param>
        /// <returns></returns>
        public string GetRi12Jian(string yueJian, string riZhi)
        {
            string result = String.Empty;
            int posYueJian = -1, posRiZhi = -1, pos;

            for (int i = 0; i < obb.Zhi.Length; i++)
            {
                if (obb.Zhi[i] == yueJian)
                {
                    posYueJian = i;
                    break;
                }
            }

            if (posYueJian >= 0)
            {
                for (int i = posYueJian + 1; i < obb.DoubleZhi.Length; i++)
                {
                    if (obb.DoubleZhi[i] == riZhi)
                    {
                        posRiZhi = i;
                        break;
                    }
                }

                if (posRiZhi >= posYueJian)
                {
                    pos = posRiZhi - posYueJian;
                    result = obb.DoubleRiJian12[pos];
                }
            }

            return result;
        }



        /// <summary>
        /// 计算本月所有日期的日十二建信息
        /// </summary>
        private void CalcRiJianThisMonth()
        {
            OB lunOb;
            string yuejian = String.Empty;

            //OB ob = new OB();

            for (int i = 0; i < this.dn; i++)    // 遍历月
            {
                lunOb = this.lun[i];

                //if (i == 0 || lunOb.jqmc.Trim().Length > 0 || lunOb.Ljq.Trim().Length > 0)    // 每月第 1 日, 或遇到了交节气日, 则计算该日的所属节气等
                //{
                //    ob.y = lunOb.y;
                //    ob.m = lunOb.m;
                //    ob.d = lunOb.d;
                //    this.CalcJieQiInfo(ob, CalcJieQiType.CalcJie);
                //}
                //yuejian = ob.ThisJieQi.YueJian;
                //if (ob.ThisJieQi.DifferentTime)
                //    yuejian = LunarHelper.SUBSTRING(lunOb.Lmonth2, 1, 1);     // 调整为实历

                // 可直接使用该属性的月建而无需再次计算节气, 但上述被注释的代码也可用(主要为了测试 CalcJieQiInfo 方法, 暂保留)
                yuejian = LunarHelper.SUBSTRING(lunOb.Lmonth2, 1, 1);

                lunOb.Ri12Jian = this.GetRi12Jian(yuejian, LunarHelper.SUBSTRING(lunOb.Lday2, 1, 1));    // 计算日十二建
            }
        }

        #endregion



    }
}
