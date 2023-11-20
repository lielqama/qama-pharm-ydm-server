using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Positive.BusinessLogic
{
    public class RecoverHfdDataManager
    {
        public List<HfdRecoverDataModel> GetDataFromStikerDirectory(string directory)
        {
            var result = new List<HfdRecoverDataModel>();

            var allFiles = Directory.GetFiles(directory);
            foreach (var file in allFiles)
            {
                var converted = GetDataFromStiker(file);
                result.Add(converted);
            }

            return result;
        }
        public HfdRecoverDataModel GetDataFromStiker(string filename, bool isFullPath = false)
        {
            string path = isFullPath ? System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, filename) : filename;
            //using (PdfReader reader = new PdfReader(path))
            //{
            //    var _data = PdfTextExtractor.GetTextFromPage(reader, 1);
            //    var converted = ParseDataFromStiker(_data);
            //    converted.StikerName = filename;
            //    return converted;
            //}
            return new HfdRecoverDataModel();
        }
        private HfdRecoverDataModel ParseDataFromStiker(string data)
        {
            var result = new HfdRecoverDataModel();

            var takeApart = this.SpiltBySpaceAndNewline(data);

            foreach (var item in takeApart)
            {
                if (item.Length == 7)
                    result.YanshufID = item;

                if (item.Length == 8)
                    result.HfdID = item;
            }

            return result;
        }
        private List<string> SpiltBySpaceAndNewline(string s)
        {
            var separators = new String[2];
            separators[0] = " ";
            separators[1] = "\n";
            var takeAPart = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return takeAPart.Where(x => IsDigitsOnly(x)).ToList();
        }
        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }

    public class HfdRecoverDataModel
    {
        public string ORDNAME { get; set; }
        public string HfdID { get; set; }
        public string YanshufID { get; set; }
        public string StikerName { get; set; }

        public string PriorityFilePatch { get {
                var fileName = StikerName.Split('\\').Last();
                var priorityPrefix = "..\\..\\system\\mail\\website\\";
                return priorityPrefix + fileName;
        } }
    }

}