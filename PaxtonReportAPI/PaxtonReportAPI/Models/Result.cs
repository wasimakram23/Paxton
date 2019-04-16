using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaxtonReportAPI.Models
{
    public class Result
    {
        public bool status { get; set; }
        public object List { get; set; }
        public string message { get; set; }
    }
}