using AMCustomerImportInspector.Interface;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace AMCustomerImportInspector.Service
{
    public class EmailService : IEmailService
    {
        public void SendEmailToRecipient(string emailAddress, string messageSubject, string messageBody, string fullFileName)
        {
            var userName = ConfigurationManager.AppSettings["EMailUser"];
            var password = ConfigurationManager.AppSettings["EMailPassword"];
            var host = ConfigurationManager.AppSettings["EmailURL"];
            var port = int.Parse(ConfigurationManager.AppSettings["EmailPort"]);

            try
            {
                MailMessage mail = new MailMessage(userName, emailAddress, messageSubject, messageBody);
                mail.Attachments.Add(new Attachment(fullFileName));
                SmtpClient client = new SmtpClient(host, port);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(userName, password);
                client.EnableSsl = true;
                client.Send(mail);
            }
            catch(Exception excp)
            {
                //TODO:Raise Log event
            }
        }
    }
}
