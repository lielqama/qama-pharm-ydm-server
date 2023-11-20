using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PharmYdm.Providers.CommunicationProviders
{
    public class MailLocalhost : IMailSender
    {
        private const string HOST = "localhost";
        private const int PORT = 25;
        private const bool SSL = false;

        private string _sender = "NoReplay@afpelp.com";

        public void SetSender(string sender)
        {
            _sender = sender;
        }

        public bool Send(string subject, string body, params string[] to)
        {
            var message = new MailMessage()
            {
                From = new MailAddress(_sender),
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
                From = new MailAddress(_sender),
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
                smtp.Host = HOST;
                smtp.Port = PORT;
                smtp.EnableSsl = SSL;
                smtp.Send(message);
            }

            return true;
        }
    }
}
