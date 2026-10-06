using MailKit.Net.Smtp;
using System.Collections.Specialized;
using System.Configuration;

namespace StockQuoteAlert.Mail
{
    public class MailClient
    {
        private readonly SmtpClient smtpClient;
        private readonly string fromName;
        private readonly string fromAddress;

        public MailClient()
        {
            NameValueCollection mailApiConfig = (NameValueCollection)ConfigurationManager.GetSection("mailApi");
            string host = mailApiConfig["host"];
            int port = int.Parse(mailApiConfig["port"]);
            string username = mailApiConfig["username"];
            string password = mailApiConfig["password"];

            this.smtpClient = new SmtpClient();
            this.smtpClient.Connect(host, port, false);
            this.smtpClient.Authenticate(username, password);
        }

        public void SendEmail(AlertMail alert)
        {
            this.smtpClient.Send(alert.GetMessage());
        }

        public void Disconnect()
        {
            this.smtpClient.Disconnect(true);
        }
    }
}
