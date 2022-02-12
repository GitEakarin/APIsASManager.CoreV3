using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.MyClass
{
    public interface IASManager
    {
        Task<ASManager.LoginResponse> Login();
        Task<Models.ASManagerModel.AccessGrantedClass> AccessGranted(int pLogTimeType, string pLogTimeStart = "", string pLogTimeEnd = "", int pStart = 1, int pLimit = -1);
        void WriteEventLog(string pUrl, string pSuccess, string pTotal, string pData);
        IBackgroundJobClient BackgroundJobClient { get; set; }
        void ClearBackgroundJobClient();
        void StartBackgroundJobClient();
        //bool Logout(string pToken);
    }
}
