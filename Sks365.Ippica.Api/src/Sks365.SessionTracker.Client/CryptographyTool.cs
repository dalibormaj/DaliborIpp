using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Sks365.SessionTracker.Client
{
    public static class CryptographyTool
    {
        // For test
        //string testEncrypt = CryptographyTool.EncryptMD5("Test", "1138D81B-F819-4416-A01D-46CB1BE71A85", true, false);
        //string testDecrypt = CryptographyTool.DecryptMD5(testEncrypt, "1138D81B-F819-4416-A01D-46CB1BE71A85", true, false);

        public static string DecryptMD5(string toDecrypt, string key, bool useHashing, bool UseBase64 = true)
        {
            byte[] keyArray;
            byte[] toEncryptArray;

            if (UseBase64 == true)
                toEncryptArray = Convert.FromBase64String(toDecrypt);
            else
                toEncryptArray = HexStringToByteArray(toDecrypt);

            if ((useHashing))
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        private static byte[] HexStringToByteArray(string toArray)
        {
            string payload = toArray.Split(',')?.FirstOrDefault();
            int strLenght = payload.Length;
            int upperBound = strLenght / 2;

            if (strLenght % 2 == 0)
                upperBound -= 1;
            else
                payload = "0" + payload;

            byte[] bytes = new byte[upperBound + 1];

            for (int i = 0; i <= upperBound; i++)
                bytes[i] = Convert.ToByte(payload.Substring(i * 2, 2), 16);

            return bytes;
        }

        public static string EncryptMD5(string toEncrypt, string key, bool useHashing, bool UseBase64 = true)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if ((useHashing))
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            byte[] resultArray;

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            if (UseBase64 == true)
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            else
            {
                var sb = new StringBuilder();
                foreach (byte item in resultArray)
                    sb.Append(item.ToString("X2"));
                return sb.ToString();
            }
        }
    }
}
