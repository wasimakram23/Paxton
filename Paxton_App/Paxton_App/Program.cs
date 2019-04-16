using Paxton.Net2.OemClientLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel=Microsoft.Office.Interop.Excel;
using Syncfusion.XlsIO;

namespace Paxton_App
{
    class Program
    {
        static void Main(string[] args)
        {
            const int PORT = 8025;
            int year = DateTime.Now.AddDays(-2).Year;
            int month = DateTime.Now.AddDays(-2).Month;
            int day = DateTime.Now.AddDays(-2).Day;
            
            string startDate = "" + year + "-" + month + "-" +day;
            string path = @"E:\Wasim\Document\Paxton\EventData_3_17_2019.xlsx";
            OemClient oemClient= new OemClient("10.10.17.245", PORT); ;
            Dictionary<int, string> users = oemClient.GetListOfOperators().UsersDictionary();
            Dictionary<string, int> net2Methods = oemClient.AuthenticateUser(0, "net1412");
            int count = 0;
            string query = "SELECT E.EventTime as 'Date/Time',u.Field12_50 as 'User Code',CONCAT(e.Surname,',',e.FirstName) as 'User Name',e.CardNumber as 'Token Number',e.PeripheralName as 'Where',e.EventTypeDescription as 'Event',e.EventDetails as 'Details' from EventsEx as E left join  UsersEx as U on e.UserID = u.UserID where CAST(EventTime AS DATE)< CONVERT(date, '2019-03-17') and CAST(EventTime AS DATE)> CONVERT(date, '2019-03-15') order by e.EventTime desc";
            DataSet ds = oemClient.QueryDb(query);
            DataTable dt = ds.Tables[0];
            DataTableReader reader = dt.CreateDataReader();
            foreach (DataColumn col in dt.Columns)
            {
                Console.Write(col.ColumnName+" ");
            }
            Console.WriteLine();
            while (reader.Read())
            {
                //Console.WriteLine("Total column found:"+reader.VisibleFieldCount);
                for (int i= 0; i <reader.VisibleFieldCount; i++)
                {
                    Console.Write("" + reader[i].ToString()+",");
                }
                Console.WriteLine();
                count++;
                if (count == 100)
                    break;
            }
            //string path = @"E:\Wasim\Document\Paxton\Test.csv";
            Console.WriteLine("Write Started:" + DateTime.Now.ToString());
            //ExportDataSetToExcel(ds);
            ExportToExcel(dt,path);
            Console.WriteLine("Write Ended:" + DateTime.Now.ToString());
            //ToCSV(dt, path);
            Console.WriteLine("Done");
            

        }

        static void ToCSV(System.Data.DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
        static void ExportDataSetToExcel(DataSet ds)
        {
            string oldPath = @"E:\Wasim\Org.xlsx";
            string newPath = @"E:\Wasim\Org5.xlsx";
            System.IO.File.Copy(oldPath, newPath);
            //Creae an Excel application instance
            Excel.Application excelApp = new Excel.Application();

            //Create an Excel workbook instance and open it from the predefined location
            Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(newPath);
            

            foreach (DataTable table in ds.Tables)
            {
                //Add a new worksheet to workbook with the Datatable name
                
                Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();
                //excelWorkBook.Sheets.Delete();
                excelWorkSheet.Name = table.TableName;

                for (int i = 1; i < table.Columns.Count + 1; i++)
                {
                    excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;
                }

                for (int j = 0; j < table.Rows.Count; j++)
                {
                    for (int k = 0; k < table.Columns.Count; k++)
                    {
                        excelWorkSheet.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString()??"";
                    }
                }
            }

            excelWorkBook.Save();
            excelWorkBook.Close();
            excelApp.Quit();
            

        }
        static void ExportToExcel(DataTable dataTable,string path)
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
                foreach (DataColumn col in dataTable.Columns)
                {
                    worksheet.Range[1, Totalcol++].Text = col.ColumnName;
                }
                for (int i = 1; i <= dataTable.Rows.Count; i++)
                {
                    for (int j = 1; j <= dataTable.Columns.Count; j++)
                    {
                        worksheet.Range[i+1, j].Text = dataTable.Rows[i - 1][j - 1].ToString()??"";
                    }
                }
                //Save the workbook to disk in xlsx format
                workbook.SaveAs(path);
            }

        }


    }
}
