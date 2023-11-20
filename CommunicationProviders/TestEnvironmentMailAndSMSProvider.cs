using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmYdm.Providers.CommunicationProviders
{
    class TestEnvironmentMailAndSMSProvider : IMailSender, ISmsSender
    {
        public bool Send(string subject, string body, params string[] to)
        {
            var oneLineDesc = $"subj: {subject}, body: {body}, to: {string.Join(";", to)}";
            Debug.WriteLine(oneLineDesc);
            return true;
        }

        public bool Send(string content, params string[] to)
        {
            var oneLineDesc = $"content: {content}, to: {string.Join(";", to)}";
            Debug.WriteLine(oneLineDesc);
            return true;
        }

        public bool Send(string content, DateTime scheduler, params string[] to)
        {
            var oneLineDesc = $"content: {content}, schedule: {scheduler.ToString()} to: {string.Join(";", to)}";
            Debug.WriteLine(oneLineDesc);
            return true;
        }

        public bool SendWithPDF(string subject, string body, byte[] pdf, params string[] to)
        {
            var oneLineDesc = $"content: {body}, to: {string.Join(";", to)}";
            Debug.WriteLine(oneLineDesc);
            return true;
        }

        public void SetSender(string sender)
        {
            return;
        }
    }
}
