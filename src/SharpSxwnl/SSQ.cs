using System;
using System.Text;

namespace SharpSxwnl;

/// <summary>
/// 实朔实气计算器
/// </summary>
public static class SSQ
{
    #region 私有字段

    /// <summary>
    /// 朔修正表
    /// </summary>
    private static string SB = SSQ.init(1);    // 朔修正表

    /// <summary>
    /// 气修正表
    /// </summary>
    private static string QB = SSQ.init(0);    // 气修正表

    /// <summary>
    /// 朔直线拟合参数
    /// </summary>
    private static double[] suoKB = new double[]{ //朔直线拟合参数
        1457698.231017,29.53067166, // -721-12-17 h=0.00032 古历·春秋
        1546082.512234,29.53085106, // -479-12-11 h=0.00053 古历·战国
        1640640.735300,29.53060000, // -221-10-31 h=0.01010 古历·秦汉
        1642472.151543,29.53085439, // -216-11-04 h=0.00040 古历·秦汉

        1683430.509300,29.53086148, // -104-12-25 h=0.00313 汉书·律历志(太初历)平气平朔
        1752148.041079,29.53085097, //   85-02-13 h=0.00049 后汉书·律历志(四分历)
        1807665.420323,29.53059851, //  237-02-12 h=0.00033 晋书·律历志(景初历)
        1883618.114100,29.53060000, //  445-01-24 h=0.00030 宋书·律历志(何承天元嘉历)
        1907360.704700,29.53060000, //  510-01-26 h=0.00030 宋书·律历志(祖冲之大明历)
        1936596.224900,29.53060000, //  590-02-10 h=0.01010 随书·律历志(开皇历)
        1939135.675300,29.53060000, //  597-01-24 h=0.00890 随书·律历志(大业历)
        1947168.00//  619-01-21
    };

    /// <summary>
    /// 气直线拟合参数
    /// </summary>
    private static double[] qiKB = new double[]{ //气直线拟合参数
        1640650.479938,15.21842500, // -221-11-09 h=0.01709 古历·秦汉
        1642476.703182,15.21874996, // -216-11-09 h=0.01557 古历·秦汉

        1683430.515601,15.218750011, // -104-12-25 h=0.01560 汉书·律历志(太初历)平气平朔 回归年=365.25000
        1752157.640664,15.218749978, //   85-02-23 h=0.01559 后汉书·律历志(四分历) 回归年=365.25000
        1807675.003759,15.218620279, //  237-02-22 h=0.00010 晋书·律历志(景初历) 回归年=365.24689
        1883627.765182,15.218612292, //  445-02-03 h=0.00026 宋书·律历志(何承天元嘉历) 回归年=365.24670
        1907369.128100,15.218449176, //  510-02-03 h=0.00027 宋书·律历志(祖冲之大明历) 回归年=365.24278
        1936603.140413,15.218425000, //  590-02-17 h=0.00149 随书·律历志(开皇历) 回归年=365.24220
        1939145.524180,15.218466998, //  597-02-03 h=0.00121 随书·律历志(大业历) 回归年=365.24321
        1947180.798300,15.218524844, //  619-02-03 h=0.00052 新唐书·历志(戊寅元历)平气定朔 回归年=365.24460
        1964362.041824,15.218533526, //  666-02-17 h=0.00059 新唐书·历志(麟德历) 回归年=365.24480
        1987372.340971,15.218513908, //  729-02-16 h=0.00096 新唐书·历志(大衍历,至德历) 回归年=365.24433
        1999653.819126,15.218530782, //  762-10-03 h=0.00093 新唐书·历志(五纪历) 回归年=365.24474
        2007445.469786,15.218535181, //  784-02-01 h=0.00059 新唐书·历志(正元历,观象历) 回归年=365.24484
        2021324.917146,15.218526248, //  822-02-01 h=0.00022 新唐书·历志(宣明历) 回归年=365.24463
        2047257.232342,15.218519654, //  893-01-31 h=0.00015 新唐书·历志(崇玄历) 回归年=365.24447
        2070282.898213,15.218425000, //  956-02-16 h=0.00149 旧五代·历志(钦天历) 回归年=365.24220
        2073204.872850,15.218515221, //  964-02-16 h=0.00166 宋史·律历志(应天历) 回归年=365.24437
        2080144.500926,15.218530782, //  983-02-16 h=0.00093 宋史·律历志(乾元历) 回归年=365.24474
        2086703.688963,15.218523776, // 1001-01-31 h=0.00067 宋史·律历志(仪天历,崇天历) 回归年=365.24457
        2110033.182763,15.218425000, // 1064-12-15 h=0.00669 宋史·律历志(明天历) 回归年=365.24220
        2111190.300888,15.218425000, // 1068-02-15 h=0.00149 宋史·律历志(崇天历) 回归年=365.24220
        2113731.271005,15.218515671, // 1075-01-30 h=0.00038 李锐补修(奉元历) 回归年=365.24438
        2120670.840263,15.218425000, // 1094-01-30 h=0.00149 宋史·律历志 回归年=365.24220
        2123973.309063,15.218425000, // 1103-02-14 h=0.00669 李锐补修(占天历) 回归年=365.24220
        2125068.997336,15.218477932, // 1106-02-14 h=0.00056 宋史·律历志(纪元历) 回归年=365.24347
        2136026.312633,15.218472436, // 1136-02-14 h=0.00088 宋史·律历志(统元历,乾道历,淳熙历) 回归年=365.24334
        2156099.495538,15.218425000, // 1191-01-29 h=0.00149 宋史·律历志(会元历) 回归年=365.24220
        2159021.324663,15.218425000, // 1199-01-29 h=0.00149 宋史·律历志(统天历) 回归年=365.24220
        2162308.575254,15.218461742, // 1208-01-30 h=0.00146 宋史·律历志(开禧历) 回归年=365.24308
        2178485.706538,15.218425000, // 1252-05-15 h=0.04606 淳祐历 回归年=365.24220
        2178759.662849,15.218445786, // 1253-02-13 h=0.00231 会天历 回归年=365.24270
        2185334.020800,15.218425000, // 1271-02-13 h=0.00520 宋史·律历志(成天历) 回归年=365.24220
        2187525.481425,15.218425000, // 1277-02-12 h=0.00520 本天历 回归年=365.24220
        2188621.191481,15.218437484, // 1280-02-13 h=0.00013 元史·历志(郭守敬授时历) 回归年=365.24250
        2321919.49// 1645-02-04
    };

    #endregion 私有字段



    #region 私有方法

    /// <summary>
    /// 低精度定朔计算,在2000年至600，误差在2小时以内(仍比古代日历精准很多)
    /// </summary>
    /// <param name="W"></param>
    /// <returns></returns>
    private static double so_low(double W)
    {
        double v = 7771.37714500204;
        double t = (W + 1.08472) / v;
        t -= (-0.0000331 * t * t
              + 0.10976 * Math.Cos(0.785 + 8328.6914 * t)
              + 0.02224 * Math.Cos(0.187 + 7214.0629 * t)
              - 0.03342 * Math.Cos(4.669 + 628.3076 * t)) / v
             + (32 * (t + 1.8) * (t + 1.8) - 20) / 86400 / 36525;
        return t * 36525 + 8d / 24d;
    }


    /// <summary>
    /// 低精度定气计算, 最大误差小于30分钟，平均5分
    /// </summary>
    /// <param name="W"></param>
    /// <returns></returns>
    private static double qi_low(double W)
    {
        double t, L, v = 628.3319653318;
        t = (W - 4.895062166) / v; //第一次估算,误差2天以内
        t -= (53 * t * t + 334116 * Math.Cos(4.67 + 628.307585 * t) + 2061 * Math.Cos(2.678 + 628.3076 * t) * t) / v / 10000000; //第二次估算,误差2小时以内

        L = 48950621.66 + 6283319653.318 * t + 53 * t * t    //平黄经
            + 334166 * Math.Cos(4.669257 + 628.307585 * t)   //地球椭圆轨道级数展开
            + 3489 * Math.Cos(4.6261 + 1256.61517 * t)       //地球椭圆轨道级数展开
            + 2060.6 * Math.Cos(2.67823 + 628.307585 * t) * t  //一次泊松项
            - 994 - 834 * Math.Sin(2.1824 - 33.75705 * t);     //光行差与章动修正

        t -= (L / 10000000 - W) / 628.332 + (32 * (t + 1.8) * (t + 1.8) - 20) / 86400 / 36525;
        return t * 36525 + 8d / 24d;
    }


    /// <summary>
    /// 较高精度定气计算
    /// </summary>
    /// <param name="W"></param>
    /// <returns></returns>
    private static double qi_high(double W)
    {
        double t = XL.S_aLon_t2(W) * 36525;
        t = t - JD.deltatT2(t) + 8d / 24d;
        double v = ((t + 0.5) % 1) * 86400;
        if (v < 600 || v > 86400 - 600) t = XL.S_aLon_t(W) * 36525 - JD.deltatT2(t) + 8d / 24d;
        return t;
    }


    /// <summary>
    /// 较高精度朔计算
    /// </summary>
    /// <param name="W"></param>
    /// <returns></returns>
    private static double so_high(double W)
    {
        double t = XL.MS_aLon_t2(W) * 36525;
        t = t - JD.deltatT2(t) + 8d / 24d;
        double v = ((t + 0.5) % 1) * 86400;
        if (v < 600 || v > 86400 - 600) t = XL.MS_aLon_t(W) * 36525 - JD.deltatT2(t) + 8d / 24d;
        return t;
    }



    /// <summary>
    /// 气朔解压缩
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private static string jieya(string s)
    {
        string o = "0000000000", o2 = o + o;
        StringBuilder sb = new StringBuilder(s, 32768);
        sb.Replace("J", "00");
        sb.Replace("I", "000");
        sb.Replace("H", "0000");
        sb.Replace("G", "00000");
        sb.Replace("t", "02");
        sb.Replace("s", "002");
        sb.Replace("r", "0002");
        sb.Replace("q", "00002");
        sb.Replace("p", "000002");
        sb.Replace("o", "0000002");
        sb.Replace("n", "00000002");
        sb.Replace("m", "000000002");
        sb.Replace("l", "0000000002");
        sb.Replace("k", "01");
        sb.Replace("j", "0101");
        sb.Replace("i", "001");
        sb.Replace("h", "001001");
        sb.Replace("g", "0001");
        sb.Replace("f", "00001");
        sb.Replace("e", "000001");
        sb.Replace("d", "0000001");
        sb.Replace("c", "00000001");
        sb.Replace("b", "000000001");
        sb.Replace("a", "0000000001");
        sb.Replace("A", o2 + o2 + o2);
        sb.Replace("B", o2 + o2 + o);
        sb.Replace("C", o2 + o2);
        sb.Replace("D", o2 + o);
        sb.Replace("E", o2);
        sb.Replace("F", o);
        return sb.ToString();
    }


    /// <summary>
    /// 初使化: 朔修正表, 气修正表
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string init(int type)
    {
        StringBuilder suoS = new StringBuilder(), qiS = new StringBuilder();
        //  619-01-21开始16598个朔日修正表 d0=1947168
        suoS.Append("EqoFscDcrFpmEsF2DfFideFelFpFfFfFiaipqti1ksttikptikqckstekqttgkqttgkqteksttikptikq2fjstgjqttjkqttgkqt");
        suoS.Append("ekstfkptikq2tijstgjiFkirFsAeACoFsiDaDiADc1AFbBfgdfikijFifegF1FhaikgFag1E2btaieeibggiffdeigFfqDfaiBkF");
        suoS.Append("1kEaikhkigeidhhdiegcFfakF1ggkidbiaedksaFffckekidhhdhdikcikiakicjF1deedFhFccgicdekgiFbiaikcfi1kbFibef");
        suoS.Append("gEgFdcFkFeFkdcfkF1kfkcickEiFkDacFiEfbiaejcFfffkhkdgkaiei1ehigikhdFikfckF1dhhdikcfgjikhfjicjicgiehdik");
        suoS.Append("cikggcifgiejF1jkieFhegikggcikFegiegkfjebhigikggcikdgkaFkijcfkcikfkcifikiggkaeeigefkcdfcfkhkdgkegieid");
        suoS.Append("hijcFfakhfgeidieidiegikhfkfckfcjbdehdikggikgkfkicjicjF1dbidikFiggcifgiejkiegkigcdiegfggcikdbgfgefjF1");
        suoS.Append("kfegikggcikdgFkeeijcfkcikfkekcikdgkabhkFikaffcfkhkdgkegbiaekfkiakicjhfgqdq2fkiakgkfkhfkfcjiekgFebicg");
        suoS.Append("gbedF1jikejbbbiakgbgkacgiejkijjgigfiakggfggcibFifjefjF1kfekdgjcibFeFkijcfkfhkfkeaieigekgbhkfikidfcje");
        suoS.Append("aibgekgdkiffiffkiakF1jhbakgdki1dj1ikfkicjicjieeFkgdkicggkighdF1jfgkgfgbdkicggfggkidFkiekgijkeigfiski");
        suoS.Append("ggfaidheigF1jekijcikickiggkidhhdbgcfkFikikhkigeidieFikggikhkffaffijhidhhakgdkhkijF1kiakF1kfheakgdkif");
        suoS.Append("iggkigicjiejkieedikgdfcggkigieeiejfgkgkigbgikicggkiaideeijkefjeijikhkiggkiaidheigcikaikffikijgkiahi1");
        suoS.Append("hhdikgjfifaakekighie1hiaikggikhkffakicjhiahaikggikhkijF1kfejfeFhidikggiffiggkigicjiekgieeigikggiffig");
        suoS.Append("gkidheigkgfjkeigiegikifiggkidhedeijcfkFikikhkiggkidhh1ehigcikaffkhkiggkidhh1hhigikekfiFkFikcidhh1hit");
        suoS.Append("cikggikhkfkicjicghiediaikggikhkijbjfejfeFhaikggifikiggkigiejkikgkgieeigikggiffiggkigieeigekijcijikgg");
        suoS.Append("ifikiggkideedeijkefkfckikhkiggkidhh1ehijcikaffkhkiggkidhh1hhigikhkikFikfckcidhh1hiaikgjikhfjicjicgie");
        suoS.Append("hdikcikggifikigiejfejkieFhegikggifikiggfghigkfjeijkhigikggifikiggkigieeijcijcikfksikifikiggkidehdeij");
        suoS.Append("cfdckikhkiggkhghh1ehijikifffffkhsFngErD1pAfBoDd1BlEtFqA2AqoEpDqElAEsEeB2BmADlDkqBtC1FnEpDqnEmFsFsAFn");
        suoS.Append("llBbFmDsDiCtDmAB2BmtCgpEplCpAEiBiEoFqFtEqsDcCnFtADnFlEgdkEgmEtEsCtDmADqFtAFrAtEcCqAE1BoFqC1F1DrFtBmF");
        suoS.Append("tAC2ACnFaoCgADcADcCcFfoFtDlAFgmFqBq2bpEoAEmkqnEeCtAE1bAEqgDfFfCrgEcBrACfAAABqAAB1AAClEnFeCtCgAADqDoB");
        suoS.Append("mtAAACbFiAAADsEtBqAB2FsDqpFqEmFsCeDtFlCeDtoEpClEqAAFrAFoCgFmFsFqEnAEcCqFeCtFtEnAEeFtAAEkFnErAABbFkAD");
        suoS.Append("nAAeCtFeAfBoAEpFtAABtFqAApDcCGJ");
        //1645-02-03开始7582个节气修正表
        qiS.Append("EmcFs22AFsckF2tsDtFqEtF1posFdFgiFseFtmelpsEfhkF2anmelpFlF1ikrotcnEqEq2FfqmcDsrFor22FgFrcgDscFs22FgEe");
        qiS.Append("FtE2sfFs22sCoEsaF2tsD1FpeE2eFsssEciFsFnmelpFcFhkF2tcnEqEpFgkrotcnEqrEtFermcDsrE222FgBmcmr22DaEfnaF22");
        qiS.Append("2sD1FpeForeF2tssEfiFpEoeFssD1iFstEqFppDgFstcnEqEpFg11FscnEqrAoAF2ClAEsDmDtCtBaDlAFbAEpAAAAAD2FgBiBqo");
        qiS.Append("BbnBaBoAAAAAAAEgDqAdBqAFrBaBoACdAAf1AACgAAAeBbCamDgEifAE2AABa1C1BgFdiAAACoCeE1ADiEifDaAEqAAFe1AcFbcA");
        qiS.Append("AAAAF1iFaAAACpACmFmAAAAAAAACrDaAAADG0");

        if (type == 1)
        {
            return SSQ.jieya(suoS.ToString());
        }
        else
        {
            return SSQ.jieya(qiS.ToString());
        }
    }

    #endregion 私有方法



    #region 公共方法(1)

    /// <summary>
    /// jd 应靠近所要取得的气朔日, 当参数 qs="气" 时，算节气的儒略日
    /// </summary>
    /// <param name="jd"></param>
    /// <param name="qs"></param>
    /// <returns></returns>
    public static double calc(double jd, string qs)
    {
        jd += LunarHelper.J2000;    // C#: 改为常量
        int i;
        double D;
        string n;
        double[] B = SSQ.suoKB;
        double pc = 14;

        if (qs == "气") { B = SSQ.qiKB; pc = 7; }
        double f1 = B[0] - pc, f2 = B[B.Length - 1] - pc, f3 = 2436935;

        if (jd < f1 || jd >= f3)
        {
            // 平气朔表中首个之前，使用现代天文算法。1960.1.1以后，使用现代天文算法 (这一部分调用了qi_high和so_high,所以需星历表支持)
            if (qs == "气")
                return Math.Floor(SSQ.qi_high(Math.Floor((jd + pc - 2451259) / 365.2422 * 24) * Math.PI / 12) + 0.5); //2451259是1999.3.21,太阳视黄经为0,春分.定气计算
            else
                return Math.Floor(SSQ.so_high(Math.Floor((jd + pc - 2451551) / 29.5306) * Math.PI * 2) + 0.5); //2451551是2000.1.7的那个朔日,黄经差为0.定朔计算
        }

        if (jd >= f1 && jd < f2)
        {
            // 平气或平朔
            for (i = 0; i < B.Length; i += 2) if (jd + pc < B[i + 2])
                    break;
            D = B[i] + B[i + 1] * Math.Floor((jd + pc - B[i]) / B[i + 1]);
            D = Math.Floor(D + 0.5);
            if (D == 1683460) D++; //如果使用太初历计算-103年1月24日的朔日,结果得到的是23日,这里修正为24日(实历)。修正后仍不影响-103的无中置闰。如果使用秦汉历，得到的是24日，本行D不会被执行。
            return D - LunarHelper.J2000;    // C#: 改为常量
        }

        if (jd >= f2 && jd < f3)
        {
            // 定气或定朔
            if (qs == "气")
            {
                D = Math.Floor(SSQ.qi_low(Math.Floor((jd + pc - 2451259) / 365.2422 * 24) * Math.PI / 12) + 0.5); //2451259是1999.3.21,太阳视黄经为0,春分.定气计算
                n = SSQ.QB.Substring((int)(Math.Floor((jd - f2) / 365.2422 * 24)), 1); //找定气修正值
            }
            else
            {
                D = Math.Floor(SSQ.so_low(Math.Floor((jd + pc - 2451551) / 29.5306) * Math.PI * 2) + 0.5); //2451551是2000.1.7的那个朔日,黄经差为0.定朔计算
                n = SSQ.SB.Substring((int)(Math.Floor((jd - f2) / 29.5306)), 1); //找定朔修正值
            }
            if (n == "1") return D + 1;
            if (n == "2") return D - 1;
            return D;
        }

        return 0;    // C#: 新增
    }

    #endregion 公共方法(1)



    #region 公共属性(注: 初始转换时为公共字段, 已改写, 请参阅“转换时增加的私有字段”)

    // 排月序(生成实际年历),在调用calcY()后得到以下数据
    // 时间系统全部使用北京时，即使是天象时刻的输出，也是使用北京时
    // 如果天象的输出不使用北京时，会造成显示混乱，更严重的是无法与古历比对

    /// <summary>
    /// 闰月位置
    /// </summary>
    public static int leap { get; set; }    // = 0;         //闰月位置

    /// <summary>
    /// 各月名称
    /// </summary>
    //------------------------------------------------------------------------------------------------------------
    // 初次转换时的语句如下:
    // public static string[] ym = new string[32]; //各月名称
    //------------------------------------------------------------------------------------------------------------
    public static string[] ym
    {
        get => SSQ.__ym;
        set => SSQ.__ym = value;
    }

    /// <summary>
    /// 中气表, 功能与 doulbe[] 类似, 但它有自定义属性的 
    /// </summary>
    //------------------------------------------------------------------------------------------------------------------------------------
    // 初次转换时的语句如下:
    // public static LunarInfoListT<double> ZQ = new LunarInfoListT<double>(31, 0d); //中气表,其中.liqiu是节气立秋的儒略日,计算三伏时用到
    //------------------------------------------------------------------------------------------------------------------------------------
    public static LunarInfoListT<double> ZQ
    {
        get => SSQ.__ZQ;
        set => SSQ.__ZQ = value;
    }

    /// <summary>
    /// 合朔表
    /// </summary>
    //------------------------------------------------------------------------------------------------------------
    // 初次转换时的语句如下:
    // public static double[] HS = new double[32]; //合朔表
    //------------------------------------------------------------------------------------------------------------
    public static double[] HS
    {
        get => SSQ.__HS;
        set => SSQ.__HS = value;
    }

    /// <summary>
    /// 各月大小
    /// </summary>
    //------------------------------------------------------------------------------------------------------------
    // 初次转换时的语句如下:
    // public static double[] dx = new double[32]; //各月大小
    //------------------------------------------------------------------------------------------------------------
    public static double[] dx
    {
        get => SSQ.__dx;
        set => SSQ.__dx = value;
    }

    /// <summary>
    /// 年计数
    /// </summary>
    //------------------------------------------------------------------------------------------------------------
    // 初次转换时的语句如下:
    // public static double[] Yn = new double[1];  //年计数
    //------------------------------------------------------------------------------------------------------------
    public static double[] Yn
    {
        get => SSQ.__Yn;
        set => SSQ.__Yn = value;
    }

    #endregion 公共属性



    #region 公共方法(2)

    /// <summary>
    /// 农历排月序计算,可定出农历,有效范围：两个冬至之间(冬至一 ≤ d ＜ 冬至二)
    /// </summary>
    /// <param name="jd"></param>
    public static void calcY(double jd)
    {
        LunarInfoListT<double> A = SSQ.ZQ;
        double[] B = SSQ.HS;  // 中气表,日月合朔表(整日)
        int i;
        double W, w;

        // 该年的气
        W = LunarHelper.int2((jd - 355 + 183) / 365.2422) * 365.2422 + 355;  // 355是2000.12冬至,得到较靠近jd的冬至估计值
        if (SSQ.calc(W, "气") > jd) W -= 365.2422;
        for (i = 0; i < 25; i++) A[i] = SSQ.calc(W + 15.2184 * i, "气");     // 25个节气时刻(北京时间),从冬至开始到下一个冬至以后
        A.pe1 = SSQ.calc(W - 15.2, "气"); A.pe2 = SSQ.calc(W - 30.4, "气");  // 补算二气,确保一年中所有月份的“气”全部被计算在内

        // 今年"首朔"的日月黄经差w
        w = SSQ.calc(A[0], "朔");    // 求较靠近冬至的朔日
        if (w > A[0]) w -= 29.53;

        // 该年所有朔,包含14个月的始末
        for (i = 0; i < 15; i++)
        {
            B[i] = SSQ.calc(w + 29.5306 * i, "朔");
        }

        // 月大小
        SSQ.leap = 0;
        for (i = 0; i < 14; i++)
        {
            SSQ.dx[i] = SSQ.HS[i + 1] - SSQ.HS[i]; // 月大小
            SSQ.ym[i] = i.ToString();  // 月序初始化
        }



        // -721年至-104年的后九月及月建问题,与朔有关，与气无关
        double YY = LunarHelper.int2((SSQ.ZQ[0] + 10 + 180) / 365.2422) + 2000; // 确定年份
        if (YY >= -721 && YY <= -104)
        {
            double ly = 0, b0 = 0, k0 = 0, x0 = 0;    // ly为历元(本历首月的儒略数),x0月建,lName闰月名称,b0,k0为置闰拟合参数
            string lName = "";
            if (YY >= -721) { ly = 1457698 - LunarHelper.J2000; k0 = 12.368422; b0 = 0.342; lName = "十三"; x0 = 2; }    // 春秋历,ly为-722.12.17
            if (YY >= -479) { ly = 1546083 - LunarHelper.J2000; k0 = 12.368422; b0 = 0.500; lName = "十三"; x0 = 2; }    // 战国历,ly为-480.12.11
            if (YY >= -220) { ly = 1640641 - LunarHelper.J2000; k0 = 12.369000; b0 = 0.866; lName = "后九"; x0 = 11; }   // 秦汉历,ly为-221.10.31;
            double nY = LunarHelper.int2((SSQ.HS[0] - ly + 100) / 365.25);    // 年积数
            double f1 = LunarHelper.int2(b0 + nY * k0), f2 = LunarHelper.int2(b0 + nY * k0 + k0), f3;    // f1有本年首的月积数,f2为下一年首的月积数
            f1 = LunarHelper.int2(f1); f2 = LunarHelper.int2(f2);
            for (i = 0; i < 14; i++)
            {
                f3 = LunarHelper.int2((SSQ.HS[i] - ly + 15) / 29.5306);    // 该月积数
                if (f3 < f2) f3 -= f1; else f3 -= f2;
                if (f3 < 12) SSQ.ym[i] = obb.ymc[(int)((f3 + x0) % 12)]; else SSQ.ym[i] = lName;
            }

        }

        // 无中气置闰法确定闰月,(气朔结合法,数据源需有冬至开始的的气和朔)
        if (B[13] <= A[24])
        {
            // 第13月的月末没有超过冬至(不含冬至),说明今年含有13个月
            for (i = 1; B[i + 1] > A[2 * i] && i < 13; i++) ; //在13个月中找第1个没有中气的月份
            SSQ.leap = i;
            for (; i < 14; i++) SSQ.ym[i] = (int.Parse(SSQ.ym[i]) - 1).ToString();
        }

        // 名称转换(月建别名)
        for (i = 0; i < 14; i++)
        {
            double Dm = SSQ.HS[i] + LunarHelper.J2000, v2 = int.Parse(SSQ.ym[i]);  // Dm初一的儒略日,v2为月建序号
            string mc = obb.ymc[(int)(v2 % 12)];    // 月建对应的默认月名称：建子十一,建丑十二,建寅为正……
            if (Dm >= 1724360 && Dm <= 1729794) mc = obb.ymc[(int)((v2 + 1) % 12)];        //  8.01.15至 23.12.02 建子为十二,其它顺推
            else if (Dm >= 1807724 && Dm <= 1808699) mc = obb.ymc[(int)((v2 + 1) % 12)];   // 237.04.12至239.12.13 建子为十二,其它顺推
            else if (Dm >= 1999349 && Dm <= 1999467) mc = obb.ymc[(int)((v2 + 2) % 12)];   // 761.12.02至762.03.30 建子为正月,其它顺推
            else if (Dm >= 1973067 && Dm <= 1977112) { if (v2 % 12 == 0) mc = "正"; if (v2 == 2) mc = "一"; }    // 689.12.18至701.01.14 建子为正月,建寅为一月,其它不变

            if (Dm == 1729794 || Dm == 1808699) mc = "拾贰";    // 239.12.13及23.12.02均为十二月,为避免两个连续十二月，此处改名

            SSQ.ym[i] = mc;
        }
    }

    #endregion 公共方法(2)



    #region 转换时增加的私有字段(用于封装成公共属性, 按转换规范 10 命名)

    private static string[] __ym = new string[32];  //各月名称
    private static LunarInfoListT<double> __ZQ = new LunarInfoListT<double>(31, 0d); //中气表,其中.liqiu是节气立秋的儒略日,计算三伏时用到
    private static double[] __HS = new double[32];  //合朔表
    private static double[] __dx = new double[32];  //各月大小
    private static double[] __Yn = new double[1];   //年计数

    #endregion




    #region 转换时新增加的方法

    /// <summary>
    /// 新增方法: 计算节气, 并返回计算的节气总数, 在调用本方法并读取 SSQ.ZQ 数据后, 应该清零 SSQ.ZQ
    /// </summary>
    /// <param name="jd"></param>
    /// <param name="calcMultiPeriod"></param>
    /// <returns></returns>
    public static int calcJieQi(double jd, bool calcMultiPeriod)
    {
        LunarInfoListT<double> A = SSQ.ZQ;
        double[] B = SSQ.HS;    // 中气表,日月合朔表(整日)
        int i;
        double W;

        if (!calcMultiPeriod)    // 只计算某年的节气
        {
            W = LunarHelper.int2((jd - 355 + 183) / 365.2422) * 365.2422 + 355;  // 355是2000.12冬至,得到较靠近jd的冬至估计值
            if (SSQ.calc(W, "气") > jd) W -= 365.2422;
            for (i = 0; i < 25; i++) A[i] = SSQ.calc(W + 15.2184 * i, "气");     // 25个节气时刻(北京时间),从冬至开始到下一个冬至以后
            A.pe1 = SSQ.calc(W - 15.2, "气"); A.pe2 = SSQ.calc(W - 30.4, "气");  // 补算二气,确保一年中所有月份的“气”全部被计算在内
        }
        else    // 需计算从霜降至下 2 个大寒之后的节气, 用于计算指定日期的所属节气, 上一节气, 下一节气等信息
        {
            W = LunarHelper.int2((jd - 355 + 183) / 365.2422) * 365.2422 + 296;  // 296是2000.10.23霜降(距200.1.1的天数),得到较靠近jd的霜降估计值
            if (SSQ.calc(W, "气") > jd) W -= 365.2422;
            for (i = 0; i < 31; i++) A[i] = SSQ.calc(W + 15.2184 * i, "气");     // 31个节气时刻(北京时间),从霜降至开始到下 2 个大寒以后
        }
        return ((!calcMultiPeriod ? 25 : 31));
    }



    #endregion

}