using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCustomerImportInspector.Interface
{
    public interface IEmailService
    {
        void SendEmailToRecipient(string emailAddress, string messageSubject, string messageBody, string fullFileName);
    }
}
