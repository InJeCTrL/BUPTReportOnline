using MimeKit;
using System;
using MailKit.Net.Smtp;
using MimeKit.Text;

namespace BUPTReportOnline.Utils
{
    public class MailHelper
    {
        public static void SendEMail(string GUID, string EMail, bool LastResult, string LastMessage, string LastTime)
        {
            var mailMsg = new MimeMessage();
            mailMsg.From.Add(MailboxAddress.Parse(GlobalConfig.SrcEMail));
            mailMsg.To.Add(MailboxAddress.Parse(EMail));
            mailMsg.Subject = GlobalConfig.EMailSubject;
            var content = GlobalConfig.EDMContent;
            content = content.Replace("$GUID$", GUID);
            content = content.Replace("$LastTime$", LastTime);
            content = content.Replace("$LastMessage$", LastMessage);
            content = content.Replace("$frontend-prefix$", GlobalConfig.FrontEndPrefix);
            if (LastResult)
            {
                content = content.Replace("$head-background-color$", "lightgreen");
            }
            else
            {
                content = content.Replace("$head-background-color$", "palevioletred");
            }
            mailMsg.Body = new TextPart(TextFormat.Html)
            {
                Text = content
            };
            var client = new SmtpClient();
            client.Connect("smtp.mxhichina.com", 465, true);
            client.Authenticate(GlobalConfig.SrcEMail, GlobalConfig.SrcEMailPWD);
            try
            {
                client.Send(mailMsg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
