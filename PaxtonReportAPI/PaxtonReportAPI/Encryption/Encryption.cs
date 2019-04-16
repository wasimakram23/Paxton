using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace PaxtonReportAPI.Encryption
{
    public class EncryptionMD5
    {
        public string EncryptUsernamePassword(string clearText)
        {
            // TODO: Parameterize the Password, Salt, and Iterations.  They should be encrypted with the machine key and stored in the registry
            if (string.IsNullOrEmpty(clearText))
            {
                return clearText;
            }

            byte[] salt = { 0xA9, 0x9B, 0xC8, 0x32, 0x56, 0x35, 0xE3, 0x03 };

            // NOTE: The keystring, salt, and iterations must be the same as what is used in the Demo java system.
            PKCSKeyGenerator crypto = new PKCSKeyGenerator("ezeon8547432gte4hd3fhh", salt, 19, 1);

            ICryptoTransform cryptoTransform = crypto.Encryptor;
            var cipherBytes = cryptoTransform.TransformFinalBlock(Encoding.UTF8.GetBytes(clearText), 0, clearText.Length);
            return Convert.ToBase64String(cipherBytes);
        }
        public string DecryptUsernamePassword(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }

            byte[] salt = { 0xA9, 0x9B, 0xC8, 0x32, 0x56, 0x35, 0xE3, 0x03 };

            // NOTE: The keystring, salt, and iterations must be the same as what is used in the Demo java system.
            PKCSKeyGenerator crypto = new PKCSKeyGenerator("ezeon8547432gte4hd3fhh", salt, 19, 1);

            ICryptoTransform cryptoTransform = crypto.Decryptor;
            var cipherBytes = Convert.FromBase64String(cipherText);
            var clearBytes = cryptoTransform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(clearBytes);
        }
    }
}