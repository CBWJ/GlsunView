using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GlsunView.CommService
{
    /// <summary>
    /// 通信数据解析类
    /// </summary>
    public static class InstructionHelper
    {
        /// <summary>
        /// 从单个指令结果获取数据
        /// </summary>
        /// <param name="input"></param>
        /// <param name="instruction"></param>
        /// <returns></returns>
        public static string ExtractData(string input, string instruction)
        {
            string ret = "";
            var noBlank = @"_[\S]*\]";      //匹配非空白字符
            string pattern = @"\[" + instruction + noBlank;
            var matches = Regex.Matches(input, pattern);
            if (matches.Count > 0)
            {
                ret = matches[0].Value;
                ret = ret.Replace("[", "").Replace("]", "").Replace(instruction + "_", "");
            }
            return ret;
        }
        /// <summary>
        /// 从多个指令结果获取数据
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="instruction"></param>
        /// <returns></returns>
        public static string ExtractDataFromSet(string dataSet, string instruction)
        {
            string ret = "";
            var pattern = @"\[" + instruction + @"_([^\[\]]*)\]";
            var match = Regex.Match(dataSet, pattern);
            if (match.Success)
            {
                ret = ExtractData(match.Value, instruction);
            }
            return ret;
        }
    }
}
