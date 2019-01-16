using GlsunView.Infrastructure.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GlsunView.Common
{
    public class LisenceHelper
    {
        public static string xmlPublicKey = "<RSAKeyValue><Modulus>tLxiwB9xY2+KWbrXYNGXSFV8ybzpE2vx9Xkk7Hm9N7V1bzOwkm5vQmCqv2TWw7aOh1mnjg6nG/e3AGrtirdFWryJyoLNSyp2gRkuCX2e3SUharFim+aqTuuwyBtcOWiIPZjXdHjZIL6JrfRz3IaiDbRiTipvWmmqoWxaksghY5U=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        public static string Type { get; set; }
        public static string MachineNumber { get; set; }
        public static string CreationDate { get; set; }
        public static string ExpiredDate { get; set; }
        public static string LocalMachineNumber { get; set; }
        public static bool IsLisenceFileExist { get; set; }
        public static string Message { get; set; }
        static LisenceHelper()
        {
            //解析Lisence
            //本机机器码
            string serialNo = WMIHelper.GetDiskSerialNumber() + WMIHelper.GetCPUSerialNumber();
            if (serialNo.Length >= 24)
            {
                LocalMachineNumber = serialNo.Substring(0, 24);
            }
            else
            {
                LocalMachineNumber = serialNo.PadRight(24 - serialNo.Length, '0');
            }
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lisence");
            if (File.Exists(fileName) == false)
            {
                IsLisenceFileExist = false;
                return;
            }
            IsLisenceFileExist = true;
            string resutl = "";
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                resutl = Encoding.UTF8.GetString(data);
            }
            string json = RSAHelper.PublicKeyDecrypt(xmlPublicKey, resutl);
            object objLicense = JsonConvert.DeserializeObject(json);
            JObject jobj = objLicense as JObject;
            Type = jobj["Type"].ToString();
            MachineNumber = jobj["MachineNumber"].ToString();
            ExpiredDate = jobj["ExpiredDate"].ToString();
            CreationDate = jobj["CreationDate"].ToString();
        }

        public static void ParseLisence()
        {
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lisence");
            if (File.Exists(fileName) == false)
            {
                IsLisenceFileExist = false;
                return;
            }
            IsLisenceFileExist = true;
            string resutl = "";
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                resutl = Encoding.UTF8.GetString(data);
            }
            string json = RSAHelper.PublicKeyDecrypt(xmlPublicKey, resutl);
            object objLicense = JsonConvert.DeserializeObject(json);
            JObject jobj = objLicense as JObject;
            Type = jobj["Type"].ToString();
            MachineNumber = jobj["MachineNumber"].ToString();
            ExpiredDate = jobj["ExpiredDate"].ToString();
            CreationDate = jobj["CreationDate"].ToString();
        }
        public static bool IsLisenceValid()
        {
            Message = "";
            if (IsLisenceFileExist)
            {
                if (LocalMachineNumber != MachineNumber)
                {
                    Message = "机器码不一致，请重新申请Lisence文件！";
                    return false;
                }
                if(DateTime.Now < DateTime.Parse(CreationDate))
                {
                    Message = "系统时间小于Lisence文件生成时间，请确保时间设置正确！";
                    return false;
                }
                //F:永久，L:期限
                if (Type == "L")
                {
                    if (DateTime.Now > DateTime.Parse(ExpiredDate))
                    {
                        Message = "Liscen已过期，请重新申请Lisence文件！";
                        return false;
                    }
                }
            }
            else
            {
                Message = "Liscen文件不存在！";
                return false;
            }
            return true;
        }
    }
}