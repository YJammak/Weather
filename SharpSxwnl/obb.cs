using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;

namespace SharpSxwnl
{
    /// <summary>
    /// 农历基础构件(常数、通用函数等)
    /// </summary>
    public static class obb
    {
        #region 公共属性(注: 初始转换时为公共字段, 已改写, 请参阅“转换时增加的私有字段”)

        /// <summary>
        /// 数字 0 - 10 对应的中文名称
        /// </summary>
        public static string[] numCn
        {
            get => obb.__numCn;
            set => obb.__numCn = value;
        }

        /// <summary>
        /// 十天干表
        /// </summary>
        public static string[] Gan
        {
            get => obb.__Gan;
            set => obb.__Gan = value;
        }

        /// <summary>
        /// 十二地支表
        /// </summary>
        public static string[] Zhi
        {
            get => obb.__Zhi;
            set => obb.__Zhi = value;
        }

        /// <summary>
        /// 十二属相表
        /// </summary>
        public static string[] ShX
        {
            get => obb.__ShX;
            set => obb.__ShX = value;
        }

        /// <summary>
        /// 十二星座表
        /// </summary>
        public static string[] XiZ
        {
            get => obb.__XiZ;
            set => obb.__XiZ = value;
        }

        /// <summary>
        /// 月相名称表
        /// </summary>
        public static string[] yxmc
        {
            get => obb.__yxmc;
            set => obb.__yxmc = value;
        }

        /// <summary>
        /// 廿四节气表
        /// </summary>
        public static string[] jqmc
        {
            get => obb.__jqmc;
            set => obb.__jqmc = value;
        }

        /// <summary>
        /// 农历各月的名称, 从 "十一" 月开始, 即从月建 "子" 开始, 与十二地支的顺序对应
        /// </summary>
        public static string[] ymc
        {
            get => obb.__ymc;
            set => obb.__ymc = value;
        }

        /// <summary>
        /// 农历各日的名称
        /// </summary>
        public static string[] rmc
        {
            get => obb.__rmc;
            set => obb.__rmc = value;
        }

        /// <summary>
        /// 历史纪年表
        /// </summary>
        public static JnbArrayList JNB
        {
            get => obb.__JNB;
            set => obb.__JNB = value;
        }

        #endregion 公共属性



        #region 公共方法

        /// <summary>
        /// 取年号
        /// </summary>
        /// <param name="y">公历年(天文纪年, 如 -1 表示常规纪年的"公元前2年")</param>
        /// <returns></returns>
        public static string getNH(int y)
        {
            int i, j;
            string c, s = "";
            JnbArrayList ob = obb.JNB;
            for (i = 0; i < ob.Count; i += 7)
            {
                j = (int)ob[i];
                if (y < j || y >= j + (int)ob[i + 1]) continue;
                c = (string)ob[i + 6] + (y - j + 1 + (int)ob[i + 2]) + "年";   // 年号及年次
                s += (s.Length > 0 ? ";" : "") + "[" + ob[i + 3] + "]" + ob[i + 4] + " " + ob[i + 5] + " " + c;   // i为年号元年,i+3朝代,i+4朝号,i+5皇帝,i+6年号
            }
            return s;
        }


        /// <summary>
        /// 计算农历节日
        /// </summary>
        /// <param name="u"></param>
        /// <param name="r"></param>
        public static void getDayName(OB u, OB r)
        {
            int i;

            // 按农历日期查找重点节假日
            string d = u.Lmc + (u.Lmc.Length < 2 ? "月" : "") + u.Ldc;
            if (u.Lleap != "闰")
            {
                for (i = 0; i < obb.LunarFeasts.Length; i++)     // C#: 查表, 查找农历节假日
                {
                    if (d == obb.LunarFeasts[i].Lmc)
                    {
                        r.A += obb.LunarFeasts[i].A;
                        r.B += obb.LunarFeasts[i].B;
                        r.C += obb.LunarFeasts[i].C;
                        r.Fjia = obb.LunarFeasts[i].Fjia;
                    }
                }
            }
            if (u.Lmc2 == "正")
            { 
                // 最后一月
                if (d == "十二三十" && u.Ldn == 30) { r.A += "除夕 "; r.Fjia = 1; }
                if (d == "十二廿九" && u.Ldn == 29) { r.A += "除夕 "; r.Fjia = 1; }
                if (d == "十二廿三") r.B += "小年 ";
            }
            if (u.Ljq.Length > 0)
            {
                for (i = 0; i < obb.JieQiFeasts.Length; i++)    // C#: 查找是否有放假的节气
                {
                    if (u.Ljq == obb.JieQiFeasts[i])               // C#: 匹配
                        break;
                }
                if (i < obb.JieQiFeasts.Length) { r.A += u.Ljq + " "; r.Fjia = 1; }
                else r.B += u.Ljq + " ";
            }

            // 农历杂节
            string w, w2;
            if (u.cur_dz >= 0 && u.cur_dz < 81)
            { // 数九
                w = obb.numCn[(int)Math.Floor(u.cur_dz / 9) + 1];
                if (u.cur_dz % 9 == 0) r.B += "『" + w + "九』 ";
                else r.C += w + "九第" + (u.cur_dz % 9 + 1) + "天 ";
            }

            w = u.Lday2.Substring(0, 1);
            w2 = u.Lday2.Substring(1, 1);
            if (u.cur_xz > 20 && u.cur_xz <= 30 && w == "庚") r.B += "初伏 ";
            if (u.cur_xz > 30 && u.cur_xz <= 40 && w == "庚") r.B += "中伏 ";
            if (u.cur_lq > 0 && u.cur_lq <= 10 && w == "庚") r.B += "末伏 ";
            if (u.cur_mz > 0 && u.cur_mz <= 10 && w == "丙") r.B += "入梅 ";
            if (u.cur_xs > 0 && u.cur_xs <= 12 && w2 == "未") r.B += "出梅 ";
        }



        /// <summary>
        /// 命理八字计算, 并保存到日对象 ob 中
        /// </summary>
        /// <param name="jd">格林尼治UT(J2000起算)</param>
        /// <param name="J">本地经度</param>
        /// <param name="ob">日对象</param>
        public static void mingLiBaZi(double jd, double J, OB ob)
        {
            mingLiBaZi(jd, J, ob, BaZiTypeS.Normal);
        }


        /// <summary>
        /// 命理八字计算, 并保存到日对象 ob 中
        /// </summary>
        /// <param name="jd">格林尼治UT(J2000起算)</param>
        /// <param name="J">本地经度</param>
        /// <param name="ob">日对象</param>
        /// <param name="southernHemisphere">南半球的标志</param>
        public static void mingLiBaZi(double jd, double J, OB ob, BaZiTypeS baziTypeS)
        {
            int i;
            string c;
            double v;
            double jd2 = jd + JD.deltatT2(jd);      // 力学时
            double w = XL.S_aLon(jd2 / 36525, -1);  // 此刻太阳视黄经
            double k = LunarHelper.int2((w / LunarHelper.pi2 * 360 + 45 + 15 * 360) / 30);   // 1984年立春起算的节气数(不含中气)
            jd += XL.shiCha2(jd2 / 36525) - J / Math.PI / 2;        // 本地真太阳时(使用低精度算法计算时差)
            ob.bz_zty = JD.timeStr(jd);

            jd += 13d / 24d;   // 转为前一日23点起算(原jd为本日中午12点起算)   // C#: 注意数据类型
            double D = Math.Floor(jd), SC = LunarHelper.int2((jd - D) * 12);   // 日数与时辰

            v = LunarHelper.int2(k / 12 + 6000000); ob.bz_jn = obb.Gan[(int)(v % 10)] + obb.Zhi[(int)(v % 12)];
            v = k + 2 + 60000000; ob.bz_jy = obb.Gan[(int)(v % 10)] + obb.Zhi[(int)(v % 12)];

            // C#: 新增的代码段, 计算南半球八字(仅纪月不同)
            switch (baziTypeS)
            {
                case BaZiTypeS.TianChongDiChong:
                    ob.bz_jy = obb.Gan[(int)((v + 4) % 10)] + obb.Zhi[(int)((v + 6) % 12)];
                    break;

                case BaZiTypeS.TianKeDiChong:
                    ob.bz_jy = obb.Gan[(int)((v + 6) % 10)] + obb.Zhi[(int)((v + 6) % 12)];
                    break;

                case BaZiTypeS.TianTongDiChong:
                    ob.bz_jy = obb.Gan[(int)((v + 0) % 10)] + obb.Zhi[(int)((v + 6) % 12)];
                    break;

                default:
                    break;
            }


            v = D - 6 + 9000000; ob.bz_jr = obb.Gan[(int)(v % 10)] + obb.Zhi[(int)(v % 12)];
            v = (D - 1) * 12 + 90000000 + SC; ob.bz_js = obb.Gan[(int)(v % 10)] + obb.Zhi[(int)(v % 12)];

            v -= SC;
            ob.bz_JS = "";    // 全天纪时表
            for (i = 0; i < 13; i++)
            { 
                // 一天中包含有13个纪时
                c = obb.Gan[(int)((v + i) % 10)] + obb.Zhi[(int)((v + i) % 12)];  // 各时辰的八字
                if (SC == i)
                {
                    ob.bz_js = c;
                    // c = "<font color=red>" + c + "</font>";   //红色显示这时辰   // C#: 注释, 即取消格式显示
                } 
                ob.bz_JS += (i != 0 ? " " : "") + c;
            }
        }


        /// <summary>
        /// 精气计算
        /// </summary>
        /// <param name="W"></param>
        /// <returns></returns>
        public static double qi_accurate(double W)
        {
            double t = XL.S_aLon_t(W) * 36525;
            return t - JD.deltatT2(t) + 8d / 24d;    // 精气
        }


        /// <summary>
        /// 精朔计算
        /// </summary>
        /// <param name="W"></param>
        /// <returns></returns>
        public static double so_accurate(double W)
        {
            double t = XL.MS_aLon_t(W) * 36525;
            return t - JD.deltatT2(t) + 8d / 24d;    // 精朔
        }


        /// <summary>
        /// 精气计算法 2: 
        /// </summary>
        /// <param name="jd"></param>
        /// <returns></returns>
        public static double qi_accurate2(double jd)
        {
            return obb.qi_accurate(Math.Floor((jd + 293) / 365.2422 * 24) * Math.PI / 12);     //精气
        }


        /// <summary>
        /// 精朔计算法 2: 
        /// </summary>
        /// <param name="jd"></param>
        /// <returns></returns>
        public static double so_accurate2(double jd)
        {
            return obb.so_accurate(Math.Floor((jd + 8) / 29.5306) * Math.PI * 2);     // 精朔
        }

        #endregion



        #region 转换时新增的方法

        /// <summary>
        /// 命理八字计算(普通计算, 不转换为当地真太阳时), 并保存到日对象 ob 中
        /// </summary>
        /// <param name="jd">格林尼治UT(J2000起算)</param>
        /// <param name="J">本地经度</param>
        /// <param name="ob">日对象</param>
        public static void mingLiBaZiNormal(double jd, double J, OB ob)
        {
            mingLiBaZiNormal(jd, J, ob, BaZiTypeS.Normal);
        }


        /// <summary>
        /// 命理八字计算(普通计算, 不转换为当地真太阳时), 并保存到日对象 ob 中
        /// </summary>
        /// <param name="jd">格林尼治UT(J2000起算)</param>
        /// <param name="J">本地经度</param>
        /// <param name="ob">日对象</param>
        /// <param name="southernHemisphere">南半球的标志</param>
        public static void mingLiBaZiNormal(double jd, double J, OB ob, BaZiTypeS baziTypeS)
        {
            int i;
            string c;
            double v;
            double jd2 = jd + JD.deltatT2(jd);      // 力学时
            double w = XL.S_aLon(jd2 / 36525, -1);  // 此刻太阳视黄经
            double k = LunarHelper.int2((w / LunarHelper.pi2 * 360 + 45 + 15 * 360) / 30);   // 1984年立春起算的节气数(不含中气)

            //----------------------------------------------------------------------------------------------
            // C#: 注: 仅有下列代码段与 mingLiBaZi 方法中的代码不同, 其余部分都是相同的
            //----------------------------------------------------------------------------------------------
            jd += 0 - J / Math.PI / 2;     // 将格林尼治UT(J2000起算), 转换为本地时间, 不必考虑真太阳与平太阳时之间的时差
            ob.bz_zty = "";                // 真太阳时置空串
            ob.bz_pty = JD.timeStr(jd);    // 计算平太阳时

            jd += 13d / 24d;   // 转为前一日23点起算(原jd为本日中午12点起算)   // C#: 注意数据类型
            double D = Math.Floor(jd), SC = LunarHelper.int2((jd - D) * 12);   // 日数与时辰

            v = LunarHelper.int2(k / 12 + 6000000); ob.bz_jn = obb.Gan[(int)(v % 10)] + obb.Zhi[(int)(v % 12)];
            v = k + 2 + 60000000; ob.bz_jy = obb.Gan[(int)(v % 10)] + obb.Zhi[(int)(v % 12)];

            // C#: 新增的代码段, 计算南半球八字(仅纪月不同)
            switch (baziTypeS)
            {
                case BaZiTypeS.TianChongDiChong:
                    ob.bz_jy = obb.Gan[(int)((v + 4) % 10)] + obb.Zhi[(int)((v + 6) % 12)];
                    break;

                case BaZiTypeS.TianKeDiChong:
                    ob.bz_jy = obb.Gan[(int)((v + 6) % 10)] + obb.Zhi[(int)((v + 6) % 12)];
                    break;

                case BaZiTypeS.TianTongDiChong:
                    ob.bz_jy = obb.Gan[(int)((v + 0) % 10)] + obb.Zhi[(int)((v + 6) % 12)];
                    break;

                default:
                    break;
            }


            v = D - 6 + 9000000; ob.bz_jr = obb.Gan[(int)(v % 10)] + obb.Zhi[(int)(v % 12)];
            v = (D - 1) * 12 + 90000000 + SC; ob.bz_js = obb.Gan[(int)(v % 10)] + obb.Zhi[(int)(v % 12)];

            v -= SC;
            ob.bz_JS = "";    // 全天纪时表
            for (i = 0; i < 13; i++)
            {
                // 一天中包含有13个纪时
                c = obb.Gan[(int)((v + i) % 10)] + obb.Zhi[(int)((v + i) % 12)];  // 各时辰的八字
                if (SC == i)
                {
                    ob.bz_js = c;
                    // c = "<font color=red>" + c + "</font>";   //红色显示这时辰   // C#: 注释, 即取消格式显示
                }
                ob.bz_JS += (i != 0 ? " " : "") + c;
            }
        }


        /// <summary>
        /// 从 Xml 对象中读取农历节日的定义
        /// </summary>
        /// <returns></returns>
        private static xList<OB> getLunarFeasts()
        {
            const string xPath = "SharpSxwnl/SxwnlData/Data[@Id = 'obb_getDayName']";
            xList<OB> result = new xList<OB>();

            if (LunarHelper.SxwnlXmlData != null)
            {
                XmlNodeList foundNodeList = LunarHelper.SxwnlXmlData.SelectNodes(xPath);
                if (foundNodeList.Count > 0)
                {
                    for (int i = 0; i < foundNodeList.Count; i++)
                        for (int j = 0; j < foundNodeList[i].ChildNodes.Count; j++)
                        {
                            result.Add(new OB());    // 添加日对象来记录节点信息
                            XmlAttributeCollection xmlAttr = foundNodeList[i].ChildNodes[j].Attributes;
                            result[result.Count - 1].Lmc = xmlAttr.GetNamedItem("Day").InnerText;
                            result[result.Count - 1].A = xmlAttr.GetNamedItem("A").InnerText;
                            result[result.Count - 1].B = xmlAttr.GetNamedItem("B").InnerText;
                            result[result.Count - 1].C = xmlAttr.GetNamedItem("C").InnerText;
                            result[result.Count - 1].Fjia = LunarHelper.VAL(xmlAttr.GetNamedItem("Fjia").InnerText) == 0 ? 0 : 1;
                        }
                }

            }

            return result;
        }


        /// <summary>
        /// 从 Xml 对象中读取农历节日的定义
        /// </summary>
        /// <returns></returns>
        private static xList<string> getJieQiFeasts()
        {
            const string xPath = "SharpSxwnl/SxwnlData/Data[@Id = 'obb_JieqiFjia']";
            xList<string> result = new xList<string>();

            if (LunarHelper.SxwnlXmlData != null)
            {
                XmlNode foundNode;
                Regex regexToTrim = new Regex(@"\s*");    // C#: 匹配任何空白字符, 用于去除所有空白字符

                // 读取并解开历史纪年表
                foundNode = LunarHelper.SxwnlXmlData.SelectSingleNode(xPath);
                if (foundNode != null)
                {
                    string[] jieqiFeasts = regexToTrim.Replace(foundNode.InnerText, "").Split(',');
                    result.AddRange(jieqiFeasts);
                }

            }

            return result;
        }

        #endregion 转换时新增的方法


        
        #region 转换时增加的私有字段(用于封装成公共属性, 按转换规范 10 命名)

        // 数字 0 - 10 对应的中文名称
        private static string[] __numCn = new string[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" }; //中文数字

        // 十天干表
        private static string[] __Gan = new string[] { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };

        // 十二地支表
        private static string[] __Zhi = new string[] { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };

        // 十二属相表
        private static string[] __ShX = new string[] { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };

        // 十二星座表
        private static string[] __XiZ = new string[] { "摩羯", "水瓶", "双鱼", "白羊", "金牛", "双子", "巨蟹", "狮子", "处女", "天秤", "天蝎", "射手" };

        // 月相名称表
        private static string[] __yxmc = new string[] { "朔", "上弦", "望", "下弦" }; //月相名称表

        // 廿四节气表
        private static string[] __jqmc = new string[] { "冬至", "小寒", "大寒", "立春", "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏", "小满", "芒种", "夏至", "小暑", "大暑", "立秋", "处暑", "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪" };

        // 农历各月的名称, 从 "十一" 月开始, 即从月建 "子" 开始, 与十二地支的顺序对应
        private static string[] __ymc = new string[] { "十一", "十二", "正", "二", "三", "四", "五", "六", "七", "八", "九", "十" }; //月名称,建寅

        // 农历各日的名称
        private static string[] __rmc = new string[] { "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十", "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十", "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十", "卅一" };

        // 历史纪年表
        private static JnbArrayList __JNB = new JnbArrayList();

        // 廿四节气对应的月建表, 与 __jqmc 对应
        private static string[] __JieQiYueJian = new string[] { "子", "丑", "丑", "寅", "寅", "卯", "卯", "辰", "辰", "巳", "巳", "午", "午", "未", "未", "申", "申", "酉", "酉", "戌", "戌", "亥", "亥", "子" };

        // 日十二建表
        private static string[] __RiJian12 = new string[] { "建", "除", "满", "平", "定", "执", "破", "危", "成", "收", "开", "闭" };

        // 双重日十二建表
        private static string[] __DoubleRiJian12 = new string[] { "建", "除", "满", "平", "定", "执", "破", "危", "成", "收", "开", "闭",
                                                                  "建", "除", "满", "平", "定", "执", "破", "危", "成", "收", "开", "闭" };
        // 双重十二地支表
        private static string[] __DoubleZhi = new string[] { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥",
                                                             "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
        // 农历节日的定义
        private static xList<OB> __LunarFeasts = obb.getLunarFeasts();

        // 二十四节气假日的定义
        private static xList<string> __JieQiFeasts = obb.getJieQiFeasts();

        #endregion



        #region 转换时新增的公共属性

        /// <summary>
        /// 廿四节气对应的月建表, 与 jqmc 属性对应, 因此"气"的月建使用节的月建
        /// </summary>
        public static string[] JieQiYueJian
        {
            get => obb.__JieQiYueJian;
            set => obb.__JieQiYueJian = value;
        }

        /// <summary>
        /// 日十二建表
        /// </summary>
        public static string[] RiJian12
        {
            get => obb.__RiJian12;
            set => obb.__RiJian12 = value;
        }

        /// <summary>
        /// 双重日十二建表
        /// </summary>
        public static string[] DoubleRiJian12
        {
            get => obb.__DoubleRiJian12;
            set => obb.__DoubleRiJian12 = value;
        }

        /// <summary>
        /// 双重十二地支表
        /// </summary>
        public static string[] DoubleZhi
        {
            get => obb.__DoubleZhi;
            set => obb.__DoubleZhi = value;
        }

        /// <summary>
        /// 农历节日的定义
        /// </summary>
        public static xList<OB> LunarFeasts
        {
            get => obb.__LunarFeasts;
            set => obb.__LunarFeasts = value;
        }

        /// <summary>
        ///  二十四节气假日的定义
        /// </summary>
        public static xList<string> JieQiFeasts
        {
            get => obb.__JieQiFeasts;
            set => obb.__JieQiFeasts = value;
        }

        #endregion

    }
}
