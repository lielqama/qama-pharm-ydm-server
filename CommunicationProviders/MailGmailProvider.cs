using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PharmYdm.Providers.CommunicationProviders
{
    class MailGmailProvider : IMailSender
    {
        private const string HOST = "smtp.gmail.com";
        private const int PORT = 587;
        private const bool SSL = true;

        private string defultUserName = "";
        private string password = "66yhbvft6";

        public void SetSender(string sender)
        {
            
        }

        public bool Send(string subject, string body, params string[] to)
        {
            var message = new MailMessage()
            {
                From = new MailAddress(defultUserName),
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            };

            foreach (var item in to)
            {
                if (!string.IsNullOrEmpty(item))
                    message.To.Add(new MailAddress(item));
            }

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = defultUserName,  // replace with valid value
                    Password = password  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = HOST;
                smtp.Port = PORT;
                smtp.EnableSsl = SSL;
                smtp.Send(message);
            }

            return true;
        }

        public bool SendWithPDF(string subject, string body, byte[] pdf, params string[] to)
        {
            var message = new MailMessage()
            {
                From = new MailAddress(defultUserName),
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            };

            var pdfAttachment = new Attachment(new MemoryStream(pdf), string.Format("plp-{0}.pdf", DateTime.Now.ToString("ddMMyyyy")), "application/pdf");
            message.Attachments.Add(pdfAttachment);

            foreach (var item in to)
            {
                if (!string.IsNullOrEmpty(item))
                    message.To.Add(new MailAddress(item));
            }

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = defultUserName,  // replace with valid value
                    Password = password  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = HOST;
                smtp.Port = PORT;
                smtp.EnableSsl = SSL;
                smtp.Send(message);
            }

            return true;
        }
    }
}
