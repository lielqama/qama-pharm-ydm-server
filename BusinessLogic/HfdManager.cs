using PharmYdm.Context;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace PharmYdm.BusinessLogic
{
    public class HfdManager
    {
        protected string BaseURL { get; set; }
        protected string Company { get; set; }
        protected string Token { get; set; }

        protected string Company_Default { get; set; }
        protected string Token_Default { get; set; }

        public HfdManager(string url, string company, string token)
        {
            BaseURL = url;
            Company_Default = Company = company;
            Token_Default = Token  =  token;
        }

        public string GenerateParametes(JToken model)
        {
            if (string.IsNullOrEmpty(model.Value<string>("QAMT_P4")))
                model["QAMT_P4"] = "4";

            if (string.IsNullOrEmpty(model.Value<string>("QAMT_P5")))
                model["QAMT_P5"] = "אינטרנט";
            if (!string.IsNullOrEmpty(model.Value<string>("QAMT_RETURNBOX")) && model.Value<string>("QAMT_RETURNBOX") == "0")
                model["QAMT_RETURNBOX"] = "";


            var parameters = $"-N{Company},-A{model["QAMT_P2"]},-N{model["QAMT_P3"]},-N{model["QAMT_P4"]},-A{model["QAMT_P5"]}," +
                             $"-A,-N{model["QAMT_P7"]},-N{model["QAMT_P8"]},-N{model["QAMT_RETURNBOX"] },-N," +
                             $"-A{model["CDES"]},-A,-A{model["QAMT_STATE"]},-A,-A{model["QAMT_SHIPSTREET"]}," +
                             $"-A{model["QAMT_SHIPNUMHOUSE"]},-A,-A,-A,-A{model["QAMT_PHONE"]}," +
                             $"-A,-A{model["DOCNO"]},-A{model["QAMT_PACKNUMBER"]},-A,-A," +
                             $"-A{model["ORDNAME"]},-A{model["QAMT_PLACINGDATE"]},-A,-N,-N," +
                             $"-N,-A,-A,-N,-N," +
                             $"-A{"XML"},-A,-A,-N";

            return parameters;
        }

        public async Task<string> CreateShip(string parameters)
        {
            var action = "ship_create_anonymous";

            var request = BuildGetRequest(action, parameters);

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    string errorMsg = "שגיאה בהמרת נתונים";
                    try
                    {
                        var xml = stream.ReadToEnd();
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(xml);
                        string json = JsonConvert.SerializeXmlNode(doc);
                        var data = JsonConvert.DeserializeObject<dynamic>(json);

                        var error = data["root"]["mydata"]["answer"]["ship_create_error"]["#cdata-section"].ToString();
                        if (!string.IsNullOrEmpty(error))
                        {
                            errorMsg = error;
                            throw new Exception();
                        }

                        string shipnum = data["root"]["mydata"]["answer"]["ship_create_num"]["#cdata-section"].ToString();

                        return shipnum;
                    }
                    catch (Exception)
                    {
                        throw new Exception(errorMsg);
                    }
     
                }
            }
        }

        public string GetCreateShipUrl(string parameters)
        {
            var action = "ship_create_anonymous";

            var request = BuildGetRequest(action, parameters);

            return request.Address.AbsoluteUri;
        }

        public async Task<dynamic> GetShipDetail(string id)
        {
            var action = "ship_status_xml";

            var parameters = $"-N20048669,-A";

            var request = BuildGetRequest(action, parameters);

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    var xml = stream.ReadToEnd();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    string json = JsonConvert.SerializeXmlNode(doc);
                    var user = JsonConvert.DeserializeObject<dynamic>(json);
                    return user;
                }
            }
        }

        public string GetShipStickerUrl(string id)
        {
            var action = "ship_print_ws";

            var parameters = $"-N{id}";

            var request = BuildGetRequest(action, parameters);

            return request.Address.ToString();
        }

        public MemoryStream GetStreamFromUrl(string url)
        {
            byte[] fileData = null;

            using (var wc = new System.Net.WebClient())
            {
                wc.Headers.Add("Authorization", $"Bearer {Token}");
                fileData = wc.DownloadData(url);
            }

            return new MemoryStream(fileData);
        }


        private HttpWebRequest BuildGetRequest(string action,string parameters)
        {
            var url = $"{BaseURL}?APPNAME=run&PRGNAME={action}&ARGUMENTS={parameters}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Headers.Add("Authorization", $"Bearer {Token}");
            request.ContentType = "application/json";
            request.Method = "GET";

            return request;
        }
        private HttpWebRequest BuildPostRequest(string action)
        {
            var url = $"{BaseURL}/{Company}/{action}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Headers.Add("authorization", $"Basic {Token}");
            request.ContentType = "application/json";
            request.Method = "POST";

            return request;
        }
        private HttpWebRequest BuildPatchRequest(string action)
        {
            var url = $"{BaseURL}/{Company}/{action}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Headers.Add("authorization", $"Basic {Token}");
            request.ContentType = "application/json";
            request.Method = "PATCH";

            return request;
        }

        public class PriorityListResult<T>
        {
            public List<T> value { get; set; }
        }
    }
}