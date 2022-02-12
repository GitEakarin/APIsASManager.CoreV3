using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIsASManager.CoreV3.MyClass;
using Microsoft.AspNetCore.Mvc;

namespace APIsASManager.CoreV3.Controllers.MVC
{
    public class GeoController : Controller
    {
        Models.EFModels.TALogContext dbTALogContext;
        Models.EFModels.TAConfigContext dbTAConfigContext;
        Models.GeoModel geoModel = new Models.GeoModel();
        public GeoController(Models.EFModels.TALogContext pContext1, Models.EFModels.TAConfigContext pContext2)
        {
            dbTALogContext = pContext1;
            dbTAConfigContext = pContext2;
        }
        public IActionResult IndexCard()
        {
            InitialSearchModel();
            DbGetCard(geoModel.Search);
            return View(geoModel);
        }
        public IActionResult IndexUser()
        {
            InitialSearchModel();
            DbGetUser(geoModel.Search);
            return View(geoModel);
        }
        public IActionResult IndexAccessGrant()
        {
            InitialMonthModel();
            return View(geoModel);
        }
        //====================================================================
        void InitialSearchModel()
        {
            geoModel.Search = new Models.GeoModel.SearchClass();
            geoModel.Search.FromDate = DateTime.Now;
            geoModel.Search.FromTime = DateTime.Now.AddHours(-2);
            geoModel.Search.ToDate = DateTime.Now;
            geoModel.Search.ToTime = DateTime.Now;
        }
        void InitialMonthModel()
        {
            Models.GeoModel.MonthClass vTemp = new Models.GeoModel.MonthClass();

            geoModel.DateAllMonth = GetDates(DateTime.Now.Year, DateTime.Now.Month);
            int vCount = (int)geoModel.DateAllMonth.First().DayOfWeek;
            DateTime vDate = geoModel.DateAllMonth.First();
            for (int i=1; i <= vCount; i++)
            {
                geoModel.DateAllMonth.Insert(0, vDate.AddDays(i * -1));
            }
            vCount = (int)geoModel.DateAllMonth.Last().DayOfWeek;
            vDate = geoModel.DateAllMonth.Last();
            for (int i = vCount; i < (int)DayOfWeek.Saturday; i++)
            {
                geoModel.DateAllMonth.Add(vDate.AddDays(i));
            }
            geoModel.DateAllMonthChunk = geoModel.DateAllMonth.ChunkBy(7);
        }
        public List<DateTime> GetDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                             // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(year, month, day))
                             // Map each day to a date
                             .ToList(); // Load dates into a list
        }
        void DbGetCard(Models.GeoModel.SearchClass pSearchModel)
        {
            string vSql = "select * from dbo.GET_CARD";
            //vSql = vSql + " where convert(datetime, created, 103) >='" + pSearchModel.FromDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.FromTime.ToString("HH:mm:ss") + "'";
            //vSql = vSql + " and convert(datetime, created, 103) <='" + pSearchModel.ToDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.ToTime.ToString("HH:mm:ss") + "'";

            vSql = vSql + " order by ch_name";

            System.Data.DataTable vDt = MyClass.DbContextHelper.EFExecSQL(vSql, dbTAConfigContext);
            geoModel.ListGetCard = MyClass.DbContextHelper.ConvertToList<Models.EFModels.GET_CARD>(vDt);
        }
        void DbGetUser(Models.GeoModel.SearchClass pSearchModel)
        {
            string vSql = "select * from dbo.GET_USER";
            //vSql = vSql + " where convert(datetime, created, 103) >='" + pSearchModel.FromDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.FromTime.ToString("HH:mm:ss") + "'";
            //vSql = vSql + " and convert(datetime, created, 103) <='" + pSearchModel.ToDate.Date.ToString("yyyy-MM-dd") + " " + pSearchModel.ToTime.ToString("HH:mm:ss") + "'";

            vSql = vSql + " order by ch_name";

            System.Data.DataTable vDt = MyClass.DbContextHelper.EFExecSQL(vSql, dbTAConfigContext);
            geoModel.ListGetUser = MyClass.DbContextHelper.ConvertToList<Models.EFModels.GET_USER>(vDt);
        }
    }
}
