using System;
using System.Collections.Generic;
using System.Text;

namespace SharpSxwnl
{
    /// <summary>
    /// 太阳月亮计算类
    /// </summary>
    public class sun_moon
    {
        #region 构造函数

        public sun_moon()
        {
        }

        #endregion



        #region 公共属性(注: 初始转换时为公共字段, 已改写)

        #region 基本参数

        /// <summary>
        /// 力学时
        /// </summary>
        public double T { get; set; }    // = 0;

        /// <summary>
        /// 站点经度
        /// </summary>
        public double L { get; set; }    // = 0;

        /// <summary>
        /// 站点纬度
        /// </summary>
        public double fa { get; set; }    // = 0;

        /// <summary>
        /// TD-UT
        /// </summary>
        public double dt { get; set; }    // = 0;

        /// <summary>
        /// UT
        /// </summary>
        public double jd { get; set; }    // = 0;

        /// <summary>
        /// 黄经章
        /// </summary>
        public double dL { get; set; }    // = 0;

        /// <summary>
        /// 交角章动
        /// </summary>
        public double dE { get; set; }    // = 0;

        /// <summary>
        /// 真黄赤交角
        /// </summary>
        public double E { get; set; }    // = 0;

        /// <summary>
        /// 真恒星时(不考虑非多项式部分)
        /// </summary>
        public double gst { get; set; }    // = 0;

        #endregion 基本参数



        #region 月球参数

        /// <summary>
        /// 月球视黄经
        /// </summary>
        public double mHJ { get; set; }    // = 0;

        /// <summary>
        /// 月球视黄纬
        /// </summary>
        public double mHW { get; set; }    // = 0;

        /// <summary>
        /// 地月质心距
        /// </summary>
        public double mR { get; set; }    // = 0;

        /// <summary>
        /// 月球视赤经
        /// </summary>
        public double mCJ { get; set; }    // = 0;

        /// <summary>
        /// 月球赤纬
        /// </summary>
        public double mCW { get; set; }    // = 0;

        /// <summary>
        /// 此时的天体时角
        /// </summary>
        public double mShiJ { get; set; }    // = 0;

        /// <summary>
        /// 修正了视差的赤道坐标: 月球视赤经
        /// </summary>
        public double mCJ2 { get; set; }    // = 0;

        /// <summary>
        /// 修正了视差的赤道坐标: 月球赤纬
        /// </summary>
        public double mCW2 { get; set; }    // = 0;

        /// <summary>
        /// 修正了视差的赤道坐标: 地月质心距
        /// </summary>
        public double mR2 { get; set; }    // = 0;

        /// <summary>
        /// 方位角
        /// </summary>
        public double mDJ { get; set; }    // = 0;

        /// <summary>
        /// 高度角
        /// </summary>
        public double mDW { get; set; }    // = 0;

        /// <summary>
        /// 方位角(大气折射修正后)
        /// </summary>
        public double mPJ { get; set; }    // = 0;

        /// <summary>
        /// 高度角(大气折射修正后)
        /// </summary>
        public double mPW { get; set; }    // = 0;

        #endregion



        #region 太阳参数

        /// <summary>
        /// 太阳视黄经
        /// </summary>
        public double sHJ { get; set; }    // = 0;

        /// <summary>
        /// 太阳视黄纬
        /// </summary>
        public double sHW { get; set; }    // = 0;

        /// <summary>
        /// 日地质心距
        /// </summary>
        public double sR { get; set; }    // = 0;

        /// <summary>
        /// 太阳视赤经
        /// </summary>
        public double sCJ { get; set; }    // = 0;

        /// <summary>
        /// 太阳视赤纬
        /// </summary>
        public double sCW { get; set; }    // = 0;

        /// <summary>
        /// 太阳时角
        /// </summary>
        public double sShiJ { get; set; }    // = 0;

        /// <summary>
        /// 修正了视差的赤道坐标: 太阳视赤经
        /// </summary>
        public double sCJ2 { get; set; }    // = 0;

        /// <summary>
        /// 修正了视差的赤道坐标: 太阳视赤纬
        /// </summary>
        public double sCW2 { get; set; }    // = 0;

        /// <summary>
        /// 修正了视差的赤道坐标: 日地质心距
        /// </summary>
        public double sR2 { get; set; }    // = 0;

        /// <summary>
        /// 方位角
        /// </summary>
        public double sDJ { get; set; }    // = 0;

        /// <summary>
        /// 高度角
        /// </summary>
        public double sDW { get; set; }    // = 0;

        /// <summary>
        /// 方位角(大气折射修正后)
        /// </summary>
        public double sPJ { get; set; }    // = 0;

        /// <summary>
        /// 高度角(大气折射修正后)
        /// </summary>
        public double sPW { get; set; }    // = 0;

        #endregion



        #region 其他参数

        /// <summary>
        /// 时差(单位:日)
        /// </summary>
        public double sc { get; set; }    // = 0;

        /// <summary>
        /// 平太阳时
        /// </summary>
        public double pty { get; set; }    // = 0;

        /// <summary>
        /// 真太阳时
        /// </summary>
        public double zty { get; set; }    // = 0;

        /// <summary>
        /// 月亮视半径(角秒)
        /// </summary>
        public double mRad { get; set; }    // = 0;

        /// <summary>
        /// 太阳视半径(角秒)
        /// </summary>
        public double sRad { get; set; }    // = 0;

        /// <summary>
        /// 月亮地心视半径(角秒)
        /// </summary>
        public double e_mRad { get; set; }    // = 0;

        /// <summary>
        /// 地本影在月球向径处的半径(角秒)
        /// </summary>
        public double eShadow { get; set; }    // = 0;

        /// <summary>
        /// 地本影在月球向径处的半径(角秒)
        /// </summary>
        public double eShadow2 { get; set; }    // = 0;

        /// <summary>
        /// 月亮被照面比例
        /// </summary>
        public double mIll { get; set; }    // = 0;

        /// <summary>
        /// 中心食计算: 经
        /// </summary>
        public double zx_J { get; set; }    // = 0;

        /// <summary>
        /// 中心食计算: 纬
        /// </summary>
        public double zx_W { get; set; }    // = 0;

        #endregion

        #endregion 公共属性




        #region 公共方法
        /// <summary>
        /// 计算 sun_moon 类的成员。参数：T是力学时,站点经度L,纬度fa,海拔high(千米)
        /// </summary>
        /// <param name="T">力学时</param>
        /// <param name="L">站点经度</param>
        /// <param name="fa">站点纬度</param>
        /// <param name="high">海拔</param>
        public void calc(double T, double L, double fa, double high)
        {
            //基本参数计算
            this.T = T; 
            this.L = L; this.fa = fa;
            this.dt = JD.deltatT2(T); //TD-UT
            this.jd = T - this.dt;    //UT
            T /= 36525; ZB.nutation(T);
            this.dL = ZB.dL;   //黄经章
            this.dE = ZB.dE;   //交角章动
            this.E = ZB.hcjj(T) + this.dE; //真黄赤交角
            this.gst = ZB.gst(this.jd, this.dt) + this.dL * Math.Cos(this.E); //真恒星时(不考虑非多项式部分)
            double[] z = new double[4];

            //=======月亮========
            //月亮黄道坐标
            XL.M_coord(T, z, -1, -1, -1); //月球坐标
            z[0] = LunarHelper.rad2mrad(z[0] + ZB.gxc_moonLon(T) + this.dL); z[1] += ZB.gxc_moonLat(T);  //补上月球光行差及章动
            this.mHJ = z[0]; this.mHW = z[1]; this.mR = z[2]; //月球视黄经,视黄纬,地月质心距

            //月球赤道坐标
            ZB.llrConv(z, this.E); //转为赤道坐标
            this.mCJ = z[0]; this.mCW = z[1]; //月球视赤经,月球赤纬

            //月亮时角计算
            this.mShiJ = LunarHelper.rad2mrad(this.gst - L - z[0]); //得到此刻天体时角
            if (this.mShiJ > Math.PI) this.mShiJ -= LunarHelper.pi2;

            //修正了视差的赤道坐标
            ZB.parallax(z, this.mShiJ, fa, high); //视差修正
            this.mCJ2 = z[0]; this.mCW2 = z[1]; this.mR2 = z[2];

            //月亮时角坐标
            z[0] += Math.PI / 2d - this.gst + L;  //转到相对于地平赤道分点的赤道坐标(时角坐标)

            //月亮地平坐标
            ZB.llrConv(z, Math.PI / 2 - fa);    //转到地平坐标(只改经纬度)
            z[0] = LunarHelper.rad2mrad(Math.PI / 2 - z[0]);
            this.mDJ = z[0]; this.mDW = z[1]; //方位角,高度角
            if (z[1] > 0) z[1] += ZB.AR2(z[1]); //大气折射修正
            this.mPJ = z[0]; this.mPW = z[1]; //方位角,高度角

            //=======太阳========
            //太阳黄道坐标
            XL.E_coord(T, z, -1, -1, -1);   //地球坐标
            z[0] = LunarHelper.rad2mrad(z[0] + Math.PI + ZB.gxc_sunLon(T) + this.dL);  //补上太阳光行差及章动
            z[1] = -z[1] + ZB.gxc_sunLat(T); //z数组为太阳地心黄道视坐标
            this.sHJ = z[0]; this.sHW = z[1]; this.sR = z[2]; //太阳视黄经,视黄纬,日地质心距

            //太阳赤道坐标
            ZB.llrConv(z, this.E); //转为赤道坐标
            this.sCJ = z[0]; this.sCW = z[1]; //太阳视赤经,视赤纬

            //太阳时角计算
            this.sShiJ = LunarHelper.rad2mrad(this.gst - L - z[0]); //得到此刻天体时角
            if (this.sShiJ > Math.PI) this.sShiJ -= LunarHelper.pi2;

            //修正了视差的赤道坐标
            ZB.parallax(z, this.sShiJ, fa, high); //视差修正
            this.sCJ2 = z[0]; this.sCW2 = z[1]; this.sR2 = z[2];

            //太阳时角坐标
            z[0] += Math.PI / 2 - this.gst + L;  //转到相对于地平赤道分点的赤道坐标

            //太阳地平坐标
            ZB.llrConv(z, Math.PI / 2 - fa);
            z[0] = LunarHelper.rad2mrad(Math.PI / 2 - z[0]);
            //z[1] -= 8.794/rad/z[2]*Math.cos(z[1]); //直接在地平坐标中视差修正(这里把地球看为球形,精度比ZB.parallax()稍差一些)
            this.sDJ = z[0]; this.sDW = z[1]; //方位角,高度角

            if (z[1] > 0) z[1] += ZB.AR2(z[1]); //大气折射修正
            this.sPJ = z[0]; this.sPW = z[1]; //方位角,高度角

            //=======其它========
            //时差计算
            double t = T / 10; double t2 = t * t, t3 = t2 * t, t4 = t3 * t, t5 = t4 * t;
            double Lon = (1753469512 + 6283319653318 * t + 529674 * t2 + 432 * t3 - 1124 * t4 - 9 * t5 + 630 * Math.Cos(6 + 3 * t)) / 1000000000 + Math.PI - 20.5 / LunarHelper.rad; //修正了光行差的太阳平黄经
            Lon = LunarHelper.rad2mrad(Lon - (this.sCJ - this.dL * Math.Cos(this.E))); //(修正了光行差的平黄经)-(不含dL*cos(E)的视赤经)
            if (Lon > Math.PI) Lon -= LunarHelper.pi2; //得到时差,单位是弧度
            this.sc = Lon / LunarHelper.pi2;   //时差(单位:日)

            //真太阳与平太阳
            this.pty = this.jd - L / LunarHelper.pi2;  //平太阳时
            this.zty = this.jd - L / LunarHelper.pi2 + this.sc; //真太阳时

            //视半径
            //this.mRad = XL.moonRad(this.mR,this.mDW);  //月亮视半径(角秒)
            this.mRad = 358473400d / this.mR2; //月亮视半径(角秒)
            this.sRad = 959.63 / this.sR2; //太阳视半径(角秒)
            this.e_mRad = 358473400d / this.mR; //月亮地心视半径(角秒)
            this.eShadow = (LunarHelper.cs_rEarA / this.mR * LunarHelper.rad - (959.63 - 8.794) / this.sR) * 51 / 50; //地本影在月球向径处的半径(角秒),式中51/50是大气厚度补偿
            this.eShadow2 = (LunarHelper.cs_rEarA / this.mR * LunarHelper.rad + (959.63 + 8.794) / this.sR) * 51 / 50; //地半影在月球向径处的半径(角秒),式中51/50是大气厚度补偿
            this.mIll = XL.moonIll(T); //月亮被照面比例

            //中心食计算
            if (Math.Abs(LunarHelper.rad2rrad(this.mCJ - this.sCJ)) < 50 / 180 * Math.PI)
            {
                ZB.line_earth(this.mCJ, this.mCW, this.mR, this.sCJ, this.sCW, this.sR * LunarHelper.cs_AU);
                this.zx_J = LunarHelper.rad2rrad(this.gst - ZB.le_J);
                this.zx_W = ZB.le_W; //无解返回值是100
            }
            else this.zx_J = this.zx_W = 100;
        }


        /// <summary>
        /// 把太阳月亮信息形成 HTML 字符串
        /// </summary>
        /// <param name="fs">是否显示ΔT, 黄经章等信息</param>
        /// <returns></returns>
        public string toHTML(double fs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table width='100%' cellspacing=1 cellpadding=0 bgcolor='#FFC0C0'>");

            sb.Append("<tr><td bgcolor=white align=center>");
            sb.Append("平太阳 " + JD.timeStr(this.pty) + " 真太阳 <font color=red>" + JD.timeStr(this.zty) + "</font><br>");
            sb.Append("时差 " + LunarHelper.m2fm(this.sc * 86400, 2, 1) + " 月亮被照亮 " + (this.mIll * 100).ToString("F2") + "% ");
            sb.Append("</td></tr>");

            sb.Append("<tr><td bgcolor=white><center><pre style='margin-top: 0; margin-bottom: 0'><font color=blue><b>表一       月亮            太阳</b></font>\r\n");
            sb.Append("视黄经 " + LunarHelper.rad2str(this.mHJ, 0) + "  " + LunarHelper.rad2str(this.sHJ, 0) + "\r\n");
            sb.Append("视黄纬 " + LunarHelper.rad2str(this.mHW, 0) + "  " + LunarHelper.rad2str(this.sHW, 0) + "\r\n");
            sb.Append("视赤经 " + LunarHelper.rad2str(this.mCJ, 1) + "  " + LunarHelper.rad2str(this.sCJ, 1) + "\r\n");
            sb.Append("视赤纬 " + LunarHelper.rad2str(this.mCW, 0) + "  " + LunarHelper.rad2str(this.sCW, 0) + "\r\n");
            sb.Append("距离     " + this.mR.ToString("F0") + "千米          " + this.sR.ToString("F6") + "AU" + "\r\n");
            sb.Append("</pre></center></td></tr>");

            sb.Append("<tr><td bgcolor=white><center><pre style='margin-top: 0; margin-bottom: 0'><font color=blue><b>表二       月亮            太阳</b></font>\r\n");
            sb.Append("方位角 " + LunarHelper.rad2str(this.mPJ, 0) + "  " + LunarHelper.rad2str(this.sPJ, 0) + "\r\n");
            sb.Append("高度角 " + LunarHelper.rad2str(this.mPW, 0) + "  " + LunarHelper.rad2str(this.sPW, 0) + "\r\n");
            sb.Append("时角   " + LunarHelper.rad2str(this.mShiJ, 0) + "  " + LunarHelper.rad2str(this.sShiJ, 0) + "\r\n");
            sb.Append("视半径(观测点) " + LunarHelper.m2fm(this.mRad, 2, 0) + "     " + LunarHelper.m2fm(this.sRad, 2, 0) + "\r\n");
            sb.Append("</pre></center></td></tr>");

            if (fs != 0)
            {
                sb.Append("<tr><td bgcolor=white align=center>");
                sb.Append("力学时 " + JD.setFromJD_str(this.T + LunarHelper.J2000));
                sb.Append(" ΔT=" + (this.dt * 86400).ToString("F1") + "秒<br>");
                sb.Append("黄经章 " + (this.dL / LunarHelper.pi2 * 360 * 3600).ToString("F2") + "\" ");
                sb.Append("交角章 " + (this.dE / LunarHelper.pi2 * 360 * 3600).ToString("F2") + "\" ");
                sb.Append("ε=" + LunarHelper.trim(LunarHelper.rad2str(this.E, 0)));
                sb.Append("</td></tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        #endregion 公共方法



        #region 转换时新增的方法

        /// <summary>
        /// 把太阳月亮信息形成纯文本字符串
        /// </summary>
        /// <param name="fs">是否显示ΔT, 黄经章等信息</param>
        /// <returns></returns>
        public string toText(double fs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("平太阳 " + JD.timeStr(this.pty) + " 真太阳 " + JD.timeStr(this.zty) + "\r\n");
            sb.Append("时差 " + LunarHelper.m2fm(this.sc * 86400, 2, 1) + " 月亮被照亮 " + (this.mIll * 100).ToString("F2") + "% ");
            sb.Append("\r\n");

            sb.Append("\r\n表一       月亮            太阳\r\n");
            sb.Append("视黄经 " + LunarHelper.rad2str(this.mHJ, 0) + "  " + LunarHelper.rad2str(this.sHJ, 0) + "\r\n");
            sb.Append("视黄纬 " + LunarHelper.rad2str(this.mHW, 0) + "  " + LunarHelper.rad2str(this.sHW, 0) + "\r\n");
            sb.Append("视赤经 " + LunarHelper.rad2str(this.mCJ, 1) + "  " + LunarHelper.rad2str(this.sCJ, 1) + "\r\n");
            sb.Append("视赤纬 " + LunarHelper.rad2str(this.mCW, 0) + "  " + LunarHelper.rad2str(this.sCW, 0) + "\r\n");
            sb.Append("距离     " + this.mR.ToString("F0") + "千米          " + this.sR.ToString("F6") + "AU" + "\r\n");

            sb.Append("\r\n表二       月亮            太阳\r\n");
            sb.Append("方位角 " + LunarHelper.rad2str(this.mPJ, 0) + "  " + LunarHelper.rad2str(this.sPJ, 0) + "\r\n");
            sb.Append("高度角 " + LunarHelper.rad2str(this.mPW, 0) + "  " + LunarHelper.rad2str(this.sPW, 0) + "\r\n");
            sb.Append("时角   " + LunarHelper.rad2str(this.mShiJ, 0) + "  " + LunarHelper.rad2str(this.sShiJ, 0) + "\r\n");
            sb.Append("视半径(观测点) " + LunarHelper.m2fm(this.mRad, 2, 0) + "     " + LunarHelper.m2fm(this.sRad, 2, 0) + "\r\n");

            if (fs != 0)
            {
                sb.Append("\r\n力学时 " + JD.setFromJD_str(this.T + LunarHelper.J2000));
                sb.Append(" ΔT=" + (this.dt * 86400).ToString("F1") + "秒\r\n");
                sb.Append("黄经章 " + (this.dL / LunarHelper.pi2 * 360 * 3600).ToString("F2") + "\" ");
                sb.Append("交角章 " + (this.dE / LunarHelper.pi2 * 360 * 3600).ToString("F2") + "\" ");
                sb.Append("\r\nε=" + LunarHelper.trim(LunarHelper.rad2str(this.E, 0)));
            }
            return sb.ToString();
        }

        #endregion
    }
}
