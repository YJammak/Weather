using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SharpSxwnl
{
    /// <summary>
    /// 公历基础构件,JD物件的补充(主要用于计算节假日, 纪念日, 回历信息等)
    /// </summary>
    public static class oba
    {
        #region 公共属性(注: 初始转换时为公共字段, 已改写, 请参阅“转换时增加的私有字段”)

        /// <summary>
        /// 按周规则定义的世界性节日(纪念日)
        /// </summary>
        public static xList<string> wFtv
        {
            get => oba.__wFtv;
            set => oba.__wFtv = value;
        }
        
        /// <summary>
        /// 各月的节日(纪念日), 将原 Javascript 代码进行了适当的改写
        /// </summary>
        public static xList<xList<string>> sFtv
        {
            get => oba.__sFtv;
            set => oba.__sFtv = value;
        }

        #endregion




        #region 公共方法

        /// <summary>
        /// 计算某日节日的信息, 并保存信息到日对象中
        /// </summary>
        /// <param name="u">日对象</param>
        /// <param name="r">日对象</param>
        public static void getDayName(OB u, OB r)
        {
            string m0 = (u.m < 10 ? "0" : "") + u.m;
            string d0 = (u.d < 10 ? "0" : "") + u.d;
            int i;
            string s, s2, type;

            if (u.week == 0 || u.week == 6)
                r.Fjia = 1;   // 星期日或星期六放假

            // 按公历日期查找
            for (i = 0; i < oba.sFtv[u.m - 1].Length; i++)    // C#: 注: 属性 sFtv 在初始化时已经包含了 12 个元素, 以对应 12 个月
            {
                // 公历节日或纪念日,遍历本月节日表
                s = oba.sFtv[u.m - 1][i];
                if (s.Length < 3 || s.Substring(0, 2) != d0)    // C#: 新增了第 1 个判断条件
                    continue;
                s = s.Substring(2, s.Length - 2);
                type = s.Substring(0, 1);
                if (s.Length >= 6 && s.Substring(5, 1) == "-")
                {
                    // 有年限的节日
                    if (u.y < (int.Parse(s.Substring(1, 4))) || u.y > (int.Parse(s.Substring(6, 4)))) continue;
                    s = s.Substring(10, s.Length - 10);
                }
                else
                {
                    if (u.y < 1850) continue;
                    s = s.Substring(1, s.Length - 1);
                }
                if (type == "#") { r.A += s + " "; r.Fjia = 1; } //放假的节日
                if (type == "I") r.B += s + " "; //主要
                if (type == ".") r.C += s + " "; //其它
            }

            // 按周规则查找
            double w = u.weeki;
            if (u.week >= u.week0) w += 1;
            double w2 = w;
            if (u.weeki == u.weekN - 1) w2 = 5;
            string w__ = m0 + w + u.week;    // d日在本月的第几个星期某
            string w2__ = m0 + w2 + u.week;

            for (i = 0; i < oba.wFtv.Length; i++)
            {
                s = oba.wFtv[i];
                s2 = s.Substring(0, 4);
                if (s2 != w__ && s2 != w2__) continue;
                type = s.Substring(4, 1);
                s = s.Substring(5, s.Length - 5);
                if (type == "#") { r.A += s + " "; r.Fjia = 1; }
                if (type == "I") r.B += s + " ";
                if (type == ".") r.C += s + " ";
            }
        }



        /// <summary>
        /// 回历计算, 并保存信息到日对象中
        /// </summary>
        /// <param name="d0">2000.0起算儒略日,北京时12:00</param>
        /// <param name="r">日对象</param>
        public static void getHuiLi(double d0, OB r)
        {
            // 以下算法使用Excel测试得到,测试时主要关心年临界与月临界
            double z, y, m, d;
            d = d0 + 503105; z = LunarHelper.int2((d + 0.1) / 10631);   // 10631为一周期(30年)
            d -= z * 10631; y = LunarHelper.int2((d + 0.5) / 354.366);  // 加0.5的作用是保证闰年正确(一周中的闰年是第2,5,7,10,13,16,18,21,24,26,29年)
            d -= LunarHelper.int2(y * 354.366 + 0.5); m = LunarHelper.int2((d + 0.11) / 29.51);   // 分子加0.11,分每加0.01的作用是第354或355天的的月分保持为12月(m=11)
            d -= LunarHelper.int2(m * 29.51 + 0.5);
            r.Hyear = z * 30 + y + 1;
            r.Hmonth = m + 1;
            r.Hday = d + 1;
        }

        #endregion 公共方法




        #region 转换时增加的私有字段(用于封装成公共属性, 按转换规范 10 命名)

        /// <summary>
        /// 按周规则定义的世界性节日(纪念日)
        /// </summary>
        private static xList<string> __wFtv = oba.getwFtvData();

        /// <summary>
        /// 各月的节日(纪念日), 功能类似于交错数组
        /// </summary>
        private static xList<xList<string>> __sFtv = oba.getsFtvData();   // 国历节日,#表示放假日,I表示重要节日或纪念日

        #endregion



        #region 转换时新增的方法

        /// <summary>
        /// 从 Xml 文档对象加载 wFtv 数据
        /// </summary>
        /// <returns></returns>
        private static xList<string> getwFtvData()
        {
            xList<string> result = new xList<string>();

            //----------------------------------------------------------------------------------------
            // 加载 Xml 数据:  按周规则定义的节日(纪念日)
            // 注: 加载时自动去除各行 Xml 数据前、后端的所有空白字符, 但对数据内部的空白字符不作处理
            //----------------------------------------------------------------------------------------
            if (LunarHelper.SxwnlXmlData != null)
            {
                const string wFtvXPath = "SharpSxwnl/SxwnlData/Data[@Id = 'oba_wFtv']";
                XmlNode foundNode;
                Regex regexToTrim = new Regex(@"(^\s*)|(\s*$)");    // C#: 匹配任何空白字符

                // 读取并解开数据
                foundNode = LunarHelper.SxwnlXmlData.SelectSingleNode(wFtvXPath);
                if (foundNode != null)
                {
                    string[] wftv = regexToTrim.Replace(foundNode.InnerText, "").Split(new char[]{ ',' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < wftv.Length; i++)
                        result.Add(regexToTrim.Replace(wftv[i], ""));
                }
            }

            return result;
        }



        /// <summary>
        /// 从 Xml 文档对象加载 sFtv 数据
        /// </summary>
        /// <returns></returns>
        private static xList<xList<string>> getsFtvData()
        {
            const int monthNumPerYear = 12;
            xList<xList<string>> result = new xList<xList<string>>();

            for (int i = 0; i < monthNumPerYear; i++)    // C#: 预置 12 个元素
                result.Add(new xList<string>());

            //----------------------------------------------------------------------------------------
            // 加载 Xml 数据:  按周规则定义的节日(纪念日)
            // 注: 加载时自动去除各行 Xml 数据前、后端的所有空白字符, 但对数据内部的空白字符不作处理
            //----------------------------------------------------------------------------------------
            if (LunarHelper.SxwnlXmlData != null)
            {
                XmlNode foundNode;
                Regex regexToTrim = new Regex(@"(^\s*)|(\s*$)");    // C#: 匹配前、后端的任何空白字符

                for (int i = 0; i < monthNumPerYear; i++)
                {
                    string xPath = "SharpSxwnl/SxwnlData/Data[@Id = 'oba_sFtv']/Month[@Id = '" + (i + 1).ToString() + "']";

                    foundNode = LunarHelper.SxwnlXmlData.SelectSingleNode(xPath);
                    if (foundNode != null)
                    {
                        string[] sftv = regexToTrim.Replace(foundNode.InnerText, "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < sftv.Length; j++)
                            result[i].Add(regexToTrim.Replace(sftv[j], ""));
                    }

                }
            }

            return result;
        }

        #endregion
    }
}
