using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlsunView.Infrastructure.Util
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// 调用一个对象实例的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static object InvokeMethod<T>(T instance, string methodName, object[] values)
        {
            var methodInfo = instance.GetType().GetMethod(methodName);
            if(methodInfo != null)
            {
                //获取方法参数信息
                var paramInfo = methodInfo.GetParameters();
                object[] paramObject = new object[paramInfo.Length];
                int i = 0;
                foreach (var e in paramInfo)
                {
                    if (i < values.Length)
                    {
                        paramObject[i] = Convert.ChangeType(values[i], e.ParameterType);
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                return methodInfo.Invoke(instance, paramObject);
            }
            return null;
        }
    }
}
