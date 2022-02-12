using APIsASManager.CoreV3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Controllers
{
    [Route("v{version:apiVersion}/api/[controller]Controller/[action]")]
    //[Route("v{version:apiVersion}/api/[controller]/[action]")]
    [ApiController]
    //[AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiVersion("3.0")]
    public class DoorController : ControllerBase
    {
        [HttpPost]
        [ActionName("temperatureRecord")]
        public IActionResult TemperatureRecord()
        {
            string json = ReadStream();
            try
            {
                RecordJson rj = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordJson>(json);
                if (rj != null)
                {
                    DbInsertRecordData(rj);
                }
                else
                {
                    DbInsertException("TemperatureRecord", "null RecordJson");
                    return NotFound();
                }
            }
            catch (Exception exp)
            {
                DbInsertException("TemperatureRecord", exp);
                return Unauthorized();
            }
            return Ok();
        }
        [HttpPost]
        public IActionResult TemperatureRecordBody([FromBody] RecordJson recordJson)
        {
            try
            {
                if (recordJson != null)
                {
                    DbInsertRecordData(recordJson);
                    return Ok();
                }
                else
                {
                    DbInsertException("TemperatureRecordBody", "null RecordJson");
                    return NotFound();
                }
            }
            catch (Exception exp)
            {
                DbInsertException("TemperatureRecord", exp);
                return Unauthorized();
            }
            return Unauthorized();
        }
        [HttpPost]
        public IActionResult heartBrake()
        {
            string json = ReadStream();
            try
            {
                HeartBrakeJson rj = Newtonsoft.Json.JsonConvert.DeserializeObject<HeartBrakeJson>(json);
                if (rj != null)
                {
                    DbInsertRecordData(rj);
                }
                else
                {
                    DbInsertException("heartBrake", "null RecordJson");
                    return NotFound();
                }
            }
            catch (Exception exp)
            {
                DbInsertException("heartBrake", exp);
                return Unauthorized();
            }
            return Ok();
        }
        [HttpPost]
        public IActionResult checkOpenAuth()
        {
            string json = ReadStream();
            try
            {
                CheckOpenAuthJson rj = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckOpenAuthJson>(json);
                if (rj != null)
                {
                    DbInsertRecordData(rj);
                }
                else
                {
                    DbInsertException("checkOpenAuth", "null RecordJson");
                    return NotFound();
                }
            }
            catch (Exception exp)
            {
                DbInsertException("checkOpenAuth", exp);
                return Unauthorized();
            }
            return Ok();
        }
        //====================================================================\
        [NonAction]
        string ReadStream()
        {
            try
            {
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
                    WriteEventLog(vRequest.Path.Value.ToString(), vJson);
                }
                return vJson;

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
            }
            catch (Exception exp)
            {
                DbInsertException("ReadStream", exp);
            }
            return null;
        }
        void DbInsertRecordData(Models.RecordJson pRecordJson)
        {
            using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            {
                try
                {
                    string vSql = "insert into rakinda.TEMPERATURE_RECORD(qrcode, device_no, base64" +
                                                        ", oper_type, temperature, username" +
                                                        ", nonce, sign, code, msg)" +
                                                        "values('$val00', '$val01', '$val02'" +
                                                        ", '$val03', '$val04', '$val05'" +
                                                        ", '$val06', '$val07', '$val08', '$val09')";
                    vSql = vSql.Replace("$val00", string.IsNullOrEmpty(pRecordJson.qrcode) ? "" : pRecordJson.qrcode);
                    vSql = vSql.Replace("$val01", string.IsNullOrEmpty(pRecordJson.device_no) ? "" : pRecordJson.device_no);
                    vSql = vSql.Replace("$val02", string.IsNullOrEmpty(pRecordJson.base64) ? "" : pRecordJson.base64);
                    vSql = vSql.Replace("$val03", string.IsNullOrEmpty(pRecordJson.oper_type) ? "" : pRecordJson.oper_type);
                    vSql = vSql.Replace("$val04", string.IsNullOrEmpty(pRecordJson.temperature.ToString()) ? "" : pRecordJson.temperature.ToString());
                    vSql = vSql.Replace("$val05", string.IsNullOrEmpty(pRecordJson.userName) ? "" : pRecordJson.userName);
                    vSql = vSql.Replace("$val06", string.IsNullOrEmpty(pRecordJson.nonce) ? "" : pRecordJson.nonce);
                    vSql = vSql.Replace("$val07", string.IsNullOrEmpty(pRecordJson.sign) ? "" : pRecordJson.sign);
                    vSql = vSql.Replace("$val08", string.IsNullOrEmpty(pRecordJson.code) ? "" : pRecordJson.code);
                    vSql = vSql.Replace("$val09", string.IsNullOrEmpty(pRecordJson.msg) ? "" : pRecordJson.msg);

                    bool vCheck = MyClass.DbContextHelper.EFExecQuery(vSql, vContext);
                }
                catch (Exception exp)
                {
                    DbInsertException("DbInsertRecordData", exp);
                }
            }
        }
        void DbInsertRecordData(Models.HeartBrakeJson  pHeartBrakeJson)
        {
            using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            {
                try
                {
                    List<Models.EFModels.HEART_BREAK> vTemp = new List<Models.EFModels.HEART_BREAK>();
                    vTemp.Add(new Models.EFModels.HEART_BREAK()
                    {
                        created = DateTime.Now,
                        device_no = pHeartBrakeJson.device_no,
                        device_status = pHeartBrakeJson.device_status,
                        nonce = pHeartBrakeJson.nonce,
                        sign = pHeartBrakeJson.sign,
                        code = pHeartBrakeJson.code,
                        msg = pHeartBrakeJson.msg,
                        create_date = pHeartBrakeJson.create_date
                    });

                    MyClass.DbContextHelper.AddEntities<Models.EFModels.HEART_BREAK>(vTemp, vContext);
                }
                catch (Exception exp)
                {
                    DbInsertException("DbInsertRecordData", exp);
                }
            }
        }
        void DbInsertRecordData(Models.CheckOpenAuthJson  pCheckOpenAuthJson)
        {
            using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            {
                try
                {
                    List<Models.EFModels.CHECK_OPEN_AUTH> vTemp = new List<Models.EFModels.CHECK_OPEN_AUTH>();
                    vTemp.Add(new Models.EFModels.CHECK_OPEN_AUTH()
                    {
                        created = DateTime.Now,
                        device_no = pCheckOpenAuthJson.device_no,
                        oper_type = pCheckOpenAuthJson.oper_type,
                        qrcode = pCheckOpenAuthJson.qrcode,
                        nonce = pCheckOpenAuthJson.nonce,
                        sign = pCheckOpenAuthJson.sign,
                        code = pCheckOpenAuthJson.code,
                        msg = pCheckOpenAuthJson.msg,
                    });

                    MyClass.DbContextHelper.AddEntities<Models.EFModels.CHECK_OPEN_AUTH>(vTemp, vContext);
                }
                catch (Exception exp)
                {
                    DbInsertException("DbInsertRecordData", exp);
                }
            }
        }
        void DbInsertException(string pMethodName, Exception pExp)
        {
            using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            {
                string vSql = "insert into rakinda.exception(controller, action, exception, exception_inner)" +
                                                    "values('Door', '$val00', '$val01', '$val02');";
                vSql = vSql.Replace("$val00", pMethodName);
                vSql = vSql.Replace("$val01", pExp.Message.Replace("'", ""));
                vSql = vSql.Replace("$val02", pExp.InnerException != null ? pExp.InnerException.Message : "");

                bool vCheck = MyClass.DbContextHelper.EFExecQuery(vSql, vContext);
            }
        }
        void DbInsertException(string pMethodName, string pExp)
        {
            using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            {
                string vSql = "insert into rakinda.exception(controller, action, exception)" +
                                                    "values('Door', '$val00', '$val01');";
                vSql = vSql.Replace("$val00", pMethodName);
                vSql = vSql.Replace("$val01", pExp);
                
                bool vCheck = MyClass.DbContextHelper.EFExecQuery(vSql, vContext);
            }
        }
        void WriteEventLog(string pUrl, string pData)
        {
            try
            {
                string vMsg = pUrl.ToString() + " [ data: " + pData + "]" + Environment.NewLine;
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    //.WriteTo.Console()
                    .WriteTo.File("Logs/RakindaLog.txt", rollingInterval: RollingInterval.Day
                                , retainedFileCountLimit: 180
                                , shared: true)
                    .CreateLogger();
                Log.Information(vMsg);
            }
            catch (Exception exp)
            {
                Log.Error(exp, "Something went wrong");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
