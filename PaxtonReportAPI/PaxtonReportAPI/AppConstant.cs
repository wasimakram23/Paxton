using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Diagnostics;
using System.Collections;
using PaxtonReportAPI.Encryption;

namespace PaxtonReportAPI
{
    public static class AppConstant
    {
        public static int getPort()
        {
            string port = (string)((IDictionary)(ConfigurationManager.GetSection("PaxtonPort")))["value"];
            return Convert.ToInt16(port);
        }
        public static string getIp()
        {
            string ip= (string)((IDictionary)(ConfigurationManager.GetSection("PaxtonIp")))["value"];
            return ip;
        }
        public static string getPassword()
        {
            EncryptionMD5 crypto = new EncryptionMD5();
            string password= (string)((IDictionary)(ConfigurationManager.GetSection("Password")))["value"];
            return crypto.DecryptUsernamePassword(password);
        }
    }
}