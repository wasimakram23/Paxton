﻿using Paxton.Net2.OemClientLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PaxtonReportAPI.Models;
using PaxtonReportAPI.Utility;
using System.Threading.Tasks;
using PaxtonReportAPI.IRepository;

namespace PaxtonReportAPI.Repository
{
    public class PaxtonRepository:IPaxtonRepository
    {
        int PORT;
        string ip;
        string password;
        OemClient oemClient;
        Dictionary<int, string> users;
        Dictionary<string, int> net2Methods;
        string query;
        Common common;
        public PaxtonRepository()
        {
            PORT = AppConstant.getPort();
            ip = AppConstant.getIp();
            password = AppConstant.getPassword();
            oemClient = new OemClient(ip, PORT);
            common=new Common();
        }

        public Result getEvent()
        {

            Result result=new Result();
            try
            {
                int year = DateTime.Now.AddDays(-2).Year;
                int month = DateTime.Now.AddDays(-2).Month;
                int day = DateTime.Now.AddDays(-2).Day;
                string startDate = "" + year + "-" + month + "-" + day;
                query = "SELECT E.EventTime as 'Date/Time',u.Field12_50 as 'User Code',CONCAT(e.Surname,',',e.FirstName) as 'User Name',e.CardNumber as 'Token Number',e.PeripheralName as 'Where',e.EventTypeDescription as 'Event',e.EventDetails as 'Details' from EventsEx as E left join  UsersEx as U on e.UserID = u.UserID where CAST(EventTime AS DATE)< CAST(GETDATE() AS DATE) and CAST(EventTime AS DATE)> CONVERT(date, '" + startDate + "') order by e.EventTime desc";
                users = oemClient.GetListOfOperators().UsersDictionary();
                net2Methods = oemClient.AuthenticateUser(0, password);
                List<EventModel> eventList = new List<EventModel>();
                DataSet ds = oemClient.QueryDb(query);
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var eventInfo = new EventModel();
                    eventInfo.dateTime = dt.Rows[i][0].ToString() ?? "";
                    eventInfo.userCode = dt.Rows[i][1].ToString() ?? "";
                    eventInfo.userName = dt.Rows[i][2].ToString() ?? "";
                    eventInfo.tokenNumber = dt.Rows[i][3].ToString() ?? "";
                    eventInfo.where = dt.Rows[i][4].ToString() ?? "";
                    eventInfo.Event = dt.Rows[i][5].ToString() ?? "";
                    eventInfo.details = dt.Rows[i][6].ToString() ?? "";
                    eventList.Add(eventInfo);
                }
                result.status = true;
                result.List = eventList;
                return result;
            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = ex.Message;
                return result;
            }
        }
        public Result getEvent(string startDate,string endDate)
        {
            Result result = new Result();
            try
            {
                query = "SELECT E.EventTime as 'Date/Time',u.Field12_50 as 'User Code',CONCAT(e.Surname,',',e.FirstName) as 'User Name',e.CardNumber as 'Token Number',e.PeripheralName as 'Where',e.EventTypeDescription as 'Event',e.EventDetails as 'Details' from EventsEx as E left join  UsersEx as U on e.UserID = u.UserID where CAST(EventTime AS DATE)<=CONVERT(date,'" + endDate + "') and CAST(EventTime AS DATE)>= CONVERT(date, '" + startDate + "') order by e.EventTime desc";
                users = oemClient.GetListOfOperators().UsersDictionary();
                net2Methods = oemClient.AuthenticateUser(0, password);
                List<EventModel> eventList = new List<EventModel>();
                DataSet ds = oemClient.QueryDb(query);
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var eventInfo = new EventModel();
                    eventInfo.dateTime = dt.Rows[i][0].ToString() ?? "";
                    eventInfo.userCode = dt.Rows[i][1].ToString() ?? "";
                    eventInfo.userName = dt.Rows[i][2].ToString() ?? "";
                    eventInfo.tokenNumber = dt.Rows[i][3].ToString() ?? "";
                    eventInfo.where = dt.Rows[i][4].ToString() ?? "";
                    eventInfo.Event = dt.Rows[i][5].ToString() ?? "";
                    eventInfo.details = dt.Rows[i][6].ToString() ?? "";
                    eventList.Add(eventInfo);
                }
                result.status = true;
                result.List = eventList;
                return result;
            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = ex.Message;
                return result;
            }
        }
        public Result getAttendes(string startDate)
        {
            int user=0;
            Result result = new Result();
            int year = DateTime.Now.AddDays(-1).Year;
            int month = DateTime.Now.AddDays(-1).Month;
            int day = DateTime.Now.AddDays(-1).Day;
            if (String.IsNullOrEmpty(startDate))
                startDate = "" + year + "-" + month + "-" + day;
            try
            {
                query = "select distinct u.UserID,u.Field12_50 as 'UserCode' from EventsEx as e"
                        +" inner join UsersEx u on e.UserID = u.UserID where"
                        +" CAST(EventTime AS DATE) = CONVERT(date, '"+startDate+"')"
                        +" and e.PeripheralName like '%:Out' and u.Field12_50 is not null"
                        +" and e.UserID in ("
                        +" select distinct u.UserID from EventsEx as e"
                        +" inner join UsersEx u on e.UserID = u.UserID"
                        +" where CAST(EventTime AS DATE)= CONVERT(date, '"+startDate+"') and e.PeripheralName like '%:In')";
                users = oemClient.GetListOfOperators().UsersDictionary();
                net2Methods = oemClient.AuthenticateUser(0, password);
                List<Attendes> attendesList = new List<Attendes>();
                DataSet userSet = oemClient.QueryDb(query);
                DataTable userTable = userSet.Tables[0];
                var userList = from data in userTable.AsEnumerable()
                               select new
                               {
                                   userId = Convert.ToInt32(data.Field<object>("UserID")),
                                   userCode = Convert.ToString(data.Field<object>("UserCode"))
                               };
                Parallel.ForEach(userList, item =>
                 {
                     Attendes info = new Attendes();
                     info.date = startDate;
                     info.elployeeID = item.userCode;
                     Info dailyIn = getInfo(item.userId, startDate, "IN");
                     Info dailyOut = getInfo(item.userId, startDate, "OUT");
                     info.inTime = dailyIn.time;
                     info.inLocation = dailyIn.location;
                     info.outTime = dailyOut.time;
                     info.outLocation = dailyOut.location;
                     info.duration = common.getDuration(dailyIn.time, dailyOut.time);
                     if (!info.duration.Contains('-'))
                        attendesList.Add(info);
                     else
                        user += 1;
                     
                 });
                result.status = true;
                result.List = attendesList;
                result.message = "Valid:"+attendesList.Count+"Negetive:" + user+"Total:"+userList.Count();
                return result;
            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = ex.Message+" For user:"+user;
                return result;
            }
        }
        private Info getInfo(int userId,string date, string type)
        {
            var info=new Info();
            string query = "";
            if (type.Equals("IN"))
                query = "select top(1)e.EventTime,e.PeripheralName from EventsEx as e where e.PeripheralName like '%:In' and e.UserID = "+userId+" and CAST(e.EventTime AS DATE) = CONVERT(date, '"+date+"') order by e.EventTime asc";
            else
                query = "select top(1)e.EventTime, e.PeripheralName from EventsEx as e where e.PeripheralName like '%:Out' and e.UserID = " + userId + " and CAST(e.EventTime AS DATE) = CONVERT(date, '"+date+"') order by e.EventTime desc";
            DataSet ds = oemClient.QueryDb(query);
            DataTable dt = ds.Tables[0];
            info.time = dt.Rows[0][0].ToString();
            info.location = dt.Rows[0][1].ToString()??"";
            return info;
        }
    }

    internal class Info
    {
        public string time { get; set; }
        public string location { get; set; }
    }

}