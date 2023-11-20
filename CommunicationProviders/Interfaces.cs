using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmYdm.Providers.CommunicationProviders
{
    public interface IMailSender
    {
        void SetSender(string sender);

        bool Send(string subject, string body, params string[] to);

        bool SendWithPDF(string subject, string body, byte[] pdf, params string[] to);
    }

    public interface ISmsSender
    {
        void SetSender(string sender);

        bool Send(string content, params string[] to);

        bool Send(string content, DateTime scheduler, params string[] to);
    }
}