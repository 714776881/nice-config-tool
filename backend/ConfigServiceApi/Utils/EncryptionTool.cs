using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ConfigServiceApi.Utils
{
    internal class EncryptionTool
    {
        public static string EncryptWithDES(string data, string desKey, string desIv)
        {
            using (var des = new DESCryptoServiceProvider())
            {
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                des.Key = Encoding.UTF8.GetBytes(desKey);
                des.IV = Encoding.UTF8.GetBytes(desIv);

                var encryptBytes = Encoding.UTF8.GetBytes(data);
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptBytes, 0, encryptBytes.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        // 生成签名
        public static string GenerateSign(string orgCode, string orgAuthCode, string encryptedData)
        {
            string signSource = $"{orgCode}{orgAuthCode}{encryptedData}";
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(signSource);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        // 先进行UrlEncode加密 ，后进行Base64加密
        public static string Encrypt_UrlEncode_Base64(string data)
        {
            // Step 1: URL encode the data
            string encodedData = HttpUtility.UrlEncode(data);

            // Step 2: Base64 encode the URL-encoded data
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(encodedData);
            string base64EncodedData = Convert.ToBase64String(bytesToEncode);

            return base64EncodedData;
        }
        // Base64 加密
        public static string Encrypt_Base64(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(bytes);
        }
        // UrlEncode 加密
        public static string UrlEncode_Encrypt(string data)
        {
            return HttpUtility.UrlEncode(data);
        }
        // Md5 加密
        public static string Md5_Encrypt(string data)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] md5Bytes = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Bytes.Length; i++)
            {
                sb.Append(md5Bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
        // Sha1 加密
        public static string Sha1_Encrypt(string str)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] sha1Bytes = sha1.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < sha1Bytes.Length; i++)
            {
                sb.Append(sha1Bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
        public static string Aes_Encrypt(string str, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] iv = Encoding.UTF8.GetBytes(key);
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.IV = iv;
            rm.Mode = CipherMode.CBC;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ct = rm.CreateEncryptor();
            byte[] resultBytes = ct.TransformFinalBlock(strBytes, 0, strBytes.Length);
            return Convert.ToBase64String(resultBytes);
        }
    }
}
