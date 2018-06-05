using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GlsunView.Infrastructure.Util
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExpansionMethod
    {
        /// <summary>
        /// 转换成匿名类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T ConvertToOther<T>(this object obj, T t)
        {
            return (T)obj;
        }

        /// <summary>
        /// 深度克隆
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object DeepClone(this object obj)
        {
            object ret = null;
            //将对象序列化成二进制流
            BinaryFormatter binFormatIn = new BinaryFormatter();
            MemoryStream memStream;
            using (memStream = new MemoryStream())
            {
                binFormatIn.Serialize(memStream, obj);
                //流的位置已经到结尾了，所以设置到开头以便反序列化
                memStream.Seek(0, SeekOrigin.Begin);
                ret = binFormatIn.Deserialize(memStream);
            }
            return ret;
        }
        /// <summary>
        /// 复制同类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T CopyProperty<T>(this T obj)
        {
            var copy = Activator.CreateInstance<T>();
            var type = obj.GetType();
            foreach(var p in copy.GetType().GetProperties())
            {
                var propIn = type.GetProperty(p.Name);
                if(propIn != null)
                {
                    p.SetValue(copy, propIn.GetValue(obj));
                }
            }
            return copy;
        } 
    }
}
