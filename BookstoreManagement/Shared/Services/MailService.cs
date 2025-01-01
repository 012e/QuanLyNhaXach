using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.Shared.Services;

public class MailService
{
    private readonly SmtpClient smtpClient;

    public MailService(SmtpClient smtpClient)
    {
        this.smtpClient = smtpClient;
    }

    public void Send(string targetEmail, string subject, string content)
    {
        MailMessage message = MakeMessage(targetEmail, subject, content);
        smtpClient.Send(message);
    }

    public void SendAsync(string targetEmail, string subject, string content)
    {
        MailMessage message = MakeMessage(targetEmail, subject, content);
        smtpClient.SendAsync(message, null);
    }

    private static MailMessage MakeMessage(string targetEmail, string subject, string content)
    {
        var message = new MailMessage();
        message.From = new MailAddress("MS_7M17HY@trial-0r83ql3z0rx4zw1j.mlsender.net");
        message.Subject = subject;
        message.Body = content;
        message.To.Add(new MailAddress(targetEmail));
        return message;
    }
}
