using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PaxtonReportAPI.Models;

namespace PaxtonReportAPI.IRepository
{
    public interface IPaxtonRepository
    {
        Result getEvent();
        Result getEvent(string startDate, string endDate);
        Result getAttendes(string startDate);
    }
}