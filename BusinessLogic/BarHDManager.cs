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
    public class BarHDManager
    {
        protected string BaseURL { get; set; }
        protected string UserName { get; set; }
        protected string Password { get; set; }
        protected string Customer { get; set; }

        public BarHDManager(string url, string company, string token, string cust)
        {
            BaseURL = url;
            UserName = company;
            Password = token;
            Customer = cust;
        }

        public string GenerateParametes(JObject model)
        {
            if (string.IsNullOrEmpty(model["QAMT_CARGOCODE"].ToString()))
                model["QAMT_CARGOCODE"] = 1;

            if (string.IsNullOrEmpty(model["QAMT_SHIPNUMHOUSE"].ToString()))
                model["QAMT_SHIPNUMHOUSE"] = 1;

            var ordtype = model.Value<string>("TYPECODE");
            if (!string.IsNullOrEmpty(ordtype))
            {
                using (var db = new PharmYdmContext())
                {
                    var cust = db.BarQamaClients.FirstOrDefault(x => x.ShipCode == ordtype);
                    if (cust != null)
                    {
                        UserName = cust.Username;
                        Password = cust.Password;
                        Customer = cust.CustCode;
                    }
                }
            }

            var parameters =    $"username={UserName}&" +
                                $"password={Password}&" +
                                $"customernumber={Customer}&" +

                                $"tasknumber={model.Value<string>("QAMM_BARRE")}&" + //במקום REFERENCE
                                $"custtasknumber={model.Value<string>("QAMT_ORDERSONLINE1")}&" +
                                $"docreference={model.Value<string>("QAMT_ORDERSONLINE")}&" +
                                $"updseqno=1&" +
                                $"custname={model.Value<string>("QAMT_SHIPNAME")}&" +
                                $"deliverytype=2&" +
                                $"tasktypecode={model.Value<string>("QAMT_CARGOCODE")}&" +
                                //$"taskStatus=3&" +
                                //$"fromHour=1000&" +
                                //$"toHour=1100&" +
                                $"cityname={model.Value<string>("QAMT_SHIPSTATE")}&" +
                                $"streetname={model.Value<string>("QAMT_SHIPSTREET")}&" +
                                $"housenumber={model.Value<string>("QAMT_SHIPNUMHOUSE")}&" +
                                $"telno1={model.Value<string>("QAMT_SHIPHONE")}&" +
                                $"windowdate={model.Value<DateTime>("CURDATE").ToString("ddMMyyyy")}";

            return parameters;
        }

        public async Task<string> CreateShip(string parameters)
        {
            var action = "ords/ws/newtask/pst";

            var request = BuildPostRequest(action);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(parameters);
            }

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

                        var status = doc.GetElementsByTagName("errorcode")[0].InnerText;
                        var shipnum = doc.GetElementsByTagName("request_num")[0].InnerText;

                        if (status == "OK")
                        {
                            return shipnum;
                        }
                        else
                        {
                            errorMsg = $"{shipnum} | {status}" ;
                            throw new Exception();
                        }

                    }
                    catch (Exception)
                    {
                        throw new Exception(errorMsg);
                    }
     
                }
            }
        }

        //public string GetCreateShipUrl(string parameters)
        //{
        //    var action = "ship_create_anonymous";

        //    var request = BuildGetRequest(action, parameters);

        //    return request.Address.AbsoluteUri;
        //}

        //public async Task<dynamic> GetShipDetail(string id)
        //{
        //    var action = "ship_status_xml";

        //    var parameters = $"-N20048669,-A";

        //    var request = BuildGetRequest(action, parameters);

        //    using (WebResponse response = await request.GetResponseAsync())
        //    {
        //        using (StreamReader stream = new StreamReader(response.GetResponseStream()))
        //        {
        //            var xml = stream.ReadToEnd();
        //            XmlDocument doc = new XmlDocument();
        //            doc.LoadXml(xml);
        //            string json = JsonConvert.SerializeXmlNode(doc);
        //            var user = JsonConvert.DeserializeObject<dynamic>(json);
        //            return user;
        //        }
        //    }
        //}

        //public string GetShipStickerUrl(string id)
        //{
        //    var action = "ship_print_ws";

        //    var parameters = $"-N{id}";

        //    var request = BuildGetRequest(action, parameters);

        //    return request.Address.ToString();
        //}


        private HttpWebRequest BuildGetRequest(string action,string parameters)
        {
            var url = $"{BaseURL}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            //request.Headers.Add("Authorization", $"Bearer {Token}");
            request.ContentType = "application/json";
            request.Method = "GET";

            return request;
        }
        private HttpWebRequest BuildPostRequest(string action)
        {
            var url = $"{BaseURL}/{action}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            //request.Headers.Add("authorization", $"Basic {Token}");
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            return request;
        }


        public class PriorityListResult<T>
        {
            public List<T> value { get; set; }
        }
    }
}