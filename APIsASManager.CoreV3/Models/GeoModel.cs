using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models
{
    public class GeoModel
    {
        public GeoModel()
        {
            Search = new SearchClass();
        }
        public List<Models.EFModels.ACCESS_GRANTED> ListAccessGrantedModel { get; set; }
        public List<Models.EFModels.GET_CARD> ListGetCard { get; set; }
        public List<Models.EFModels.GET_USER> ListGetUser { get; set; }
        [BindProperty]
        public SearchClass Search { get; set; }

        [BindProperties]
        public class SearchClass
        {
            public string Msg { get; set; }
            public SelectList DDLMenuMain { get; set; }
            public string DDLMenuMainValue { get; set; }
            public SelectList DDLMenuSub { get; set; }
            public string DDLMenuSubValue { get; set; }
            public SelectList DDLLogType { get; set; }
            public string DDLLogTypeValue { get; set; }
            public SelectList DDLUser { get; set; }
            public string DDLUserValue { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
            public DateTime FromDate { get; set; }
            [DataType(DataType.Time)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
            public DateTime FromTime { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
            public DateTime ToDate { get; set; }
            [DataType(DataType.Time)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
            public DateTime ToTime { get; set; }
        }
        public List<DateTime> DateAllMonth = new List<DateTime>();
        public List<List<DateTime>> DateAllMonthChunk = new List<List<DateTime>>();
        //public List<MonthClass> DateAllMonth = new List<MonthClass>();
        //public List<List<MonthClass>> DateAllMonthChunk = new List<List<MonthClass>>();

        [BindProperties]
        public class MonthClass
        {
            public DateTime DateAll { get; set; }
            public bool CurrentDate { get; set; }
            object Data { get; set; }
        }
    }
}
