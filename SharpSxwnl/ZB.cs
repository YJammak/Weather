using System;
using System.Collections.Generic;
using System.Text;

namespace SharpSxwnl
{
    /// <summary>
    /// 坐标类
    /// </summary>
    public static class ZB
    {
        #region 公共属性(注: 初始转换时为字段, 已改写为自动实现的属性)

        /// <summary>
        /// 黄经章动
        /// </summary>
        public static double dL { get; set; }    // = 0;

        /// <summary>
        /// 交角章动
        /// </summary>
        public static double dE { get; set; }    // = 0;

        /// <summary>
        /// 保存空间两点连线与地球的交点: 经(?)
        /// </summary>
        public static double le_J { get; set; }    // = 0;

        /// <summary>
        /// 保存空间两点连线与地球的交点: 纬(?)
        /// </summary>
        public static double le_W { get; set; }    // = 0;

        /// <summary>
        /// (?)
        /// </summary>
        public static double le_R1 { get; set; }    // = 0;

        #endregion



        #region 私有字段(注: 初始转换时为公共字段, 已改写)

        /// <summary>
        /// 章动计算时使用的数据(?)
        /// </summary>
        private static double[] nutB = new double[] {
            2.1824,  -33.75705, 36e-6,-1720,920,
            3.5069, 1256.66393, 11e-6,-132, 57,
            1.3375,16799.4182, -51e-6, -23, 10,
            4.3649,  -67.5141,  72e-6,  21, -9,
            0.04,   -628.302,   0,     -14,  0,
            2.36,   8328.691,   0,       7,  0,
            3.46,   1884.966,   0,      -5,  2,
            5.44,  16833.175,   0,      -4,  2,
            3.69,  25128.110,   0,      -3,  0,
            3.55,    628.362,   0,       2,  0
        };

        #endregion



        #region 公共方法

        /// <summary>
        /// 球面坐标旋转
        /// </summary>
        /// <param name="JW"></param>
        /// <param name="E"></param>
        public static void llrConv(double[] JW, double E)
        {
            //黄道赤道坐标变换,赤到黄E取负
            double sinE = Math.Sin(E), cosE = Math.Cos(E);
            double sinJ = Math.Sin(JW[0]), cosJ = Math.Cos(JW[0]);
            double sinW = Math.Sin(JW[1]), cosW = Math.Cos(JW[1]), tanW = Math.Tan(JW[1]);
            JW[0] = Math.Atan2(sinJ * cosE - tanW * sinE, cosJ);
            JW[1] = Math.Asin(cosE * sinW + sinE * cosW * sinJ);
            JW[0] = LunarHelper.rad2mrad(JW[0]);
        }

        /// <summary>
        /// 球面坐标旋转
        /// </summary>
        /// <param name="JW"></param>
        /// <param name="E"></param>
        public static void llrConv(LunarInfoListT<double> JW, double E)    // C#: 新扩展出来的方法
        {
            //黄道赤道坐标变换,赤到黄E取负
            double sinE = Math.Sin(E), cosE = Math.Cos(E);
            double sinJ = Math.Sin((JW[0])), cosJ = Math.Cos((JW[0]));
            double sinW = Math.Sin((JW[1]));
            double cosW = Math.Cos((JW[1]));
            double tanW = Math.Tan((JW[1]));
            JW[0] = Math.Atan2(sinJ * cosE - tanW * sinE, cosJ);
            JW[1] = Math.Asin(cosE * sinW + sinE * cosW * sinJ);
            JW[0] = LunarHelper.rad2mrad((JW[0]));
        }



        /// <summary>
        /// 球面转直角坐标
        /// </summary>
        /// <param name="J"></param>
        /// <param name="W"></param>
        /// <param name="R"></param>
        /// <param name="z"></param>
        public static void llr2xyz(double J, double W, double R, double[] z)
        {
            z[0] = R * Math.Cos(W) * Math.Cos(J);
            z[1] = R * Math.Cos(W) * Math.Sin(J);
            z[2] = R * Math.Sin(W);
        }



        /// <summary>
        /// 直角坐标转球
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="q"></param>
        public static void xyz2llr(double x, double y, double z, double[] q)
        { 
            q[2] = Math.Sqrt(x * x + y * y + z * z);
            q[1] = Math.Asin(z / q[2]);
            q[0] = LunarHelper.rad2mrad(Math.Atan2(y, x));
        }



        /// <summary>
        /// 章动(黄经章动和交角章动)计算,t是世纪数
        /// </summary>
        /// <param name="t">世纪数</param>
        public static void nutation(double t)
        { 
            int i;
            double c, a, t2 = t * t, dL = 0, dE = 0;
            double[] B = ZB.nutB;
            for (i = 0; i < B.Length; i += 5)
            {
                c = B[i] + B[i + 1] * t + B[i + 2] * t2;
                if (i == 0) a = -1.742 * t; else a = 0;
                dL += (B[i + 3] + a) * Math.Sin(c);
                dE += B[i + 4] * Math.Cos(c);
            }
            ZB.dL = dL / 100 / LunarHelper.rad;  //黄经章动
            ZB.dE = dE / 100 / LunarHelper.rad;  //交角章动
        }



        /// <summary>
        /// 只计算黄经章动
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double nutationLon(double t)
        { 
            int i;
            double a, t2 = t * t, dL = 0;
            double[] B = ZB.nutB;
            for (i = 0; i < B.Length; i += 5)
            {
                if (i == 0) a = -1.742 * t; else a = 0;
                dL += (B[i + 3] + a) * Math.Sin(B[i] + B[i + 1] * t + B[i + 2] * t2);
            }
            return dL / 100 / LunarHelper.rad;
        }


        /// <summary>
        /// 返回黄赤交角,t是世纪数
        /// </summary>
        /// <param name="t">世纪数</param>
        /// <returns></returns>
        public static double hcjj(double t)
        {
            double t2 = t * t, t3 = t2 * t, t4 = t3 * t;
            return (84381.4088 - 46.836051 * t - 0.0001667 * t2 - 0.00199911 * t3 - 0.000000523 * t4) / LunarHelper.rad;
        }



        /// <summary>
        /// 求角度差(未测试)
        /// </summary>
        /// <param name="J1"></param>
        /// <param name="W1"></param>
        /// <param name="J2"></param>
        /// <param name="W2"></param>
        /// <returns></returns>
        public static double j1_j2(double J1, double W1, double J2, double W2)
        {
            double dJ = LunarHelper.rad2rrad(J1 - J2), dW = W1 - W2;
            if (Math.Abs(dJ) < 1 / 1000 && Math.Abs(dW) < 1 / 1000)
            {
                dJ *= Math.Cos((W1 + W2) / 2);
                return Math.Sqrt(dJ * dJ + dW * dW);
            }
            return Math.Acos(Math.Sin(W1) * Math.Sin(W2) + Math.Cos(W1) * Math.Cos(W2) * Math.Cos(dJ));
        }


        /// <summary>
        /// 传入T是2000年首起算的日数(UT),dt是deltatT(日),精度要求不高时dt可取值为0
        /// 返回格林尼治恒星时(不含赤经章动及非多项式部分),即格林尼治子午圈的平春风点起算的赤经
        /// </summary>
        /// <param name="T">2000年首起算的日数(UT)</param>
        /// <param name="dt">deltatT(日)</param>
        /// <returns></returns>
        public static double gst(double T, double dt)
        {
            double t = (T + dt) / 36525, t2 = t * t, t3 = t2 * t, t4 = t3 * t;
            return LunarHelper.pi2 * (0.7790572732640 + 1.00273781191135448 * T) //严格说这里的T是UT,下一行的t是力学时(世纪数)
                + (0.014506 + 4612.15739966 * t + 1.39667721 * t2 - 0.00009344 * t3 + 0.00001882 * t4) / LunarHelper.rad;
        }


        /// <summary>
        /// 太阳光行差,t是世纪数
        /// </summary>
        /// <param name="t">世纪数</param>
        /// <returns></returns>
        public static double gxc_sunLon(double t)
        {
            double v = -0.043126 + 628.301955 * t - 0.000002732 * t * t; //平近点角
            double e = 0.016708634 - 0.000042037 * t - 0.0000001267 * t * t;
            return (-20.49552 * (1 + e * Math.Cos(v))) / LunarHelper.rad; //黄经光行差
        }


        /// <summary>
        /// 黄纬光行差
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double gxc_sunLat(double t)
        {
            return 0;
        }


        /// <summary>
        /// 月球经度光行差,误差0.07"
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double gxc_moonLon(double t)
        {
            return -3.4E-6;
        }


        /// <summary>
        /// 月球纬度光行差,误差0.006"
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double gxc_moonLat(double t)
        {
            return 0.063 * Math.Sin(0.057 + 8433.4662 * t + 0.000064 * t * t) / LunarHelper.rad;
        }


        /// <summary>
        /// 大气折射,ho是视高度
        /// </summary>
        /// <param name="ho"></param>
        /// <returns></returns>
        public static double AR(double ho)
        {
            return -0.0002909 / Math.Tan(ho + 0.002227 / (ho + 0.07679));
        }


        /// <summary>
        /// 大气折射,h是真高度
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public static double AR2(double h)
        {
            return 0.0002967 / Math.Tan(h + 0.003138 / (h + 0.08919));
        }


        /// <summary>
        /// 视差修正
        /// </summary>
        /// <param name="z">赤道坐标</param>
        /// <param name="H">时角</param>
        /// <param name="fa">地理纬度</param>
        /// <param name="high">海拔(千米)</param>
        public static void parallax(double[] z, double H, double fa, double high)
        {
            double dw = 1d;
            if (z[2] < 500) dw = LunarHelper.cs_AU;
            z[2] *= dw;
            double r0, x0, y0, z0, f = 0.99664719;
            double u = Math.Atan(f * Math.Tan(fa));
            double g = z[0] + H;
            r0 = LunarHelper.cs_rEar * Math.Cos(u) + high * Math.Cos(fa); //站点与地地心向径的赤道投影长度
            z0 = LunarHelper.cs_rEar * Math.Sin(u) * f + high * Math.Sin(fa); //站点与地地心向径的轴向投影长度
            x0 = r0 * Math.Cos(g);
            y0 = r0 * Math.Sin(g);

            ZB.llr2xyz(z[0], z[1], z[2], z);
            z[0] -= x0; z[1] -= y0; z[2] -= z0;
            ZB.xyz2llr(z[0], z[1], z[2], z);
            z[2] /= dw;
        }



        /// <summary>
        /// 求空间两点连线与地球的交点(靠近点1的交点),返回在ZB.le_J,le_W,赤道坐标。R1,R2单位是千米
        /// </summary>
        /// <param name="J1"></param>
        /// <param name="W1"></param>
        /// <param name="R1"></param>
        /// <param name="J2"></param>
        /// <param name="W2"></param>
        /// <param name="R2"></param>
        public static void line_earth(double J1, double W1, double R1, double J2, double W2, double R2)
        {
            double x1 = R1 * Math.Cos(W1) * Math.Cos(J1), y1 = R1 * Math.Cos(W1) * Math.Sin(J1), z1 = R1 * Math.Sin(W1);
            double x2 = R2 * Math.Cos(W2) * Math.Cos(J2), y2 = R2 * Math.Cos(W2) * Math.Sin(J2), z2 = R2 * Math.Sin(W2);
            double dx = x2 - x1, dy = y2 - y1, dz = z2 - z1, f = 0.99664719, r = LunarHelper.cs_rEar; //直线参数及地球参数
            double x, y, z, lh = 0;
            if (Math.Abs(dx) < Math.Abs(dy)) //必要时仑换
            { lh = dx; dx = dy; dy = lh; lh = x1; x1 = y1; y1 = lh; lh = 1; }
            double a = dy / dx, b = y1 - a * x1, c = dz / dx / f, d = z1 / f - c * x1;
            double A = a * a + c * c + 1, B = a * b + c * d, C = b * b + d * d - r * r, D = B * B - A * C;
            if (D < 0) { ZB.le_J = ZB.le_W = 100; return; } //返回100表示无解
            D = Math.Sqrt(D); if (x1 + B / A < 0) D = -D;
            x = (-B + D) / A; y = a * x + b; z = (c * x + d) / f;
            ZB.le_R1 = Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1) + (z - z1) * (z - z1));
            if (lh != 0) { lh = x; x = y; y = lh; }
            ZB.le_W = Math.Atan(z / Math.Sqrt(x * x + y * y));
            ZB.le_J = LunarHelper.rad2mrad(Math.Atan2(y, x));
        }

        #endregion

    }
}
