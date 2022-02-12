using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Controllers
{
    [Route("v{version:apiVersion}/api/[controller]/[action]")]
    [ApiController]
    //[AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiVersion("3.0")]
    //[Authorize(Roles = "Admin, User")]
    public class ASManagerController : ControllerBase
    {
        static MyClass.IASManager asManager;
        Models.EFModels.TALogContext taLogDbContext;
        public ASManagerController(MyClass.IAuthorizationManager pAuthorizationManager, IConfiguration configuration, Models.EFModels.TALogContext pdBContext, MyClass.IASManager pASManager)
        {
            //authorizationManager = pAuthorizationManager;
            //asManager = new MyClass.ASManager(configuration);
            asManager = pASManager;
            taLogDbContext = pdBContext;
        }
        /// <summary>
        /// get access log with access granted msg
        /// </summary>
        /// <param name="log_time_type">0-6</param>
        /// <param name="log_time_start">yyyy/MM/dd HH:mm:ss</param>
        /// <param name="log_time_end">yyyy/MM/dd HH:mm:ss</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<Models.ASManagerModel.AccessGrantedClass>> ASAccessGranted(int log_time_type, string log_time_start = "", string log_time_end = "")
        {
            Models.ASManagerModel.AccessGrantedClass vModel;
            try
            {
                vModel = await asManager.AccessGranted(log_time_type, log_time_start, log_time_end);
                Task.Run(() => asManager.WriteEventLog("[" + HttpStatusCode.OK.ToString() + "]" +  nameof(this.ASAccessGranted).ToString(), vModel.success, vModel.total.ToString(), vModel.data == null ? "0" : vModel.data.Count.ToString()));
                return Ok(vModel);
            }
            catch(Exception exp)
            {
                Task.Run(() => asManager.WriteEventLog("[" + HttpStatusCode.BadRequest.ToString() + "]" + nameof(this.ASAccessGranted).ToString()  + "->" + exp.Message, "", "", "0"));
                return BadRequest(vModel = new Models.ASManagerModel.AccessGrantedClass());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="log_time_type">0-6</param>
        /// <param name="log_time_start">yyyy-MM-dd</param>
        /// <param name="log_time_end">yyyy-MM-dd HH:mm:ss</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<Models.ASManagerModel.AccessGrantedClass>> AccessGranted(int log_time_type, string log_time_start = "", string log_time_end = "")
        {
            //log_time_type
            //0 = Today
            //1 = Yesterday
            //2 = This Week
            //3 = Last Week
            //4 = This Month
            //5 = Last Month
            //6 = Date Range without Time
            //7 = Date Range with Time

            Models.ASManagerModel.AccessGrantedClass vModel = new Models.ASManagerModel.AccessGrantedClass();
            string vSql = "select * from access_granted";
            try
            {
                switch (log_time_type)
                {
                    case 0:
                        vSql = vSql + " where convert(date, log_time, 103)='" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "'";
                        break;
                    case 1:
                        vSql = vSql + " where convert(date, log_time, 103)='" + DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd") + "'";
                        break;
                    case 6:
                        vSql = vSql + " where convert(date, log_time, 103) >= '" + log_time_start.Replace(@"/", "-") + "' and convert(date, log_time, 103) <= '" + log_time_end.Replace("/", "-") + "'";
                        break;
                    case 7:
                        vSql = vSql + " where convert(datetime, log_time, 103) >= '" + log_time_start.Replace(@"/", "-") + "' and convert(datetime, log_time, 103) <= '" + log_time_end.Replace("/", "-") + "'";
                        break;
                }
                if (taLogDbContext.Database.CanConnect() == true)
                {
                    vModel.success = bool.TrueString.ToLower();
                    DataTable vDt = MyClass.DbContextHelper.EFExecSQL(vSql, taLogDbContext);
                    vModel.data = MyClass.DbContextHelper.ConvertToList<Models.EFModels.ACCESS_GRANTED>(vDt);
                    if (vModel.data.Count > 0)
                        vModel.total = vModel.data.Count - 1;
                    else
                        vModel.total = 0;
                }
                else
                {
                    vModel.success = bool.FalseString.ToLower();
                    vModel.total = 0;
                }
                Task.Run(() => asManager.WriteEventLog("[" + HttpStatusCode.OK.ToString() + "]" + nameof(this.AccessGranted).ToString(), vModel.success, vModel.total.ToString(), vModel.data == null ? "0" : vModel.data.Count.ToString()));
                return Ok(vModel);
            }
            catch(Exception exp)
            {
                Task.Run(() => asManager.WriteEventLog("[" + HttpStatusCode.BadRequest.ToString() + "]" + nameof(this.ASAccessGranted).ToString() + "->" + exp.Message, "", "", "0"));
                return BadRequest(vModel = new Models.ASManagerModel.AccessGrantedClass());
            }
        }
    }
}
