using AMCustomerImportInspector.Interface;
using log4net;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace AMCustomerImportInspector.Service
{
    public class EmailService : IEmailService
    {

        private ILog _LogHandler;

        public EmailService(ILog logHandler)
        {
            _LogHandler = logHandler;
        }

        public void SendEmailToRecipient(string emailAddress, string messageSubject, string messageBody, string fullFileName)
        {
            var userName = ConfigurationManager.AppSettings["EMailUser"];
            var password = ConfigurationManager.AppSettings["EMailPassword"];
            var host = ConfigurationManager.AppSettings["EmailURL"];
            var port = int.Parse(ConfigurationManager.AppSettings["EmailPort"]);
            MailMessage mail = new MailMessage(userName, emailAddress, messageSubject, messageBody);

            try
            {
                mail.Attachments.Add(new Attachment(fullFileName));
                SmtpClient client = new SmtpClient(host, port);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(userName, password);
                client.EnableSsl = true;
                client.Send(mail);
                _LogHandler.Debug("Mail Has been Sent");
            }
            catch(Exception excp)
            {
                _LogHandler.Fatal(excp.Message);
            }
            finally
            {
                _LogHandler.Debug("Mail Disposing");
                mail.Dispose();
                _LogHandler.Debug("Mail Disposed");
            }
        }
    }
}
