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
    public class PriorityManager
    {
        protected string BaseURL { get; set; }
        public string Company { get; protected set; }
        protected string Token { get; set; }
        protected string AppID { get; set; }
        protected string AppKey { get; set; }
        protected string Tabula { get; set; } = "tabula.ini";

        public PriorityManager(string url, string company, string username, string password, string appId, string appKey)
        {
            BaseURL = url;
            Company = company;

            var creds = username + ":" + password;
            var bcreds = Encoding.ASCII.GetBytes(creds);
            var base64Creds = Convert.ToBase64String(bcreds);
            Token = base64Creds;

            AppID = appId;
            AppKey = appKey;
        }

        public async Task<List<dynamic>> GetOrderByID(List<string> ids)
        {
            var PRIORITY_ID_NAME = "QAMT_ORDERSONLINE";
            var GROUP_SIZE = 25;
            var result = new List<dynamic>();

            var pointer = 0;
            while (pointer < ids.Count)
            {
                var currentIds = ids.Skip(pointer).Take(GROUP_SIZE);

                var filter = string.Join(" or ", currentIds.Select(x => $"{PRIORITY_ID_NAME} eq '{x}'"));
                var select = "ORDNAME,QAMT_HDFREFERENCE," + PRIORITY_ID_NAME;

                var items = await this.GetListAsync<dynamic>("ORDERS", GROUP_SIZE, filter, select);
                result.AddRange(items);

                pointer += GROUP_SIZE;
            }

            return result;
        }


        public async Task<T> GetSingelAsync<T>(string screen, string id, string select = null, string expand = null)
        {
            screen = $"{screen.ToUpper()}({id})";

            var parameters = new List<string>();

            if (!string.IsNullOrEmpty(select))
                parameters.Add($"$select={select}");

            if (!string.IsNullOrEmpty(expand))
                parameters.Add($"$expand={expand}");

            var action = parameters.Count == 0 ? screen : $"{screen}?{string.Join("&", parameters)}";

            var request = BuildGetRequest(action);

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    var result = stream.ReadToEnd();
                    T user = JsonConvert.DeserializeObject<T>(result);
                    return user;
                }
            }
        }
        public async Task<List<T>> GetListAsync<T>(string screen, int top = 100, string filter = null, string select = null, string expand = null)
        {
            screen = $"{screen.ToUpper()}";

            var parameters = new List<string>();
            if (top > 0)
                parameters.Add($"$top={top}");

            if (!string.IsNullOrEmpty(filter))
                parameters.Add($"$filter={filter}");

            if (!string.IsNullOrEmpty(select))
                parameters.Add($"$select={select}");

            if (!string.IsNullOrEmpty(expand))
                parameters.Add($"$expand={expand}");

            var action = parameters.Count == 0 ? screen : $"{screen}?{string.Join("&", parameters)}";

            var request = BuildGetRequest(action);

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    var result = stream.ReadToEnd();
                    return JsonConvert.DeserializeObject<PriorityListResult<T>>(result).value;
                }
            }
        }
        public async Task<T> UpdateItemAsync<T>(string screen, string id, object _data)
        {
            var action = $"{screen.ToUpper()}({id})";
            var request = BuildPatchRequest(action);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string data = JsonConvert.SerializeObject(_data);
                streamWriter.Write(data);
            }

            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                    {
                        var result = stream.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(result);
                    }
                }
            }
            catch (WebException ex)
            {
                throw new Exception(PriorityErrorHandler(ex));
            }
        }
        public async Task<T> AddItemAsync<T>(string screen, object _data)
        {
            var action = $"{screen.ToUpper()}";
            var request = BuildPostRequest(action);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string data = JsonConvert.SerializeObject(_data);
                streamWriter.Write(data);
            }

            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                    {
                        var result = stream.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(result);
                    }
                }
            }
            catch (WebException ex)
            {
                throw new Exception(PriorityErrorHandler(ex));
            }
        }

        private string PriorityErrorHandler(WebException ex)
        {
            string messageFromServer = null;

            if (ex.Response == null && !string.IsNullOrEmpty(ex.Message))
                return ex.Message;

            var response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
            if (statusCode == HttpStatusCode.NotFound)
            {
                return "404";
            }
            if (statusCode == HttpStatusCode.Conflict)
            {
                return "409";
            }
            else if (statusCode == HttpStatusCode.Forbidden)
            {
                messageFromServer = response;
            }
            else if (string.IsNullOrEmpty(response))
            {
                messageFromServer = ex.Message;
            }
            else if (response[0] == '{') //json
            {
                var json = JToken.Parse(response);
                try
                {
                    messageFromServer = json["error"].Value<string>("message");

                    try
                    {
                        var inner = json["error"]["innererror"].Value<string>("message");
                        if (!string.IsNullOrEmpty(inner))
                        {
                            messageFromServer += inner;
                        }
                    }
                    catch (Exception) { };

                }
                catch (Exception)
                {
                    try
                    {
                        messageFromServer = json.Value<JToken>("FORM").Value<JToken>("InterfaceErrors").Value<string>("text");
                    }
                    catch (Exception)
                    {
                        try
                        {
                            var erros = json.Value<JToken>("FORM").Value<JArray>("InterfaceErrors").Select(x => x.Value<string>("text"));
                            messageFromServer = string.Join(",", erros);

                        }
                        catch (Exception) { }
                    }
                }
            }
            else if (response[0] == '<') //xml
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                XmlNode currentItem = doc.FirstChild;
                while (currentItem != null && string.IsNullOrEmpty(messageFromServer))
                {
                    messageFromServer = currentItem.InnerText;
                    currentItem = currentItem.FirstChild;
                }
            }


            if (string.IsNullOrEmpty(messageFromServer))
                messageFromServer = "שגיאה לא ידועה";

            return messageFromServer;
        }




        protected HttpWebRequest BuildGetRequest(string action)
        {
            return BuildRequest(action, "GET");

        }
        protected HttpWebRequest BuildPostRequest(string action)
        {
            return BuildRequest(action, "POST");

        }
        protected HttpWebRequest BuildPatchRequest(string action)
        {
            return BuildRequest(action, "PATCH");

        }
        protected HttpWebRequest BuildDeleteRequest(string action)
        {

            return BuildRequest(action, "DELETE");
        }

        protected HttpWebRequest BuildRequest(string action, string method)
        {
            if (string.IsNullOrEmpty(Tabula))
                Tabula = "tabula.ini";
            var url = $"{BaseURL}/odata/Priority/{Tabula}/{Company}/{action}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Headers.Add("authorization", $"Basic {Token}");
            request.Headers.Add("X-App-Id", AppID);
            request.Headers.Add("X-App-Key", AppKey);
            request.ContentType = "application/json";
            request.Method = method;

            return request;
        }

        public class PriorityListResult<T>
        {
            public List<T> value { get; set; }
        }
    }
}