using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;

namespace SharpSxwnl;

/// <summary>
/// 纪年表数据
/// </summary>
public class JnbArrayList : ArrayList    // 由于存在多种数据类型, 故派生于 ArrayList, 需要装箱和拆箱操作
{
    /// <summary>
    /// 构造函数, 完成纪年表数据的加载
    /// </summary>
    public JnbArrayList()
    {

        //----------------------------------------------------------------------------------------
        // 加载 Xml 数据:  历史纪年表
        // 注: 加载时自动去除历史纪年表 Xml 数据中所有的空白字符
        //----------------------------------------------------------------------------------------
        if (LunarHelper.SxwnlXmlData != null)
        {
            const string JnbXPath = "SharpSxwnl/SxwnlData/Data[@Id = 'obb_JNB']";

            XmlNode foundNode;
            Regex regexToTrim = new Regex(@"\s*");    // C#: 匹配任何空白字符, 用于去除所有空白字符
            int i;

            // 读取并解开历史纪年表
            foundNode = LunarHelper.SxwnlXmlData.SelectSingleNode(JnbXPath);
            if (foundNode != null)
            {
                string[] JNB = regexToTrim.Replace(foundNode.InnerText, "").Split(',');

                this.AddRange(JNB);
                for (i = 0; i < JNB.Length; i += 7)
                {
                    this[i] = int.Parse((string)(this[i]));
                    this[i + 1] = int.Parse((string)(this[i + 1]));
                    this[i + 2] = int.Parse((string)(this[i + 2]));
                }
            }
        }
    }

}