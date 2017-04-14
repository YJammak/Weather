using System;
using System.Collections.Generic;
using System.Text;

namespace SharpSxwnl
{
    /// <summary>
    /// 日对象
    /// </summary>
    public class OB
    {
        #region 构造函数
        
        public OB()
        {
            this.ThisJieQi = new JieQiInfo();
            this.PreviousJieQi = new JieQiInfo();
            this.NextJieQi = new JieQiInfo();
        }

        #endregion 构造函数


        
        #region 公共属性(注: 初始转换时为公共字段, 已改写)

        #region 日的公历信息

        /// <summary>
        /// 2000.0起算儒略日,北京时12:00
        /// </summary>
        public double d0 { get; set; }    // = 0;    // 2000.0起算儒略日,北京时12:00

        /// <summary>
        /// 所在公历月内日序数
        /// </summary>
        public double di { get; set; }    // = 0;    // 所在公历月内日序数

        /// <summary>
        /// 所在公历年,同lun.y
        /// </summary>
        public int y { get; set; }    // = 0;    // 所在公历年,同lun.y

        /// <summary>
        /// 所在公历月,同lun.m
        /// </summary>
        public int m { get; set; }    // = 0;    // 所在公历月,同lun.m

        /// <summary>
        /// 日名称(公历)
        /// </summary>
        public int d { get; set; }    // = 0;    // 日名称(公历)

        /// <summary>
        /// 所在公历月的总天数,同lun.d0
        /// </summary>
        public double dn { get; set; }    // = 0;    // 所在公历月的总天数,同lun.d0

        /// <summary>
        /// 所在月的月首的星期,同lun.w0
        /// </summary>
        public double week0 { get; set; }    // = 0;    // 所在月的月首的星期,同lun.w0

        /// <summary>
        /// 星期
        /// </summary>
        public double week { get; set; }    // = 0;     // 星期

        /// <summary>
        /// 在本月中的周序号
        /// </summary>
        public double weeki { get; set; }    // = 0;    // 在本月中的周序号

        /// <summary>
        /// 本月的总周数
        /// </summary>
        public double weekN { get; set; }    // = 0;    // 本月的总周数

        #endregion



        #region 日的农历信息

        /// <summary>
        /// 距农历月首的编移量,0对应初一
        /// </summary>
        public double Ldi { get; set; }    // = 0;    // 距农历月首的编移量,0对应初一

        /// <summary>
        /// 日名称(农历),即'初一,初二等'
        /// </summary>
        public string Ldc { get; set; }    // = "";   // 日名称(农历),即'初一,初二等'

        /// <summary>
        /// 距冬至的天数
        /// </summary>
        public double cur_dz { get; set; }    // = 0;    // 距冬至的天数

        /// <summary>
        /// 距夏至的天数
        /// </summary>
        public double cur_xz { get; set; }    // = 0;    // 距夏至的天数

        /// <summary>
        /// 距立秋的天数
        /// </summary>
        public double cur_lq { get; set; }    // = 0;    // 距立秋的天数

        /// <summary>
        /// 距芒种的天数
        /// </summary>
        public double cur_mz { get; set; }    // = 0;    // 距芒种的天数

        /// <summary>
        /// 距小暑的天数
        /// </summary>
        public double cur_xs { get; set; }    // = 0;    // 距小暑的天数

        /// <summary>
        /// 月名称
        /// </summary>
        public string Lmc { get; set; }    // = "";   // 月名称

        /// <summary>
        /// 月大小
        /// </summary>
        public double Ldn { get; set; }    // = 0;    // 月大小

        /// <summary>
        /// 月大小
        /// </summary>
        public string Ldns => (Ldn == 30 ? "大" : "小");

        /// <summary>
        /// 月信息
        /// </summary>
        public string LMouthInfo => $"{Lleap}{Lmc}{Ldns}{Ldc}";

        /// <summary>
        /// 所在月信息
        /// </summary>
        public Lunar Lunar { get; set; }

        /// <summary>
        /// 闰状况(值为'闰'或空串)
        /// </summary>
        public string Lleap { get; set; }    // = "";   // 闰状况(值为'闰'或空串)

        /// <summary>
        /// 下个月名称,判断除夕时要用到
        /// </summary>
        public string Lmc2 { get; set; }    // = "";    // 下个月名称,判断除夕时要用到

        #region 日的农历纪年、月、日、时及星座

        /// <summary>
        /// 农历纪年(10进制,1984年起算,分界点可以是立春也可以是春节,在程序中选择一个)
        /// </summary>
        public double Lyear { get; set; }    // = 0;    // 农历纪年(10进制,1984年起算,分界点可以是立春也可以是春节,在程序中选择一个)

        /// <summary>
        /// 干支纪年(立春)
        /// </summary>
        public string Lyear2 { get; set; }    // = "";   // 干支纪年(立春)

        /// <summary>
        /// 生肖(立春)
        /// </summary>
        public string LShX1 { get; set; }

        /// <summary>
        /// 生肖(正月)
        /// </summary>
        public string LShX2 { get; set; }

        /// <summary>
        /// 纪月处理,1998年12月7日(大雪)开始连续进行节气计数,0为甲子
        /// </summary>
        public double Lmonth { get; set; }    // = 0;    // 纪月处理,1998年12月7日(大雪)开始连续进行节气计数,0为甲子

        /// <summary>
        /// 干支纪月
        /// </summary>
        public string Lmonth2 { get; set; }    // = "";    // 干支纪月

        /// <summary>
        /// 干支纪日
        /// </summary>
        public string Lday2 { get; set; }    // = "";    // 纪日

        /// <summary>
        /// 干支纪时
        /// </summary>
        public double Ltime2 { get; set; }    // = 0;    // 纪时

        /// <summary>
        /// 星座
        /// </summary>
        public string XiZ { get; set; }    // = "";    // 星座

        #endregion
        #endregion 日的农历信息



        #region 日的回历信息
        
        /// <summary>
        /// 年(回历)
        /// </summary>
        public double Hyear { get; set; }    // = 0;    // 年(回历)

        /// <summary>
        /// 月(回历)
        /// </summary>
        public double Hmonth { get; set; }    // = 0;    // 月(回历)

        /// <summary>
        /// 日(回历)
        /// </summary>
        public double Hday { get; set; }    // = 0;    // 日(回历)
        
        #endregion



        #region 日的其它信息

        /// <summary>
        /// 月相名称
        /// </summary>
        public string yxmc { get; set; }    // = "";    // 月相名称

        /// <summary>
        /// 月相时刻(儒略日)
        /// </summary>
        public string yxjd { get; set; }    // = "";     // 月相时刻(儒略日)

        /// <summary>
        /// 月相时间串
        /// </summary>
        public string yxsj { get; set; }    // = "";    // 月相时间串

        /// <summary>
        /// 节气名称
        /// </summary>
        public string jqmc { get; set; }    // = "";    // 节气名称

        /// <summary>
        /// 节气时刻(儒略日)
        /// </summary>
        public string jqjd { get; set; }    // = "";     // 节气时刻(儒略日)

        /// <summary>
        /// 节气时间串
        /// </summary>
        public string jqsj { get; set; }    // = "";    // 节气时间串

        #endregion



        #region C#: 从 Javascript 代码中提取出来的其他字段(属性)

        /// <summary>
        /// 农历纪年(10进制,1984年起算)
        /// </summary>
        public double Lyear0 { get; set; }    // = 0;     // 农历纪年(10进制,1984年起算)

        /// <summary>
        /// 干支纪年(正月)
        /// </summary>
        public string Lyear3 { get; set; }    // = "";    // 干支纪年(正月)

        /// <summary>
        /// 黄帝纪年
        /// </summary>
        public double Lyear4 { get; set; }    // = 0;      // 黄帝纪年

        /// <summary>
        /// A 类节日纪念日(重要喜庆日子名称(可将日子名称置红))
        /// </summary>
        public string A { get; set; }    // = "";

        /// <summary>
        /// B 类节日纪念日(重要日子名称)
        /// </summary>
        public string B { get; set; }    // = "";

        /// <summary>
        /// C 类节日纪念日(各种日子名称(连成一大串, 以空格符分隔))
        /// </summary>
        public string C { get; set; }    // = "";

        /// <summary>
        /// 放假的标志
        /// </summary>
        public double Fjia { get; set; }    // = 0;

        /// <summary>
        /// 八字信息: 真太阳时间
        /// </summary>
        public string bz_zty { get; set; }    // = "";

        /// <summary>
        /// 八字信息: 干支纪年
        /// </summary>
        public string bz_jn { get; set; }    // = "";

        /// <summary>
        /// 八字信息: 干支纪月
        /// </summary>
        public string bz_jy { get; set; }    // = "";

        /// <summary>
        /// 八字信息: 干支纪日
        /// </summary>
        public string bz_jr { get; set; }    // = "";

        /// <summary>
        /// 八字信息: 干支纪时
        /// </summary>
        public string bz_js { get; set; }    // = "";

        /// <summary>
        /// 八字信息: 干支纪时(从子时至亥时共 12 个时辰的干支集合, 以空格分隔
        /// </summary>
        public string bz_JS { get; set; }    // = "";

        /// <summary>
        /// 节气名称(实历?)
        /// </summary>
        public string Ljq { get; set; }    // = "";

        #endregion


        #endregion 公共属性


        
        #region 转换时新增的公共属性

        /// <summary>
        /// 八字信息: 平太阳时间
        /// </summary>
        public string bz_pty { get; set; }    // = "";

        /// <summary>
        /// 所属节令
        /// </summary>
        public JieQiInfo ThisJieQi { get; set; }

        /// <summary>
        /// 上一节令
        /// </summary>
        public JieQiInfo PreviousJieQi { get; set; }

        /// <summary>
        /// 下一节令
        /// </summary>
        public JieQiInfo NextJieQi { get; set; }

        /// <summary>
        /// 每日的十二建信息, 即: {建, 除, 满, 平, 定, 执, 破, 危, 成, 收, 开, 闭} 其中之一
        /// </summary>
        public string Ri12Jian { get; set; }

        #endregion

    }
}
