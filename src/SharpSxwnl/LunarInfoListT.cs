using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SharpSxwnl
{
    /// <summary>
    /// 由于在 Javascript 中, 数组可以有自己的属性, 为了对应此功能, 设计本类
    /// 可用的方案: 使用 List&lt;T&gt; 或 ArrayList, 使用前者可以提高代码的效率, 后者则需要装箱拆箱和显式类型转换操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LunarInfoListT<T> : List<T>      // 派生于泛型 List<T> 类, 以提高代码的效率
    {
        #region 公共属性(注: 初始转换时为公共字段, 已改写)

        /// <summary>
        /// 升(时间)
        /// </summary>
        public double s__ { get; set; }    // = 0;

        /// <summary>
        /// 中(时间)
        /// </summary>
        public double z__ { get; set; }    // = 0;

        /// <summary>
        /// 降(时间)
        /// </summary>
        public double j__ { get; set; }    // = 0;

        /// <summary>
        /// 晨(时间)
        /// </summary>
        public double c__ { get; set; }    // = 0;

        /// <summary>
        /// 昏(时间)
        /// </summary>
        public double h__ { get; set; }    // = 0;

        /// <summary>
        /// 晨昏差(时间)
        /// </summary>
        public double ch__ { get; set; }    // = 0;

        /// <summary>
        /// 升降差(时间)
        /// </summary>
        public double sj__ { get; set; }    // = 0;



        /// <summary>
        /// 升(时间串)
        /// </summary>
        public string s { get; set; }    // = "";

        /// <summary>
        /// 中(时间串)
        /// </summary>
        public string z { get; set; }    // = "";

        /// <summary>
        /// 降(时间串)
        /// </summary>
        public string j { get; set; }    // = "";

        /// <summary>
        /// 晨(时间串)
        /// </summary>
        public string c { get; set; }    // = "";

        /// <summary>
        /// 昏(时间串)
        /// </summary>
        public string h { get; set; }    // = "";

        /// <summary>
        /// 日照时间(串)
        /// </summary>
        public string ch { get; set; }    // = "";

        /// <summary>
        /// 昼长(时间串)
        /// </summary>
        public string sj { get; set; }    // = "";
        
        /// <summary>
        /// 月出时间(串)
        /// </summary>
        public string Ms { get; set; }    // = "";

        /// <summary>
        /// 月亮中天时间(串)
        /// </summary>
        public string Mz { get; set; }    // = "";

        /// <summary>
        /// 月落时间(串)
        /// </summary>
        public string Mj { get; set; }    // = "";



        /// <summary>
        /// 本属性(字段)有不同的含义： 
        /// (1) 用于月对象 LunarInfoListT<OB> lun 的属性(字段) dn: 该月的总天数； 
        /// (2) 用于多天的升中降容器 LunarInfoListT<LunarInfoListT<double>> rts 的属性(字段) dn: 要求计算升中降信息的天数
        /// </summary>
        public double dn { get; set; }    // = 0;

        /// <summary>
        /// 指定时刻的天体时角
        /// </summary>
        public double H { get; set; }    // = 0;

        /// <summary>
        /// 本属性(字段)有不同的含义： 升起对应的时角(月亮?), 或地平以下50分的时角(太阳?)
        /// </summary>
        public double H0 { get; set; }    // = 0;

        /// <summary>
        /// 地平以下6度的时角(太阳?)
        /// </summary>
        public double H1 { get; set; }    // = 0;



        /// <summary>
        /// 节气的儒略日
        /// </summary>
        public double pe1 { get; set; }    // = 0;

        /// <summary>
        /// 节气的儒略日
        /// </summary>
        public double pe2 { get; set; }    // = 0;


        /// <summary>
        /// 值等于 Count 属性(只读)
        /// </summary>
        public int Length => this.Count;

        #endregion



        #region 构造函数

        public LunarInfoListT()
        {
        }


        /// <summary>
        /// 构造函数, 添加指定数目的元素到本类中, 并赋初值
        /// </summary>
        /// <param name="itemsCount">要添加的元素个数</param>
        /// <param name="initValue">元素的初值(泛型)</param>
        public LunarInfoListT(int itemsCount, T initValue)
        {
            for (int i = 0; i < itemsCount; i++)
            {
                this.Add(initValue);
            }
        }
        
        #endregion

    }


}
