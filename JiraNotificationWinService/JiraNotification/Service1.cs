using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace JiraNotification
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;) 
        string[] phones = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\Configuration\\PhoneNumbers.txt", Encoding.UTF8);
        string[] projects = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\Configuration\\QueueAPIs.txt", Encoding.UTF8);

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile(DateTime.Now + ": Service is started");
            CallToAction();
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 300000; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteToFile(DateTime.Now + ": Service is stopped");
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile(DateTime.Now + ": Service is recall");
            CallToAction();
        }

        private void CallToAction()
        {
            foreach( string project in projects)
            {
                string[] data = project.Split(',');
                BaseModel baseModel = GetQueueIssue(data[1], "", "");
                int countNewTickets = CountNewTickets(baseModel);
                if (countNewTickets > 0)
                {
                    string sendSMSLogs = DateTime.Now + ": Send SMS To Group, Project: " + data[0];
                    foreach (string phone in phones)
                    {
                        sendSMSLogs += ("\n" + SendSMS(phone, data[0]));
                    }
                    WriteToFile(sendSMSLogs);
                }
            }
        }

        private int CountNewTickets(BaseModel baseModel)
        {
            if (baseModel.Size > 0)
            {
                Value[] issues = baseModel.Values;
                int countNew = 0;
                foreach (Value issue in issues)
                {
                    DateTime createdDate = DateTime.Parse(issue.Fields.Created);
                    double totalSecond = DateTime.Now.Subtract(createdDate).TotalSeconds;
                    if (totalSecond < 600)
                    {
                        countNew++;
                    }
                }
                return countNew;
            }
            return 0;
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
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

        private static string SendSMS(string toPhone, string projectName)
        {
            const string accountSid = "";
            const string authToken = "";
            const string fromPhone = "";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: projectName + ": New Issue was created, please take action. From AMS with love",
                from: new Twilio.Types.PhoneNumber(fromPhone),
                to: new Twilio.Types.PhoneNumber(toPhone)
            );

            return message.Sid;
        }

        private static BaseModel GetQueueIssue(string url, string jUser, string jPassword)
        {
            HttpWebRequest requestObj = (HttpWebRequest)WebRequest.Create(url);
            requestObj.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(jUser + ":" + jPassword));
            try
            {
                HttpWebResponse responseObj = (HttpWebResponse)requestObj.GetResponse();
                string strResult = String.Empty;
                using (Stream stream = responseObj.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(stream);
                    strResult = streamReader.ReadToEnd();
                    streamReader.Close();
                }
                BaseModel result = JsonConvert.DeserializeObject<BaseModel>(strResult);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetQueueIssue: " + ex);
            }
            return null;
        }
    }
}
