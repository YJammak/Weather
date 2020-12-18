using System;
using System.Collections.Generic;
using System.Text;

namespace SharpSxwnl
{
    /// <summary>
    /// 日月的升中天降,不考虑气温和气压的影响
    /// </summary>
    public static class SZJ
    {
        #region 私有字段(注: 初始转换时为公共字段, 已改写)

        /// <summary>
        /// 站点地理经度,向东测量为负
        /// </summary>
        private static double L = 0;             //站点地理经度,向东测量为负

        /// <summary>
        /// 站点地理纬度
        /// </summary>
        private static double fa = 0;            //站点地理纬度

        /// <summary>
        /// TD-UT
        /// </summary>
        private static double dt = 0;            //TD-UT

        /// <summary>
        /// 黄赤交角
        /// </summary>
        private static double E = 0.409092614;   //黄赤交角

        #endregion



        #region 公共属性(注: 初始转换时为公共字段, 已改写)

        /// <summary>
        /// 多天的升中降
        /// 该字段为嵌套的 LunarInfoListT 实例, 与交错数组 double[][] 的功能类似, 但它的特别之处在于拥有自定义的字段(属性)
        /// </summary>
        //------------------------------------------------------------------------------------------------------------------------------
        // C#: 改写时要注意的是该属性应该先初始化(见本类的 calcRTS 方法), 初始转换时的语句如下:
        //public static LunarInfoListT<LunarInfoListT<double>> rts = new LunarInfoListT<LunarInfoListT<double>>();    // 多天的升中降
        //------------------------------------------------------------------------------------------------------------------------------
        public static LunarInfoListT<LunarInfoListT<double>> rts { get; set; }    // 多天的升中降

        #endregion



        #region 公共方法

        /// <summary>
        /// h地平纬度,w赤纬,返回时角
        /// </summary>
        /// <param name="h"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static double getH(double h, double w)
        {
            double c = (Math.Sin(h) - Math.Sin(SZJ.fa) * Math.Sin(w)) / Math.Cos(SZJ.fa) / Math.Cos(w);
            if (Math.Abs(c) > 1) return Math.PI;
            return Math.Acos(c);
        }


        /// <summary>
        /// 章动同时影响恒星时和天体坐标,所以不计算章动。返回时角及赤经纬
        /// </summary>
        /// <param name="jd"></param>
        /// <param name="H0"></param>
        /// <param name="z"></param>
        public static void Mcoord(double jd, double H0, LunarInfoListT<double> z)
        {
            XL.M_coord((jd + SZJ.dt) / 36525, z, 30, 20, 8); //低精度月亮赤经纬
            ZB.llrConv(z, SZJ.E); //转为赤道坐标
            z.H = LunarHelper.rad2mrad(ZB.gst(jd, SZJ.dt) - SZJ.L - z[0]);
            if (z.H > Math.PI) z.H -= LunarHelper.pi2; //得到此刻天体时角
            if (H0 != 0) z.H0 = SZJ.getH(0.7275 * LunarHelper.cs_rEar / z[2] - 34 * 60 / LunarHelper.rad, z[1]); //升起对应的时角
        }


        /// <summary>
        /// 月亮到中升降时刻计算,传入jd含义与St()函数相同
        /// </summary>
        /// <param name="jd"></param>
        /// <returns></returns>
        public static LunarInfoListT<double> Mt(double jd)
        {
            SZJ.dt = JD.deltatT2(jd);
            SZJ.E = ZB.hcjj(jd / 36525);
            jd -= LunarHelper.mod2(0.1726222 + 0.966136808032357 * jd - 0.0366 * SZJ.dt - SZJ.L / LunarHelper.pi2, 1); //查找最靠近当日中午的月上中天,mod2的第1参数为本地时角近似值

            LunarInfoListT<double> r = new LunarInfoListT<double>(3, 0d);
            double sv = LunarHelper.pi2 * 0.966;
            r.z__ = r.s__ = r.j__ = r.c__ = r.h__ = jd;
            SZJ.Mcoord(jd, 1, r); //月亮坐标
            r.s__ += (-r.H0 - r.H) / sv;
            r.j__ += (r.H0 - r.H) / sv;
            r.z__ += (0 - r.H) / sv;
            SZJ.Mcoord(r.s__, 1, r); r.s__ += (-r.H0 - r.H) / sv;
            SZJ.Mcoord(r.j__, 1, r); r.j__ += (+r.H0 - r.H) / sv;
            SZJ.Mcoord(r.z__, 0, r); r.z__ += (0 - r.H) / sv;
            return r;
        }


        /// <summary>
        /// 章动同时影响恒星时和天体坐标,所以不计算章动。返回时角及赤经纬
        /// </summary>
        /// <param name="jd"></param>
        /// <param name="H0"></param>
        /// <param name="H1"></param>
        /// <param name="z"></param>
        public static void Scoord(double jd, double H0, double H1, LunarInfoListT<double> z)
        {
            z[0] = XL.E_Lon((jd + SZJ.dt) / 36525, 5) + Math.PI - 20.5 / LunarHelper.rad;  //太阳坐标(修正了光行差)
            z[1] = 0d; z[2] = 1d;       // C#: 添加 d, 强制为 double 类型, 否则在把该元素显式转换为 double 时会出错
            ZB.llrConv(z, SZJ.E);       // 转为赤道坐标
            z.H = LunarHelper.rad2rrad(ZB.gst(jd, SZJ.dt) - SZJ.L - z[0]); //得到此刻天体时角
            if (H0 != 0) z.H0 = SZJ.getH(-50 * 60 / LunarHelper.rad, z[1]);  //地平以下50分
            if (H1 != 0) z.H1 = SZJ.getH(-Math.PI / 30, z[1]);  // 地平以下6度
        }


        /// <summary>
        /// 太阳到中升降时刻计算,传入jd是当地中午12点时间对应的2000年首起算的格林尼治时间UT
        /// </summary>
        /// <param name="jd"></param>
        /// <returns></returns>
        public static LunarInfoListT<double> St(double jd)
        {
            SZJ.dt = JD.deltatT2(jd);
            SZJ.E = ZB.hcjj(jd / 36525);
            jd -= LunarHelper.mod2(jd - SZJ.L / LunarHelper.pi2, 1); //查找最靠近当日中午的日上中天,mod2的第1参数为本地时角近似值

            LunarInfoListT<double> r = new LunarInfoListT<double>(3, 0d);
            double sv = LunarHelper.pi2;
            r.z__ = r.s__ = r.j__ = r.c__ = r.h__ = jd;
            SZJ.Scoord(jd, 1, 1, r);       //太阳坐标
            r.s__ += (-r.H0 - r.H) / sv;   //升起
            r.j__ += (r.H0 - r.H) / sv;    //降落
            r.c__ += (-r.H1 - r.H) / sv;   //民用晨
            r.h__ += (r.H1 - r.H) / sv;    //民用昏
            r.z__ += (0 - r.H) / sv;       //中天
            SZJ.Scoord(r.s__, 1, 0, r); r.s__ += (-r.H0 - r.H) / sv;
            SZJ.Scoord(r.j__, 1, 0, r); r.j__ += (+r.H0 - r.H) / sv;
            SZJ.Scoord(r.c__, 0, 1, r); r.c__ += (-r.H1 - r.H) / sv;
            SZJ.Scoord(r.h__, 0, 1, r); r.h__ += (+r.H1 - r.H) / sv;
            SZJ.Scoord(r.z__, 0, 0, r); r.z__ += (0 - r.H) / sv;
            return r;
        }



        /// <summary>
        /// 多天升中降计算,jd是当地起始儒略日(中午时刻),sq是时区
        /// </summary>
        /// <param name="jd"></param>
        /// <param name="n"></param>
        /// <param name="Jdl"></param>
        /// <param name="Wdl"></param>
        /// <param name="sq"></param>
        public static void calcRTS(double jd, double n, double Jdl, double Wdl, double sq)
        {
            int i;
            double c;
            LunarInfoListT<double> r;     // C#: 不需要实例化, 因此注释语句的后半部分:  = new LunarInfoListT<double>(3, 0d);

            // C#: 由于将 rts 设置为自动实现的公共属性, 故添加了以下代码段来初始化
            if (SZJ.rts == null)
                SZJ.rts = new LunarInfoListT<LunarInfoListT<double>>();

            if (SZJ.rts.Count == 0)
            {
                for (i = 0; i < 31; i++)
                {
                    SZJ.rts.Add(new LunarInfoListT<double>());
                    // SZJ.rts[i] = new LunarInfoListT();
                }
            }
            SZJ.L = Jdl; SZJ.fa = Wdl; sq /= 24; //设置站点参数
            for (i = 0; i < n; i++) { r = SZJ.rts[i]; r.Ms = r.Mz = r.Mj = ""; }
            for (i = -1; i <= n; i++)
            {
                if (i >= 0 && i < n)
                { 
                    //太阳
                    r = SZJ.St(jd + i + sq);
                    ((SZJ.rts[i])).s = JD.timeStr(r.s__ - sq);    //升
                    ((SZJ.rts[i])).z = JD.timeStr(r.z__ - sq);    //中
                    ((SZJ.rts[i])).j = JD.timeStr(r.j__ - sq);    //降
                    ((SZJ.rts[i])).c = JD.timeStr(r.c__ - sq);    //晨
                    ((SZJ.rts[i])).h = JD.timeStr(r.h__ - sq);    //昏
                    ((SZJ.rts[i])).ch = JD.timeStr(r.h__ - r.c__ - 0.5);   //光照时间,timeStr()内部+0.5,所以这里补上-0.5
                    ((SZJ.rts[i])).sj = JD.timeStr(r.j__ - r.s__ - 0.5);   //昼长
                }
                r = SZJ.Mt(jd + i + sq);     //月亮
                c = LunarHelper.int2(r.s__ - sq + 0.5) - jd; if (c >= 0 && c < n) (SZJ.rts[(int)c]).Ms = JD.timeStr(r.s__ - sq);
                c = LunarHelper.int2(r.z__ - sq + 0.5) - jd; if (c >= 0 && c < n) (SZJ.rts[(int)c]).Mz = JD.timeStr(r.z__ - sq);
                c = LunarHelper.int2(r.j__ - sq + 0.5) - jd; if (c >= 0 && c < n) (SZJ.rts[(int)c]).Mj = JD.timeStr(r.j__ - sq);
            }
            SZJ.rts.dn = n;
        }

        #endregion

    }
}
