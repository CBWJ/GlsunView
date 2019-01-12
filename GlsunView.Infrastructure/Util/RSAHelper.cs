using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GlsunView.Infrastructure.Util
{
    public class RSAHelper
    {
        /// <summary>
        /// 用私钥给数据进行RSA加密
        /// </summary>
        /// <param name="xmlPrivateKey"> 私钥(XML格式字符串)</param>
        /// <param name="strEncryptString"> 要加密的数据 </param>
        /// <returns> 加密后的数据 </returns>
        public static string PrivateKeyEncrypt(string xmlPrivateKey, string strEncryptString)
        {
            //加载私钥
            RSACryptoServiceProvider privateRsa = new RSACryptoServiceProvider();
            privateRsa.FromXmlString(xmlPrivateKey);

            //转换密钥
            AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetKeyPair(privateRsa);
            IBufferedCipher c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding"); //使用RSA/ECB/PKCS1Padding格式
                                                                                   //第一个参数为true表示加密，为false表示解密；第二个参数表示密钥

            c.Init(true, keyPair.Private);
            byte[] DataToEncrypt = Encoding.UTF8.GetBytes(strEncryptString);
            byte[] outBytes = c.DoFinal(DataToEncrypt);//加密
            string strBase64 = Convert.ToBase64String(outBytes);

            return strBase64;
        }
        /// <summary>
        /// 用公钥给数据进行RSA解密 
        /// </summary>
        /// <param name="xmlPublicKey"> 公钥(XML格式字符串) </param>
        /// <param name="strDecryptString"> 要解密数据 </param>
        /// <returns> 解密后的数据 </returns>
        public static string PublicKeyDecrypt(string xmlPublicKey, string strDecryptString)
        {
            //加载公钥
            RSACryptoServiceProvider publicRsa = new RSACryptoServiceProvider();
            publicRsa.FromXmlString(xmlPublicKey);
            RSAParameters rp = publicRsa.ExportParameters(false);

            //转换密钥
            AsymmetricKeyParameter pbk = DotNetUtilities.GetRsaPublicKey(rp);

            IBufferedCipher c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");
            //第一个参数为true表示加密，为false表示解密；第二个参数表示密钥
            c.Init(false, pbk);

            byte[] DataToDecrypt = Convert.FromBase64String(strDecryptString);
            byte[] outBytes = c.DoFinal(DataToDecrypt);//解密

            string strDec = Encoding.UTF8.GetString(outBytes);
            return strDec;
        }
    }
}
