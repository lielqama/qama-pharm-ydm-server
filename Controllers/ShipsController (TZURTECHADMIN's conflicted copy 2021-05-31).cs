using HFD.Attributes;
using HFD.BusinessLogic;
using HFD.Context;
using HFD.Models;
using HFD.Models.Enums;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Positive.BusinessLogic;
using Positive.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HFD.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Ships")]
    public class ShipsController : ApiController
    {
        HfdManager hfd = InstenceFactory.GetHfdManager();
        BarHDManager bar = InstenceFactory.GetBarManager();
        PriorityManager priority = InstenceFactory.GetPriorityManager();

        [HttpGet,Route("CreateShip")]
        public async Task<IHttpActionResult> CreateShip(string o)
        {
            string shipParameters = "";
            string priorityIDForLog = $"{priority.Company} \\ {o}";
            try
            {
                var select = "STCODE,QAMT_CARGOCODE,QAMT_SHIPNAME,QAMT_SHIPSTATE,QAMT_SHIPSTREET,QAMT_SHIPNUMHOUSE,QAMT_SHIPHONE,REFERENCE,ORDNAME,QAMT_DATESLOT,QAMO_DELIVERY,QAMT_ORDERSONLINE,QAMT_CARGOBACK,QAMO_SPEC11,TOTQUANT";
                var model = await this.priority.GetSingelAsync<dynamic>("ORDERS", o, select);

                shipParameters = this.hfd.GenerateParametes(model);
                var createShip = await this.hfd.CreateShip(shipParameters);
                var stikerUrl = this.hfd.GetShipStickerUrl(createShip);

                var stiker = this.hfd.GetStreamFromUrl(stikerUrl);
                var stikerTitle = "Hfd מדקבה.pdf";
                ServerResponse<FileStoreResult> priortyFile = await this.StoreFileInServer(stiker, stikerTitle);

                var attacmentList = new List<dynamic>();
                attacmentList.Add(new { 
                        EXTFILENAME = priortyFile.Singel.location.Replace("c:\\priority","..\\.."), 
                        EXTFILEDES = stikerTitle 
                });

                var priorityUpdateVm = new
                {
                    QAMT_HDFREFERENCE = createShip,
                    EXTFILES_SUBFORM = attacmentList
                };

                await this.priority.UpdateItemAsync<dynamic>("ORDERS", $"'{o}'", priorityUpdateVm);

                this.CreateLogRecord(priorityIDForLog, createShip, stikerUrl, shipParameters);

                return Ok(createShip);

            }
            catch (Exception ex)
            {

                var error = ex.Message;
                try
                {
                    var priorityUpdateVm = new
                    {
                        QAMT_HDFERROR = ex.Message
                    };
                   await this.priority.UpdateItemAsync<dynamic>("ORDERS", $"'{o}'", priorityUpdateVm);

                }
                catch (Exception ex2) { error = ex2.Message; };

                this.CreateErrorLogRecord(priorityIDForLog, error, shipParameters);

                return Ok(error);
            }

        }


        [HttpGet, Route("CreateShipBar")]
        public async Task<IHttpActionResult> CreateShipBar(string o)
        {
            string shipParameters = "";
            string priorityIDForLog = $"{priority.Company} \\ {o}";
            try
            {
                var select = "STCODE,QAMT_CARGOCODE,QAMT_SHIPNAME,QAMT_SHIPSTATE,QAMT_SHIPSTREET,QAMT_SHIPNUMHOUSE,QAMT_SHIPHONE,REFERENCE,ORDNAME,QAMT_DATESLOT,QAMO_DELIVERY,QAMM_BARRE,QAMT_CARGOBACK,TYPECODE,QAMT_ORDERSONLINE,QAMT_ORDERSONLINE1";
                var model = await this.priority.GetSingelAsync<dynamic>("ORDERS", o, select);

                shipParameters = this.bar.GenerateParametes(model);
                var createShip = await this.bar.CreateShip(shipParameters);

                var priorityUpdateVm = new
                {
                    QAMT_HDFREFERENCE = createShip,
                    ORDSTATUSDES = "התקבל מBAR"
                };

                await this.priority.UpdateItemAsync<dynamic>("ORDERS", $"'{o}'", priorityUpdateVm);

                this.CreateLogRecord(priorityIDForLog, createShip, "", shipParameters);

                return Ok(createShip);

            }
            catch (Exception ex)
            {

                var error = ex.Message;
                try
                {
                    var priorityUpdateVm = new
                    {
                        QAMT_HDFERROR = ex.Message
                    };
                    await this.priority.UpdateItemAsync<dynamic>("ORDERS", $"'{o}'", priorityUpdateVm);

                }
                catch (Exception ex2) { error = ex2.Message; };

                this.CreateErrorLogRecord(priorityIDForLog, error, shipParameters);

                return Ok(error);
            }

        }


        private void CreateLogRecord(string priorityID, string hfdID,string stikerUrl,string parameters)
        {
            var vm = new HFD_Log()
            {
                Date = DateTime.Now,
                HfdID = hfdID,
                PriorityID = priorityID,
                Parameters = parameters,
                Stiker = stikerUrl
            };

            using (var db = new HFDContext())
            {
                db.HFD_Log.Add(vm);
                db.SaveChanges();

            }
        }
        private void CreateErrorLogRecord(string priorityID, string hfdError, string parameters)
        {
            var vm = new HFD_Log()
            {
                Date = DateTime.Now,
                PriorityID = priorityID,
                Parameters = parameters,
                Error = hfdError,
                Success = false
            };

            using (var db = new HFDContext())
            {
                db.HFD_Log.Add(vm);
                db.SaveChanges();
            }
        }
        protected async Task<ServerResponse<FileStoreResult>> StoreFileInServer(MemoryStream inputStream, string name)
        {
            var actionUrl = "https://hyanshuf.wee.co.il/PriorityFileUploader/api/Upload";
            HttpContent fileStreamContent = new StreamContent(inputStream);

            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(fileStreamContent, "file1", name);
                    var response = await client.PostAsync(actionUrl, formData);
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var res = (await response.Content.ReadAsStringAsync()).ToString();
                    return JsonConvert.DeserializeObject<ServerResponse<FileStoreResult>>(res);
                }
            }
        }

    }
}
