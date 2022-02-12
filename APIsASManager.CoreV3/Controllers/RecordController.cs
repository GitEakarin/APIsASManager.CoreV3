using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIsASManager.CoreV3.Models;
using Serilog;

namespace APIsASManager.CoreV3.Controllers.api
{
    [Route("v{version:apiVersion}/api/[controller]/[action]")]
    [ApiController]
    //[AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiVersion("3.0")]
    public class RecordController : ControllerBase
    {
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
            catch (Exception exp)
            {
                DbInsertException("RecordData", exp);
                return Unauthorized();
            }
            return Ok();
        }
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
        [HttpPost]
        public IActionResult RecordDataBody([FromBody] RecordJson recordJson)
        {
            try
            {
                DbInsertRecordData(recordJson);
                return Ok();
            }
            catch (Exception exp)
            {
                DbInsertException("RecordDataBody", exp);
                return Unauthorized();
            }
            return Unauthorized();
        }
        
        /// <summary>
        /// return forecast temp.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<WeatherForecast> WeatherForecast()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpPost]
        [ActionName("RecordData/DoorController/temperatureRecord")]
        public IActionResult TemperatureRecord()
        {
            string json = ReadStream();
            try
            {
                RecordJson rj = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordJson>(json);
                if (rj != null)
                {
                    DbInsertTemperatureData(rj);
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
                DbInsertException("TemperatureRecord", json);
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
        //====================================================================
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
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
                catch (Exception exp)
                {
                    DbInsertException("DbInsertRecordData", exp);
                }
            }
        }
        void DbInsertTemperatureData(Models.RecordJson pRecordJson)
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
        void DbInsertException(string pMethodName, Exception pExp)
        {
            using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
            {
                string vSql = "insert into rakinda.exception(controller, action, exception, exception_inner)" +
                                                    "values('Record', '$val00', '$val01', '$val02');";
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
                                                    "values('Record', '$val00', '$val01');";
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
