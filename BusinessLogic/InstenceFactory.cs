using PharmYdm.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Positive.BusinessLogic
{
    public sealed class InstenceFactory
    {

        private static HfdManager hfd_instance { get; set; }
        private static BarHDManager bar_instance { get; set; }
        private static PriorityManager priority_instance { get; set; }


        public static HfdManager GetHfdManager()
        {
            if (hfd_instance == null)
            {
                var url = ConfigurationManager.AppSettings["hfd_url"];
                var user = ConfigurationManager.AppSettings["hfd_company"];
                var token = ConfigurationManager.AppSettings["hfd_token"];
                hfd_instance = new HfdManager(url,user,token);
            }
    
            return hfd_instance;
        }

        public static BarHDManager GetBarManager()
        {
            if (bar_instance == null)
            {
                var url = ConfigurationManager.AppSettings["bar_url"];
                var user = ConfigurationManager.AppSettings["bar_username"];
                var token = ConfigurationManager.AppSettings["bar_password"];
                var customer = ConfigurationManager.AppSettings["bar_customer"];
                bar_instance = new BarHDManager(url, user, token, customer);
            }

            return bar_instance;
        }

        public static PriorityManager GetPriorityManager()
        {
            if (priority_instance == null)
            {
                // Set Priority vars
                var p_url = ConfigurationManager.AppSettings["priority_url"];
                var p_comp = ConfigurationManager.AppSettings["priority_company"];
                var p_un = ConfigurationManager.AppSettings["priority_username"];
                var p_pass = ConfigurationManager.AppSettings["priority_password"];
                var appid = ConfigurationManager.AppSettings["priorityAppID"];
                var appkey = ConfigurationManager.AppSettings["priorityAppKey"];
                priority_instance = new PriorityManager(p_url, p_comp, p_un, p_pass, appid, appkey);
            }

            return priority_instance;
        }
    }
}