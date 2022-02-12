using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using APIsASManager.CoreV3.Models;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace APIsASManager.CoreV3.Controllers
{
    public class RakindaController : Controller
    {
        Models.EFModels.TALogContext dbTALogContext;
        Models.RakindaModel rakindaModel = new RakindaModel();
        public RakindaController(Models.EFModels.TALogContext pContext)
        {
            dbTALogContext = pContext;
        }
        public IActionResult Index()
        {
            ViewData["index"] = "return from db";
            InitialSearchModel();
            rakindaModel.ListPageModel = new List<Models.EFModels.VIEW_RECORD_DATA>();
            DbGetRecordData(rakindaModel.Search);
            return View(rakindaModel);
        }
        public IActionResult IndexTemperatureRecord()
        {
            ViewData["index"] = "return from db";
            InitialSearchModel();
            rakindaModel.ListTemperatureRecord = new List<Models.EFModels.VIEW_TEMPERATURE_RECORD>();
            DbGetTemperatureRecord(rakindaModel.Search);
            return View(rakindaModel);
        }
        public IActionResult IndexHeartBreak()
        {
            ViewData["index"] = "return from db";
            InitialSearchModel();
            rakindaModel.ListHeartBreakModel = new List<Models.EFModels.HEART_BREAK>();
            DbGetHeartBreakData(rakindaModel.Search);
            return View(rakindaModel);
        }
        public IActionResult IndexCheckOpenAuth()
        {
            ViewData["index"] = "return from db";
            InitialSearchModel();
            rakindaModel.ListCheckOpenAuthen = new List<Models.EFModels.CHECK_OPEN_AUTH>();
            DbGetCheckOpenAuthData(rakindaModel.Search);
            return View(rakindaModel);
        }
        public IActionResult ExceptionData()
        {
            ViewData["index"] = "return from db";
            InitialSearchModel();
            rakindaModel.ListExceptionModel = new List<Models.EFModels.EXCEPTION>();
            DbGetExceptionData(rakindaModel.Search);
            //using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            //{
            //    rakindaModel.ListExceptionModel = vContext.EXCEPTIONS.OrderByDescending(x => x.created).ToList();
            //}
            return View(rakindaModel);
        }
        public IActionResult SearchRecordData(Models.RakindaModel.SearchClass pModel)
        {
            DbGetRecordData(pModel);
            return PartialView("/Views/Rakinda/_Index.cshtml", rakindaModel);
        }
        public IActionResult SearchTemperatureRecord(Models.RakindaModel.SearchClass pModel)
        {
            DbGetTemperatureRecord(pModel);
            return PartialView("/Views/Rakinda/_IndexTemperatureRecord.cshtml", rakindaModel);
        }
        public IActionResult SearchExceptionData(Models.RakindaModel.SearchClass pModel)
        {
            DbGetExceptionData(pModel);
            return PartialView("/Views/Rakinda/_ExceptionData.cshtml", rakindaModel);
        }
        public IActionResult SearchHeartBreakData(Models.RakindaModel.SearchClass pModel)
        {
            DbGetHeartBreakData(pModel);
            return PartialView("/Views/Rakinda/_IndexHeartBreak.cshtml", rakindaModel.ListHeartBreakModel);
        }
        public IActionResult SearchCheckOpenAuthData(Models.RakindaModel.SearchClass pModel)
        {
            DbGetCheckOpenAuthData(pModel);
            return PartialView("/Views/Rakinda/_IndexCheckOpenAuth.cshtml", rakindaModel.ListCheckOpenAuthen);
        }
        /// <summary>
        /// This is MVC, so use InputStream, If you use the web API, you can use the feature , fromBody, to receive
        /// </summary>
        [HttpPost]
        public ActionResult RecordData()
        {
            string json = ReadStream();
            try
            {
                RecordJson rj = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordJson>(json);
                if (rj != null)
                {
                    DbInsertRecordData(rj);
                }
            }
            catch(Exception exp)
            {
                DbInsertException("RecordData", exp);
            }
            return View();
        }
        [HttpPost]
        public ActionResult RecordDataBody(RecordJson pBody)
        {
            string json = ReadStream();
            try
            {
                RecordJson rj = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordJson>(json);
                if (rj != null)
                {
                    DbInsertRecordData(rj);
                }
                if(pBody != null)
                {
                    DbInsertRecordData(pBody);
                }
            }
            catch (Exception exp)
            {
                DbInsertException("RecordData", exp);
            }
            return new EmptyResult();
        }
        public IActionResult IndexFaceSync()
        {
            DbGetFaceSyncData(null);
            return View(rakindaModel);
        }
        //====================================================================
        public string ReadStream()
        {
            try
            {
                //Stream reqStream = Request.Body;

                //reqStream.Seek(0, SeekOrigin.Begin);

                //byte[] buffer = new byte[(int)reqStream.Length];

                //reqStream.Read(buffer, 0, (int)reqStream.Length);

                //string json = "";

                //foreach (char a in buffer)

                //{
                //    json = json + a.ToString();

                //}
                //return json;
                var vRequest = HttpContext.Request;
                vRequest.EnableBuffering();
                var vJson = "";
                var vStream = HttpContext.Request.Body;
                long? vLength = HttpContext.Request.ContentLength;
                if (vLength != null && vLength > 0)
                {
                    // Use this method to read, and use asynchronous
                    StreamReader streamReader = new StreamReader(vStream, Encoding.UTF8);
                    vJson = streamReader.ReadToEndAsync().Result;
                }
                return vJson;
            }
            catch(Exception exp)
            {
                DbInsertException("ReadStream", exp);
            }
            return null;
        }
        void InitialSearchModel()
        {
            rakindaModel.Search = new RakindaModel.SearchClass();
            rakindaModel.Search.FromDate = DateTime.Now;
            rakindaModel.Search.FromTime = DateTime.Now.AddHours(-2);
            rakindaModel.Search.ToDate = DateTime.Now;
            rakindaModel.Search.ToTime = DateTime.Now;
        }
        void DbGetRecordData(Models.RakindaModel.SearchClass pSearchModel)
        {
            //if(pSearchModel == null)
            //{
            //    using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            //    {
            //        rakindaModel.ListPageModel = vContext.VIEW_RECORD_DATAS.OrderByDescending(x => x.created).ToList();
            //    }
            //}
            string vSql = "select * from rakinda.ViewRECORD_DATA";
            vSql = vSql + " where convert(datetime, created, 103) >='" + pSearchModel.FromDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.FromTime.ToString("HH:mm:ss") + "'";
            vSql = vSql + " and convert(datetime, created, 103) <='" + pSearchModel.ToDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.ToTime.ToString("HH:mm:ss") + "'";

            vSql = vSql + " order by created desc";

            System.Data.DataTable vDt = MyClass.DbContextHelper.EFExecSQL(vSql, dbTALogContext);
            rakindaModel.ListPageModel = MyClass.DbContextHelper.ConvertToList<Models.EFModels.VIEW_RECORD_DATA>(vDt);
        }
        void DbGetTemperatureRecord(Models.RakindaModel.SearchClass pSearchModel)
        {
            //if(pSearchModel == null)
            //{
            //    using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            //    {
            //        rakindaModel.ListPageModel = vContext.VIEW_RECORD_DATAS.OrderByDescending(x => x.created).ToList();
            //    }
            //}
            string vSql = "select * from rakinda.ViewTEMPERATURE_RECORD";
            vSql = vSql + " where convert(datetime, created, 103) >='" + pSearchModel.FromDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.FromTime.ToString("HH:mm:ss") + "'";
            vSql = vSql + " and convert(datetime, created, 103) <='" + pSearchModel.ToDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.ToTime.ToString("HH:mm:ss") + "'";

            vSql = vSql + " order by created desc";

            System.Data.DataTable vDt = MyClass.DbContextHelper.EFExecSQL(vSql, dbTALogContext);
            rakindaModel.ListTemperatureRecord = MyClass.DbContextHelper.ConvertToList<Models.EFModels.VIEW_TEMPERATURE_RECORD>(vDt);
        }
        void DbGetExceptionData(Models.RakindaModel.SearchClass pSearchModel)
        {
            //if(pSearchModel == null)
            //{
            //    using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            //    {
            //        rakindaModel.ListPageModel = vContext.VIEW_RECORD_DATAS.OrderByDescending(x => x.created).ToList();
            //    }
            //}
            string vSql = "select * from rakinda.EXCEPTION";
            vSql = vSql + " where convert(datetime, created, 103) >='" + pSearchModel.FromDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.FromTime.ToString("HH:mm:ss") + "'";
            vSql = vSql + " and convert(datetime, created, 103) <='" + pSearchModel.ToDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.ToTime.ToString("HH:mm:ss") + "'";
            vSql = vSql + " order by created desc";

            System.Data.DataTable vDt = MyClass.DbContextHelper.EFExecSQL(vSql, dbTALogContext);
            rakindaModel.ListExceptionModel = MyClass.DbContextHelper.ConvertToList<Models.EFModels.EXCEPTION>(vDt);
        }
        void DbGetHeartBreakData(Models.RakindaModel.SearchClass pSearchModel)
        {
            string vSql = "select * from rakinda.HEART_BREAK";
            vSql = vSql + " where convert(datetime, created, 103) >='" + pSearchModel.FromDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.FromTime.ToString("HH:mm:ss") + "'";
            vSql = vSql + " and convert(datetime, created, 103) <='" + pSearchModel.ToDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.ToTime.ToString("HH:mm:ss") + "'";
            vSql = vSql + " order by created desc";

            //MyClass.DbContextHelper.RefreshContext(dbTALogContext);
            System.Data.DataTable vDt = MyClass.DbContextHelper.EFExecSQL(vSql, dbTALogContext);
            rakindaModel.ListHeartBreakModel = MyClass.DbContextHelper.ConvertToList<Models.EFModels.HEART_BREAK>(vDt);
        }
        void DbGetCheckOpenAuthData(Models.RakindaModel.SearchClass pSearchModel)
        {
            string vSql = "select * from rakinda.CHECK_OPEN_AUTH";
            vSql = vSql + " where convert(datetime, created, 103) >='" + pSearchModel.FromDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.FromTime.ToString("HH:mm:ss") + "'";
            vSql = vSql + " and convert(datetime, created, 103) <='" + pSearchModel.ToDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.ToTime.ToString("HH:mm:ss") + "'";
            vSql = vSql + " order by created desc";

            MyClass.DbContextHelper.RefreshContext(dbTALogContext);
            System.Data.DataTable vDt = MyClass.DbContextHelper.EFExecSQL(vSql, dbTALogContext);
            rakindaModel.ListCheckOpenAuthen = MyClass.DbContextHelper.ConvertToList<Models.EFModels.CHECK_OPEN_AUTH>(vDt);
        }
        void DbGetFaceSyncData(Models.RakindaModel.SearchClass pSearchModel)
        {
            string vSql = "select * from rakinda.FACE_SYNC";
            if (pSearchModel != null)
            {
                vSql = vSql + " where convert(datetime, created, 103) >='" + pSearchModel.FromDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.FromTime.ToString("HH:mm:ss") + "'";
                vSql = vSql + " and convert(datetime, created, 103) <='" + pSearchModel.ToDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.ToTime.ToString("HH:mm:ss") + "'";
                vSql = vSql + " order by created desc";
            }
            MyClass.DbContextHelper.RefreshContext(dbTALogContext);
            System.Data.DataTable vDt = MyClass.DbContextHelper.EFExecSQL(vSql, dbTALogContext);
            rakindaModel.ListFaceSync = MyClass.DbContextHelper.ConvertToList<Models.EFModels.FACE_SYNC>(vDt);
        }
        void DbInsertRecordData(Models.RecordJson pRecordJson)
        {
            using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            {
                try
                {
                    string vSql = "insert into rakinda.record_data(qrcode, device_no, base64" +
                                                        ", oper_type, temperature, username" +
                                                        ", nonce, sign)" +
                                                        "values('$val00', '$val01', '$val02'" +
                                                        ", '$val03', '$val04', '$val05'" +
                                                        ", '$val06', '$val07')";
                    vSql = vSql.Replace("$val00", string.IsNullOrEmpty(pRecordJson.qrcode) ? "" : pRecordJson.qrcode);
                    vSql = vSql.Replace("$val01", string.IsNullOrEmpty(pRecordJson.device_no) ? "" : pRecordJson.device_no);
                    vSql = vSql.Replace("$val02", string.IsNullOrEmpty(pRecordJson.base64) ? "" : pRecordJson.base64);
                    vSql = vSql.Replace("$val03", string.IsNullOrEmpty(pRecordJson.oper_type) ? "" : pRecordJson.oper_type);
                    vSql = vSql.Replace("$val04", string.IsNullOrEmpty(pRecordJson.temperature.ToString()) ? "" : pRecordJson.temperature.ToString());
                    vSql = vSql.Replace("$val05", string.IsNullOrEmpty(pRecordJson.userName) ? "" : pRecordJson.userName);
                    vSql = vSql.Replace("$val06", string.IsNullOrEmpty(pRecordJson.nonce) ? "" : pRecordJson.nonce);
                    vSql = vSql.Replace("$val07", string.IsNullOrEmpty(pRecordJson.sign) ? "" : pRecordJson.sign);

                    bool vCheck = MyClass.DbContextHelper.EFExecQuery(vSql, vContext);
                }
                catch(Exception exp)
                {
                    DbInsertException("DbInsertRecordData", exp);
                }
            }
        }
        void DbInsertException(string pMethodName, Exception pExp)
        {
            using(Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            {
                string vSql = "insert into rakinda.exception(controller, action, exception, exception_inner)" +
                                                    "values('Rakinda', '$val00', '$val01', '$val02');";
                vSql = vSql.Replace("$val00", pMethodName);
                vSql = vSql.Replace("$val01", pExp.Message.Replace("'", ""));
                vSql = vSql.Replace("$val02", pExp.InnerException != null ? pExp.InnerException.Message : "");

                bool vCheck = MyClass.DbContextHelper.EFExecQuery(vSql, vContext);
            }
        }
    }
}
