using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaxtonReportAPI.Models
{
    public class EventModel
    {
        public string dateTime { get; set; }
        public string userCode { get; set; }
        public string userName { get; set; }
        public string tokenNumber { get; set; }
        public string where { get; set; }
        public string Event{ get; set; }
        public string details { get; set; }

    }
}