using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaxtonReportAPI.Utility
{
    public class Common
    {
        public string getDuration(string inTime, string outTime)
        {
            DateTime st = getDate(inTime);
            DateTime et = getDate(outTime);
            TimeSpan span = et.Subtract(st);
            return "" + span.Hours + ":" + span.Minutes + ":" + span.Seconds;
        }
        public DateTime getDate(string datetime)
        {
            try
            {
                bool isPM = datetime.Contains("PM");
                datetime = datetime.Replace(" PM", "");
                datetime = datetime.Replace(" AM", "");
                string[] parts = datetime.Split(' ');
                string[] date = parts[0].Split( '/', ' ');
                string[] time = parts[1].Split(':');
                int hour = (isPM) ? Convert.ToInt16(time[0]) + 12 : Convert.ToInt16(time[0]);
                DateTime result = new DateTime(
                    Convert.ToInt16(date[2]),
                    Convert.ToInt16(date[0]),
                    Convert.ToInt16(date[1]),
                    (hour==24)?hour/2:hour,
                    Convert.ToInt16(time[1]),
                    Convert.ToInt16(time[2]));
                return result;
            }
            catch
            {
                return DateTime.Now;
            }
            
        }
    }
}