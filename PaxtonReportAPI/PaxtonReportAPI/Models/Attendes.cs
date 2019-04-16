using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaxtonReportAPI.Models
{
    public class Attendes
    {
        public string elployeeID { get; set; }
        public string date { get; set; }
        public string inTime { get; set; }
        public string outTime { get; set; }
        public string duration { get; set; }
        public string inLocation { get; set; }
        public string outLocation { get; set; }
    }
}