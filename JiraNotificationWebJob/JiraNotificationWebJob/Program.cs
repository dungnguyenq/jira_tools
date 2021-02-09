using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;

namespace JiraNotificationWebJob
{
    class Program
    {
        private static IConfigurationRoot configuration = GetConfig();

        static void Main(string[] args)
        {
            Console.WriteLine("Jira Notification: Start...");
            string[] phoneNumbers = configuration["AppSetting:PhoneNumber"].Split("|");
            string[] projects = configuration["AppSetting:QueueAPIs"].Split("|");

            foreach (string project in projects)
            {
                string[] data = project.Split('+');
                BaseModel baseModel = GetQueueIssue(data[1], configuration["JiraAccount:Id"], configuration["JiraAccount:Token"]);
                int countNewTickets = CountNewTickets(baseModel);
                if (countNewTickets > 0)
                {
                    string sendSMSLogs = DateTime.Now + ": Send SMS To Group, Project: " + data[0];
                    foreach (string phone in phoneNumbers)
                    {
                        sendSMSLogs += ("\n" + SendSMS(phone, data[0]));
                    }
                    Console.WriteLine(sendSMSLogs);
                }
            }
            Console.WriteLine("Jira Notification: Stop...");
        }

        private static int CountNewTickets(BaseModel baseModel)
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

        private static string SendSMS(string toPhone, string projectName)
        {
            string accountSid = configuration["Twillio:AccountSid"];
            string authToken = configuration["Twillio:AuthToken"];
            string fromPhone = configuration["Twillio:FromPhone"];

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: projectName + ": New Issue was created, please take action. From AMS with love",
                from: new Twilio.Types.PhoneNumber(fromPhone),
                to: new Twilio.Types.PhoneNumber("+" + toPhone)
            );

            return message.Sid;
        }

        private static string MakeCall(string toPhone)
        {
            string accountSid = configuration["Twillio:AccountSid"];
            string authToken = configuration["Twillio:AuthToken"];
            string fromPhone = configuration["Twillio:FromPhone"];

            TwilioClient.Init(accountSid, authToken);

            var to = new PhoneNumber(toPhone);
            var from = new PhoneNumber(fromPhone);
            var call = CallResource.Create(to, from,
                url: new Uri("http://demo.twilio.com/docs/voice.xml"));

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

        private static IConfigurationRoot GetConfig()
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
