using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Positive.Models
{
    public class ServerResponse<T>
    {
        public ServerResponse()
        {
            List = new List<T>();
            _Errors = new Dictionary<string, string>();
        }

        public ServerResponse(T obj)
        {
            isList = false;
            Singel = obj;
            _Errors = new Dictionary<string, string>();
        }

        public ServerResponse(List<T> list)
        {
            isList = true;
            List = list;
            _Errors = new Dictionary<string, string>();
        }

        public static ServerResponse<bool> GetErrorInstance(string msg)
        {
            var response = new ServerResponse<bool>(false);
            response.AddError(msg);
            return response;
        }

        public void AddError(string value)
        {
            AddError(string.Empty, value);

        }

        public void AddError(string key, string value)
        {
            if (_Errors.Keys.Contains(key))
            {
                _Errors[key] += ' ' + value;
            }
            _Errors.Add(key, value);
        }

        public T Singel { get; set; }
        public List<T> List { get; set; }
        private Dictionary<string, string> _Errors { get; }

        public List<ErrorItem> Errors { get { return _Errors.Select(x => new ErrorItem(x.Key, x.Value)).ToList(); } }

        public bool isOk { get { return _Errors.Count == 0; } }
        public bool isList { get; set; }
    }

    public class ErrorItem
    {
        public ErrorItem(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class FileStoreResult
    {
        public string location { get; set; }
        public string url { get; set; }

    }
}