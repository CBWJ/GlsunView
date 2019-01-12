using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace GlsunView.Infrastructure.Util
{
    /// <summary>
    /// WMI获取硬件信息
    /// </summary>
    public class WMIHelper
    {
        public static string GetDiskSerialNumber()
        {
            return GetClassPropertyValue("Win32_LogicalDisk", "VolumeSerialNumber");
        }
        public static string GetCPUSerialNumber()
        {
            return GetClassPropertyValue("Win32_Processor", "Processorid");
        }

        private static string GetClassPropertyValue(string className, string propName)
        {
            string value = "";
            ManagementClass managementClass = new ManagementClass(className);
            ManagementObjectCollection objCollection = managementClass.GetInstances();
            foreach (ManagementObject obj in objCollection)
            {
                value = obj.Properties[propName].Value.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                    break;
            }
            return value;
        }
    }
}
