using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PharmYdm.Providers.CommunicationProviders
{
    public class SmsMicropayProvider : ISmsSender
    {
        private const string SERVICE_URL = "http://www.micropay.co.il/ExtApi/ScheduleSms.php";
        private string FROM_NUMBER = "";
        private string UID = "";
        private string UN = "";
        private const string CHARSET = "utf-8";

        private string fromNumber;

        private string FromNumber { get { return !string.IsNullOrEmpty(fromNumber) ? fromNumber : FROM_NUMBER; } }


        public SmsMicropayProvider(string number, string uid, string un)
        {
            FROM_NUMBER = number;
            UID = uid;
            UN = un;
        }

        public void SetSender(string sender)
        {
            fromNumber = sender;
        }

        public bool Send(string content, params string[] to)
        {
            var hasContent = !string.IsNullOrEmpty(content);
            var toList = this._ValideateDestList(to);

            if (hasContent && toList.Count > 0)
            {
                return this._send(content, null, toList);
            }

            return false;

        }

        public bool Send(string content, DateTime scheduler, params string[] to)
        {
            var hasContent = !string.IsNullOrEmpty(content);
            var LaterDate = scheduler >= DateTime.Now;
            var toList = this._ValideateDestList(to);

            if (hasContent && LaterDate && toList.Count > 0)
            {
                return this._send(content, scheduler, toList);
            }

            return false;
        }

        private List<string> _ValideateDestList(params string[] list)
        {
            List<char> digits = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            List<string> toList = new List<string>();
            foreach (var item in list)
            {
                string getNumberOnly = string.Empty;
                foreach (var c in item.ToCharArray())
                {
                    if (digits.Contains(c))
                    {
                        getNumberOnly += c;
                    }
                }
                if (getNumberOnly.Length == 10)
                {
                    toList.Add(getNumberOnly);
                }
            }

            return toList;
        }

        private bool _send(string msg, DateTime? date, List<string> to)
        {
            try
            {
                var sendDateAsString = !date.HasValue ? "" :
                        string.Format("&dh={0}&di={1}&dy={2}&dm={3}&dd={4}",
                                        date.Value.ToString("HH"),
                                        date.Value.ToString("mm"),
                                        date.Value.ToString("yyyy"),
                                        date.Value.ToString("MM"),
                                        date.Value.ToString("dd")
                    );
                var url = string.Format("{0}?get=1&uid={1}&un={2}&msglong={3}&list={4}&charset={5}&from={6}{7}",
                                            SERVICE_URL, UID, UN, msg, string.Join(",",to), CHARSET, FROM_NUMBER, sendDateAsString);

                HttpWebRequest requerst = (HttpWebRequest)WebRequest.Create(url);
                requerst.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)requerst.GetResponse();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
