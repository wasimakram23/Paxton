using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paxton.Net2.OemClientLibrary;
using Syncfusion.XlsIO;
using System.Data;
using System.IO;

namespace PaxtonReportService
{
    class PaxtonReport
    {
        const int PORT = 8025;
        string reportPath = @"C:\PaxtonReport\Report";
        string errorPath = @"C:\PaxtonReport\Error";
        OemClient oemClient;
        Dictionary<int, string> users;
        Dictionary<string, int> net2Methods;
        public void GenerateReport()
        {
            try
            {
                int year = DateTime.Now.AddDays(-2).Year;
                int month = DateTime.Now.AddDays(-2).Month;
                int day = DateTime.Now.AddDays(-2).Day;

                string startDate = "" + year + "-" + month + "-" + day;
                oemClient = new OemClient("10.10.17.245", PORT); ;
                users = oemClient.GetListOfOperators().UsersDictionary();
                net2Methods = oemClient.AuthenticateUser(0, "net1412");
                string query = "SELECT E.EventTime as 'Date/Time',u.Field12_50 as 'User Code',CONCAT(e.Surname,',',e.FirstName) as 'User Name',e.CardNumber as 'Token Number',e.PeripheralName as 'Where',e.EventTypeDescription as 'Event',e.EventDetails as 'Details' from EventsEx as E left join  UsersEx as U on e.UserID = u.UserID where CAST(EventTime AS DATE)< CAST(GETDATE() AS DATE) and CAST(EventTime AS DATE)> CONVERT(date, '" + startDate + "') order by e.EventTime desc";
                DataSet ds = oemClient.QueryDb(query);
                DataTable dt = ds.Tables[0];
                ExportToExcel(dt);
            }
            catch(Exception ex)
            {
                WriteToFile("Error occured at:" + DateTime.Now.ToString());
                WriteToFile("Error description:\n" + ex.ToString());
            }            

        }

        private void ExportToExcel(DataTable dt)
        {
            string path = @"C:\PaxtonReport\Report\EventData_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".xlsx";
            try
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    //Set the default application version as Excel 2016
                    excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2016;

                    //Create a workbook with a worksheet
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);

                    //Access first worksheet from the workbook instance
                    IWorksheet worksheet = workbook.Worksheets[0];
                    int Totalcol = 1;
                    foreach (DataColumn col in dt.Columns)
                    {
                        worksheet.Range[1, Totalcol++].Text = col.ColumnName;
                    }
                    for (int i = 1; i <= dt.Rows.Count; i++)
                    {
                        for (int j = 1; j <= dt.Columns.Count; j++)
                        {
                            worksheet.Range[i + 1, j].Text = dt.Rows[i - 1][j - 1].ToString() ?? "";
                        }
                    }
                    //Save the workbook to disk in xlsx format
                    workbook.SaveAs(path);
                }
            }
            catch(Exception ex)
            {
                WriteToFile("Error occured at:" + DateTime.Now.ToString());
                WriteToFile("Error description:\n" + ex.ToString());
            }
        }
        private void WriteToFile(string Message)
        {
            
            string filepath = errorPath + "\\ErrorLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
