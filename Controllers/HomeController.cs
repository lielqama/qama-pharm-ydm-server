using PharmYdm.BusinessLogic;
using PharmYdm.Context;
using PharmYdm.Models;
using PharmYdm.Providers.CommunicationProviders;
using Positive.BusinessLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PharmYdm.Controllers
{
    public class HomeController : Controller
    {
        HfdManager hfd = InstenceFactory.GetHfdManager();
        BarHDManager bar = InstenceFactory.GetBarManager();
        PriorityManager priority = InstenceFactory.GetPriorityManager();


        public ActionResult Index()
        {
            ViewBag.Title = "Hfd log";

            return View();
        }

        public ActionResult _GetLog(DateTime start, DateTime end, bool? success = null)
        {
            using (var db = new PharmYdmContext())
            {
                var query = db.PharmYdm_Log.Where(x => x.Date >= start && x.Date <= end);
                if (success.HasValue)
                {
                    query = query.Where(x => x.Success == success.Value);
                }

                return PartialView(query.ToList());
            }
        }

        [HttpPost]
        public ActionResult _ParseParameters(string p)
        {
            var data = p.Split(',').ToList();
            if (data.Count < 2)
            {
                data = p.Split('&').ToList();
                return View("_ParseParametersBar", data);
            }
            data.Add("---");
            var fullUrl = InstenceFactory.GetHfdManager().GetCreateShipUrl(p);
            ViewBag.FullUrl = fullUrl;
            return View(data);
        }

        [HttpGet, Route("RecoverStikeData")]
        public ActionResult RecoverStikeData()
        {
            var dir = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "recoverFiles");
            var allFiles = Directory.GetFiles(dir);

            ViewBag.Count = allFiles.Count();
            return View();
        }

        [HttpGet, Route("_RecoverStikeData")]
        public async Task<ActionResult> _RecoverStikeData()
        {
            try
            {
                var recoverManger = new RecoverHfdDataManager();
                var stikerTitle = "Hfd מדקבה.pdf";


                var dir = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "recoverFiles");
                var data = recoverManger.GetDataFromStikerDirectory(dir);

                var allIds = data.Select(x => x.YanshufID).ToList();
                var orders = await priority.GetOrderByID(allIds);

                var okItems = 0;
                var errors = new List<string>();
                foreach (var item in data)
                {
                    var order = orders.FirstOrDefault(x => x["QAMT_ORDERSONLINE"].ToString() == item.YanshufID);
                    if (order != null)
                    {
                        var o = order["ORDNAME"].ToString();

                        var attacmentList = new List<dynamic>();
                        attacmentList.Add(new
                        {
                            EXTFILENAME = item.PriorityFilePatch,
                            EXTFILEDES = stikerTitle
                        });

                        var priorityUpdateVm = new
                        {
                            QAMT_HDFREFERENCE = item.HfdID,
                            EXTFILES_SUBFORM = attacmentList
                        };

                        try
                        {
                            await this.priority.UpdateItemAsync<dynamic>("ORDERS", $"'{o}'", priorityUpdateVm);
                            okItems++;
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"הזמנה {o} לא שוחזרה: {ex.Message}");
                        }

                    }

                }

                var result = new RecoverResult()
                {
                    OkItems = $"{okItems} מדבקות שוחזרו בהצלחה",
                    ErrorItems = $"{errors.Count} מדבקות נכשלו בשחזור",
                    Errors = errors
                };

                return View(result);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }

        }


        [HttpGet, Route("SendSmsFromList")]
        public async Task<ActionResult> SendSmsFromList(string filename = "custOrders.xls")
        {
            try
            {
                var manager = new SMSSendManager();
                var dir = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "recoverFiles");
                var path = $"{dir}\\{filename}";

                var items = manager.GetDataFromExcel(path);

                var smsManger = CommunicationFactory.GetSmsInstance();
                foreach (var item in items)
                {
                    var content = $"{item.Item1}, אתר שופרסל Gift מאחל לך חג שמח. הזמנתך התקבלה, הועברה לחברת המשלוחים ותצא להפצה בימים הקרובים.\n" +
                                  $"תשלח הודעה על מועד האספקה.\n" +
                                  $"חג שמח";
                    var phone = item.Item2;

                    smsManger.Send(content, phone);
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }

        }


        [HttpGet, Route("BarHotfix")]
        public async Task<ActionResult> BarHotfix()
        {

            using (var db = new PharmYdmContext())
            {
                var today = DateTime.Today;
                var logs = db.PharmYdm_Log.Where(x => x.Date >= today).ToList();

                var result = new List<Tuple<string, string>>();

                foreach (var log in logs)
                {
                    var o = log.PriorityID.Split(' ').LastOrDefault();
                    if(!string.IsNullOrEmpty(o))
                    {
                        string shipParameters = "";
                        string priorityIDForLog = $"{priority.Company} \\ {o}";
                        try
                        {
                            var select = "STCODE,ORDSTATUSDES,TYPECODE,QAMT_CARGOCODE,QAMT_SHIPNAME,QAMT_SHIPSTATE,QAMT_SHIPSTREET,QAMT_SHIPNUMHOUSE,QAMT_SHIPHONE,REFERENCE,ORDNAME,QAMT_DATESLOT,QAMO_DELIVERY,QAMM_BARRE,QAMT_CARGOBACK";
                            var model = await this.priority.GetSingelAsync<dynamic>("ORDERS", o, select);

                            string status = $"{model["TYPECODE"]}";
                            if (string.IsNullOrEmpty(status) || status != "BO")
                                continue;

                            shipParameters = this.bar.GenerateParametes(model);
                            var createShip = await this.bar.CreateShip(shipParameters);

                            result.Add(new Tuple<string, string>(o, createShip));

                            this.CreateLogRecord(priorityIDForLog, createShip, "", shipParameters);
                        }
                        catch (Exception ex)
                        {

                            var error = ex.Message;
        
                            this.CreateErrorLogRecord(priorityIDForLog, error, shipParameters);

                            result.Add(new Tuple<string, string>(o, error));

                            //return Ok(error);
                        }
                    }
                }


                return View(result);

            }


        }


        private void CreateLogRecord(string priorityID, string hfdID, string stikerUrl, string parameters)
        {
            var vm = new PharmYdm_Log()
            {
                Date = DateTime.Now,
                HfdID = hfdID,
                PriorityID = priorityID,
                Parameters = parameters,
                Stiker = stikerUrl
            };

            using (var db = new PharmYdmContext())
            {
                db.PharmYdm_Log.Add(vm);
                db.SaveChanges();

            }
        }
        private void CreateErrorLogRecord(string priorityID, string hfdError, string parameters)
        {
            var vm = new PharmYdm_Log()
            {
                Date = DateTime.Now,
                PriorityID = priorityID,
                Parameters = parameters,
                Error = hfdError,
                Success = false
            };

            using (var db = new PharmYdmContext())
            {
                db.PharmYdm_Log.Add(vm);
                db.SaveChanges();
            }
        }
    }

    public class RecoverResult
    {
        public string OkItems { get; set; }
        public string ErrorItems { get; set; }
        public List<string> Errors { get; set; }
    }
}
