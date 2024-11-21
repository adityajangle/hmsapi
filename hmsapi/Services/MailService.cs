using System;
using System.Net;
using System.Net.Mail;

namespace hmsapi.Services
{
	public class MailService
	{

        private static SmtpClient? smtpServer = null;
        private static MailMessage? msgMail = null;
        private NetworkCredential? credentials = null;

        public MailService()
        {
            this.credentials = new NetworkCredential("task@jjit.net", "jjittask@123");
        }
        public MailService(NetworkCredential credentials)
        {
            this.credentials = credentials;
        }

        //public static Dictionary<string, string> mail_msg = new Dictionary<string, string>();

        public static Dictionary<string, MailMessage> mail_dict = null;

        public MailMessage Mail_Msg(string key)
        {
            MailMessage mailMessage = new MailMessage();
            if (mail_dict.ContainsKey(key))
            {
                mailMessage.Subject = mail_dict[key].Subject;
                mailMessage.Body = mail_dict[key].Body;
                //var value = mail_dict[key];
                return mailMessage;
            }
            return null;
        }


        public void createMessage(string[] arrToAddresses, string body, string subject, string[]? cc = null, Attachment? arrFiles = null)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(credentials!.UserName);

            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;
            if (body == string.Empty & arrFiles != null)
                mailMessage.Body = "Please Find Attachment";
            else
                mailMessage.Body = body;
            foreach (string addr in arrToAddresses)
                mailMessage.To.Add(new MailAddress(addr));
            if (cc != null)
            {
                foreach (string addr in cc)
                    mailMessage.CC.Add(new MailAddress(addr));
                //mailMessage.CC.Add(new MailAddress(cc));
            }
            if (arrFiles != null)
            {
                //foreach (FileInfo tempFile in arrFiles)

                mailMessage.Attachments.Add(arrFiles);
            }
            try
            {
                smtpServer = new SmtpClient();
                smtpServer.UseDefaultCredentials = false;
                smtpServer.Credentials = new NetworkCredential("task@jjit.net", "jjittask@123");
                smtpServer.Port = 25;
                smtpServer.Host = "jmail.jjit.net";
                smtpServer.EnableSsl = false;
                smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtpServer.UseDefaultCredentials = false;
                smtpServer.Send(mailMessage);

            }
            catch (Exception ex)
            {
                //LogWriter.AppendLog("EmailSend:", ex.StackTrace);
            }


        }
    }
}

