using System;
using System.Collections.Generic;
using System.Text;

namespace SharpSxwnl
{
    /// <summary>
    /// List&lt;T&gt; 的派生类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class xList<T> : List<T>
    {
        #region 公共属性

        /// <summary>
        /// 值等于 Count 属性(只读)
        /// </summary>
        public int Length => this.Count;

        #endregion
       
    }
}
