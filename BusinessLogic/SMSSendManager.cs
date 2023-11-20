using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;

namespace Positive.BusinessLogic
{
    public class SMSSendManager
    {
        public List<Tuple<string,string>> GetDataFromExcel(string path)
        {
            var result = new List<Tuple<string, string>>();

            var connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", path);
            var adapter = new OleDbDataAdapter("SELECT * FROM [orders$]", connectionString);
            var ds = new DataSet();

            adapter.Fill(ds, "orders");

            var data = ds.Tables["orders"].AsEnumerable();

            foreach (var x in data)
            {
                try
                {
                    var name = x.Field<dynamic>("cust");
                    var phone = x.Field<dynamic>("phone");

                    result.Add(new Tuple<string, string>(name, phone));
                }
                catch (Exception) { }
            }

            return result;
        }
    }


}