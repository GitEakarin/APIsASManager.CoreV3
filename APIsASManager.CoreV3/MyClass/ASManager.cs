using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace APIsASManager.CoreV3.MyClass
{
    public class ASManager : IASManager, IDisposable
    {
        string asHostName;
        string asUser;
        string asPwd;
        static string asToken;
        static DateTime asTokenTime;
        static string owner;
        private bool disposedValue;
        IConfiguration configuration;
        //Models.EFLiteModels.LiteDBContext taLogDbContext = new Models.EFLiteModels.LiteDBContext();
        #region Request/Response Structure
        public class LoginResponse
        {
            public int Success;
            public string Token;
            public string ErrMsg;
        }
        #endregion
        public ASManager(IConfiguration pConfiguration)
        {
            asHostName = pConfiguration.GetSection("ASManager").GetSection("HostName").Value;
            asUser = pConfiguration.GetSection("ASManager").GetSection("user").Value;
            asPwd = pConfiguration.GetSection("ASManager").GetSection("pwd").Value;
            configuration = pConfiguration;
            owner = pConfiguration.GetSection("MySettings").GetSection("Owner").Value;
            //var x = Login().Result;
            //BackgroundJobClient.Schedule(() => Console.WriteLine("Delayed job executed"), TimeSpan.FromMinutes(1));
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ASManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        HttpClient CreateHttpClient()
        {
            HttpClient vHttpClient = new HttpClient();
            vHttpClient.DefaultRequestHeaders.Accept.Clear();
            vHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            return vHttpClient;
        }
        public IBackgroundJobClient BackgroundJobClient { get; set; }
        public void ClearBackgroundJobClient()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in StorageConnectionExtensions.GetRecurringJobs(connection))
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }
        }
        //public RecurringJob RecurringJobs { get; set; }
        public void StartBackgroundJobClient()
        {
            //BackgroundJobClient.Schedule(() =>AccessGranted(0, "", "", 1, 50), TimeSpan.FromMinutes(1));

            //RecurringJob.AddOrUpdate(() => AccessGranted(0, "", "", 1, 50), Cron.Minutely);

            int vInterval = Convert.ToInt16(configuration.GetSection("ASManager").GetSection("APIsInterval").Value);
            //RecurringJob.RemoveIfExists(nameof(AccessGranted));
            RecurringJob.AddOrUpdate(() => AccessGranted(0, "", "", 1, 50), Cron.MinuteInterval(vInterval));

        }
        public async Task<LoginResponse> Login()
        {
            
            LoginResponse vLoginRes = new LoginResponse();
            HttpClient vHttpClient = CreateHttpClient();
            var vBody = new
            {
                action = "LOGIN",
                username = asUser,
                password = asPwd
            };
            //string vUrl = asHostName + @"/WebService.srf";
            //string vJson = JsonConvert.SerializeObject(vBody);
            //StringContent vStringContent = new StringContent(vJson, Encoding.UTF8, "application/json");
            //var vResponse = await vHttpClient.PostAsync(vUrl, vStringContent);
            //var vResult = await vResponse.Content.ReadAsStringAsync();
            try
            {
                string vUrl = asHostName + @"/WebService.srf?action=LOGIN&username=" + asUser + "&password=" + asPwd;
                var vResponse = await vHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, vUrl));
                var vResult = await vResponse.Content.ReadAsStringAsync();
                vLoginRes = JsonConvert.DeserializeObject<LoginResponse>(vResult.ToString());
                asToken = vLoginRes.Token;
                if (vLoginRes.Success == 1)
                    asTokenTime = DateTime.Now;

                if (vLoginRes.Success == 0)
                {
                    WriteExceptions("[Login]success:" + vLoginRes.Success + ", errmsg:" + vLoginRes.ErrMsg);
                    //BackgroundJobClient.Enqueue(() => WriteExceptions("[Login]success:" + vLoginRes.Success + ", errmsg:" + vLoginRes.ErrMsg));
                }
                if(vResponse.StatusCode != HttpStatusCode.OK)
                {
                    WriteExceptions("[Login]status code:" + vResponse.StatusCode.ToString() + ", reason:" + vResponse.ReasonPhrase.ToString());
                }
            }
            catch(Exception exp)
            {
                vLoginRes.Success = -2;
                vLoginRes.ErrMsg = exp.Message;
            }
            return vLoginRes;
        }

        public async Task<IEnumerable<Models.EFModels.USER_ID_LIST>> USER_ID_LIST()
        {
            if (CheckLogin() == false)
                return new List<Models.EFModels.USER_ID_LIST>();

            HttpClient vHttpClient = CreateHttpClient();
            var vBody = new
            {
                action = "USER_ID_LIST",
                token = asToken,
                module = "persondata"
            };
            string vUrl = asHostName + @"/WebService.srf?action=$val00&token=$val01&module=$val02";
            vUrl = vUrl.Replace("$val00", vBody.action);
            vUrl = vUrl.Replace("$val01", vBody.token);
            vUrl = vUrl.Replace("$val02", vBody.module);
            var vResponse = await vHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, vUrl));
            var vResult = await vResponse.Content.ReadAsStringAsync();
            var x = JsonConvert.DeserializeObject<Models.ASManagerModel.UserIdListClass>(vResult);
            return x.data.ToList();
        }

        public async Task<Models.ASManagerModel.GetUserClass> GET_USER(int pChId)
        {
            if (CheckLogin() == false)
                return new Models.ASManagerModel.GetUserClass();

            HttpClient vHttpClient = CreateHttpClient();
            Models.ASManagerModel.GetUserClass vRet = new Models.ASManagerModel.GetUserClass();
            var vBody = new
            {
                action = "GET_USER",
                token = asToken,
                module = "persondata",
                ch_id = pChId.ToString()
            };
            string vUrl = asHostName + @"/WebService.srf?action=$val00&token=$val01&module=$val02&ch_id=$val03";
            vUrl = vUrl.Replace("$val00", vBody.action);
            vUrl = vUrl.Replace("$val01", vBody.token);
            vUrl = vUrl.Replace("$val02", vBody.module);
            vUrl = vUrl.Replace("$val03", vBody.ch_id);

            WebRequest vRequest = WebRequest.Create(vUrl);
            WebResponse vResponse = await Task.Run(() => vRequest.GetResponse());
            StreamReader vReader = new StreamReader(vResponse.GetResponseStream());
            string vJson = vReader.ReadToEnd();
            
            vRet = JsonConvert.DeserializeObject<Models.ASManagerModel.GetUserClass>(vJson);
            if (vRet != null)
                WriteEventLog(nameof(this.GET_CARD).ToString(), vRet.success, vRet.total.ToString(), vRet.data == null ? "0" : vRet.data.Count.ToString());

            List<Models.EFModels.GET_USER> vModel = vRet.data.ToList();
            if (vRet != null)
                WriteEventLog(nameof(this.GET_CARD).ToString(), vRet.success, vRet.total.ToString(), vRet.data.Count.ToString());

            using (Models.EFModels.TAConfigContext vContext = new Models.EFModels.TAConfigContext())
            {
                MyClass.DbContextHelper.AddEntities<Models.EFModels.GET_USER>(vModel, vContext);
            }
            return vRet;
        }
        public async Task<Models.ASManagerModel.GetCardClass> GET_CARD(int pCCode, string pCNo)
        {
            if (CheckLogin() == false)
                return new Models.ASManagerModel.GetCardClass();

            HttpClient vHttpClient = CreateHttpClient();
            Models.ASManagerModel.GetCardClass vRet = new Models.ASManagerModel.GetCardClass();
            var vBody = new
            {
                action = "GET_CARD",
                token = asToken,
                module = "persondata",
                c_code = pCCode.ToString(),
                c_no = pCNo
            };
            string vUrl = asHostName + @"/WebService.srf?action=$val00&token=$val01&module=$val02&c_code=$val03&c_no=$val04";
            vUrl = vUrl.Replace("$val00", vBody.action);
            vUrl = vUrl.Replace("$val01", vBody.token);
            vUrl = vUrl.Replace("$val02", vBody.module);
            vUrl = vUrl.Replace("$val03", vBody.c_code);
            vUrl = vUrl.Replace("$val04", vBody.c_no);

            WebRequest vRequest = WebRequest.Create(vUrl);
            WebResponse vResponse = await Task.Run(() => vRequest.GetResponse());
            StreamReader vReader = new StreamReader(vResponse.GetResponseStream());
            string vJson = vReader.ReadToEnd();

            vRet = JsonConvert.DeserializeObject<Models.ASManagerModel.GetCardClass>(vJson);
            if (vRet != null)
            {
                WriteEventLog(nameof(this.GET_CARD).ToString(), vRet.success, vRet.total.ToString(), vRet.data == null ? "0" : vRet.data.Count.ToString());
            }
            List<Models.EFModels.GET_CARD> vModel = vRet.data.ToList();
            using (Models.EFModels.TAConfigContext vContext = new Models.EFModels.TAConfigContext())
            {
                MyClass.DbContextHelper.AddEntities<Models.EFModels.GET_CARD>(vModel, vContext);
            }
            return vRet;
        }
        public async Task<Models.ASManagerModel.AccessGrantedClass> AccessGranted(int pLogTimeType, string pLogTimeStart = "", string pLogTimeEnd = "", int pStart = 1, int pLimit = 50)
        {
            if (CheckLogin() == false)
                return new Models.ASManagerModel.AccessGrantedClass();
            
            HttpClient vHttpClient = CreateHttpClient();
            string vUrl = asHostName + @"/bin/Accesslog.srf";
            string vJson = "";
            Models.ASManagerModel.AccessGrantedClass vRet = new Models.ASManagerModel.AccessGrantedClass();
            if (pLogTimeType < 6)
            {
                var vBody = new
                {
                    action = "AccessLog",
                    token = asToken,
                    log_time_type = pLogTimeType.ToString(),
                    log_msg = 1,
                    sort_by = "log_id",
                    sort_dir = "DESC",
                    start = pStart,
                    limit = pLimit
                };
                vUrl = vUrl + @"?action=$val00&token=$val01&log_time_type=$val02&log_msg=$val03&sort_by=log_id&sort_dir=DESC&start=$val04&limit=$val05";
                vUrl = vUrl.Replace("$val00", vBody.action);
                vUrl = vUrl.Replace("$val01", vBody.token);
                vUrl = vUrl.Replace("$val02", vBody.log_time_type);
                vUrl = vUrl.Replace("$val03", vBody.log_msg.ToString());
                vUrl = vUrl.Replace("$val04", vBody.start.ToString());
                vUrl = vUrl.Replace("$val05", vBody.limit.ToString());
            }
            else
            {
                var vBody = new
                {
                    action = "AccessLog",
                    token = asToken,
                    log_time_type = pLogTimeType.ToString(),
                    //log_msg = 6,
                    log_time_start = pLogTimeStart,
                    log_time_end = pLogTimeEnd,
                    sort_by = "log_id",
                    sort_dir = "DESC",
                    start = pStart,
                    limit = pLimit
                };
                vUrl = vUrl + @"?action=$val00&token=$val01&log_time_type=$val02&log_time_start=$val04&log_time_end=$val05&sort_by=log_id&sort_dir=DESC&start=$val06&limit=$val07";
                vUrl = vUrl.Replace("$val00", vBody.action);
                vUrl = vUrl.Replace("$val01", vBody.token);
                vUrl = vUrl.Replace("$val02", vBody.log_time_type);
                //vUrl = vUrl.Replace("$val03", vBody.log_msg.ToString());
                vUrl = vUrl.Replace("$val04", vBody.log_time_start);
                vUrl = vUrl.Replace("$val05", vBody.log_time_end);
                vUrl = vUrl.Replace("$val06", vBody.start.ToString());
                vUrl = vUrl.Replace("$val06", vBody.limit.ToString());
            }
            try
            {
                //var vResponse = await vHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, vUrl));
                //var vResult = await vResponse.Content.ReadAsStringAsync();
                //vRet = JsonConvert.DeserializeObject<Models.ASManagerModel.AccessGrantedClass>(vResult);
                //MyClass.HelperClass.EFExecQuery("delete from access_log", taLogDbContext);

                //taLogDbContext.SaveChanges();

                WebRequest request = WebRequest.Create(vUrl);
                WebResponse response = await Task.Run(() => request.GetResponse());
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string json = reader.ReadToEnd();
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                settings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "yyyy/MM/dd" });

                vRet = JsonConvert.DeserializeObject<Models.ASManagerModel.AccessGrantedClass>(json, settings);
                //return x;
                reader.Close();
                reader.Dispose();
                response.Close();
                response.Dispose();
                request = null;

                //List<Models.EFLiteModels.ACCESS_LOG> vModel = vRet.data.ToList();
                //List<Models.EFModels.ACCESS_GRANTED>  vModel = ConvertDateTimeAcessLog(vRet.data.ToList());//for sqlite
                List<Models.EFModels.ACCESS_GRANTED> vModel = vRet.data;
                if (vRet != null)
                    WriteEventLog(nameof(this.GET_CARD).ToString(), vRet.success, vRet.total.ToString(), vRet.data == null ? "0" : vRet.data.Count.ToString());


                using (Models.EFModels.TALogContext vContext = new Models.EFModels.TALogContext())
                {
                    MyClass.DbContextHelper.AddEntities<Models.EFModels.ACCESS_GRANTED>(vModel, vContext);
                }
                List<Models.EFModels.GET_USER> vListUser = vModel.Select(x => x.ch_id).Distinct().Select(x => new Models.EFModels.GET_USER() { ch_id = x }).ToList();
                //var vTemp = vModel.Select(y => new Models.EFLiteModels.GET_USER() { ch_id = y.ch_id }).Distinct().ToList().Select(y => y.ch_id ).Distinct().ToList();
                //vList = vTemp.Select(x => new Models.EFLiteModels.GET_USER() { ch_id = x }).ToList();

                List<Models.EFModels.GET_CARD> vListCard = vModel.Select(x => new { c_no = x.c_no, c_code = x.code_value, ch_id = x.ch_id }).Distinct().Select(x => new Models.EFModels.GET_CARD() { c_no = x.c_no, c_code = x.c_code, ch_id = x.ch_id }).ToList();

                //Task.Run(() => DbCheckGetUser(vListUser));
                //Task.Run(() => DbCheckGetCard(vListCard));
                

                BackgroundJob.Enqueue(() => this.DbCheckGetUser(vListUser));
                BackgroundJob.Enqueue(() => this.DbCheckGetCard(vListCard));

                asTokenTime = DateTime.Now;
            }
            catch (Exception exp)
            {
                BackgroundJobClient.Enqueue(() => WriteExceptions(exp.Message, exp));
                WriteExceptions(exp.Message, exp);
                return new Models.ASManagerModel.AccessGrantedClass();
            }
            finally
            {
                vHttpClient.Dispose();
                vHttpClient = null;
            }
            return vRet;
        }
        #region Internal method
        List<Models.EFModels.ACCESS_GRANTED> ConvertDateTimeAcessLog(List<Models.EFModels.ACCESS_GRANTED> pList)//for sqlite
        {
            List<Models.EFModels.ACCESS_GRANTED> vModel = new List<Models.EFModels.ACCESS_GRANTED>();
            if (pList.Count == 0)
                return vModel;
            //Parallel.For(0, pList.Count -1, i => {
            //    var v_log_time = DateTime.Parse(pList[i].log_time.ToString());
            //    vModel.Add(pList[i]);
            //    vModel[i].log_time = v_log_time.ToString("yyyy-MM-dd HH:mm:ss");
            //});
            //for(int i=0; i<pList.Count-1; i++)
            //{
            //    var v_log_time = DateTime.Parse(pList[i].log_time.ToString());
            //    vModel.Add(pList[i]);
            //    //vModel[i].log_time = v_log_time.ToString("yyyy-MM-dd HH:mm:ss");
            //    vModel[i].log_time = v_log_time;
            //}
            return pList;
        }
        bool CheckLogin()
        {
            if ((DateTime.Now - asTokenTime).TotalMinutes >= 20)
            {
                LoginResponse vLoginRes = Login().Result;
                if (vLoginRes.Success == 1)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }
        void WriteExceptions(string pMsg, Exception exp)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    //.WriteTo.Console()
                    .WriteTo.File("Exceptions/log.txt", rollingInterval: RollingInterval.Day
                                , retainedFileCountLimit: 180
                                , outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
                Log.Error(pMsg, exp);
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "Something went wrong");
                WriteExceptions(ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        public void WriteEventLog(string pUrl, string pSuccess, string pTotal, string pData)
        {
            try
            {
                string vMsg = pUrl.ToString() + " [ success:" + pSuccess + ", total:" + pTotal + ", data:" + pData + "]" + Environment.NewLine;
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    //.WriteTo.Console()
                    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day
                                , retainedFileCountLimit: 180
                                , shared:true)
                    .CreateLogger();
                Log.Information(vMsg);
            }
            catch(Exception exp)
            {
                Log.Error(exp, "Something went wrong");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        void WriteExceptions(string pMsg)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    //.WriteTo.Console()
                    .WriteTo.File("/Exceptions/log.txt", rollingInterval: RollingInterval.Day
                                , retainedFileCountLimit: 180, shared: true)
                    .CreateLogger();
                Log.Error(pMsg);
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
        public void DbCheckGetUser(List<Models.EFModels.GET_USER> pList)
        {
            List<Models.EFModels.GET_USER> vGET_USER = null;
            using (Models.EFModels.TAConfigContext vContext = new Models.EFModels.TAConfigContext())
            {
                //vGET_USER = vContext.GET_USERS.Where(x => !pList.Select(y => y.ch_id).Contains(x.ch_id)).ToList();
                vGET_USER = pList.Where(x => !vContext.GET_USERS.Where(y => y.ch_id == x.ch_id).Any()).ToList();
            }

            //foreach (var vItem in pList)
            //{
            //    GET_USER(vItem.ch_id);
            //}

            if (vGET_USER == null || vGET_USER.Count == 0)
                return;
            foreach (var vItem in vGET_USER)
            {
                GET_USER(vItem.ch_id);
            }
        }
        public void DbCheckGetCard(List<Models.EFModels.GET_CARD> pList)
        {
            List<Models.EFModels.GET_CARD> vGET_CARD = null;
            using (Models.EFModels.TAConfigContext vContext = new Models.EFModels.TAConfigContext())
            {
                //vGET_USER = vContext.GET_USERS.Where(x => !pList.Select(y => y.ch_id).Contains(x.ch_id)).ToList();
                vGET_CARD = pList.Where(x => !vContext.GET_CARDS.Where(y => y.c_no == x.c_no && y.c_code == x.c_code && y.ch_id == x.ch_id).Any()).ToList();
            }

            //foreach (var vItem in pList)
            //{
            //    GET_USER(vItem.ch_id);
            //}

            if (vGET_CARD == null || vGET_CARD.Count == 0)
                return;
            foreach (var vItem in vGET_CARD)
            {
                GET_CARD(vItem.c_code, vItem.c_no.ToString());
            }
        }
        #endregion
    }
}
