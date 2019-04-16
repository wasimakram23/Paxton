using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.IO;

namespace PaxtonReportService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        string path = @"C:\PaxtonReport";
        string reportPath = @"C:\PaxtonReport\Report";
        string errorPath = @"C:\PaxtonReport\Error";
        string logPath = @"C:\PaxtonReport\Log";
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            PaxtonReport pr = new PaxtonReport();
            createDirectoryStructure();
            WriteToFile("Service is started at " + DateTime.Now);
            pr.GenerateReport();
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 1000 * 60 * 60 * 24;
            //timer.Interval = 1000 * 60 * 5;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
        protected void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            PaxtonReport pr = new PaxtonReport();
            createDirectoryStructure();
            WriteToFile("Service is recall at " + DateTime.Now);
            pr.GenerateReport();
            
        }
        private void WriteToFile(string Message)
        {
            if (!Directory.Exists(path))
            {
                createDirectoryStructure();
            }
            string filepath = logPath + "\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
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
        protected void createDirectoryStructure()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Directory.CreateDirectory(reportPath);
                Directory.CreateDirectory(errorPath);
                Directory.CreateDirectory(logPath);
            }
        }
    }
}
