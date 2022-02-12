using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models
{
    public class RakindaModel
    {
        public RakindaModel()
        {
            Search = new SearchClass();
        }
        [BindProperty]
        public SearchClass Search { get; set; }
        public List<Models.EFModels.VIEW_RECORD_DATA> ListPageModel = new List<EFModels.VIEW_RECORD_DATA>();
        public List<Models.EFModels.VIEW_TEMPERATURE_RECORD> ListTemperatureRecord = new List<EFModels.VIEW_TEMPERATURE_RECORD>();
        public List<Models.EFModels.EXCEPTION> ListExceptionModel = new List<EFModels.EXCEPTION>();
        public List<Models.EFModels.HEART_BREAK> ListHeartBreakModel = new List<EFModels.HEART_BREAK>();
        public List<Models.EFModels.CHECK_OPEN_AUTH> ListCheckOpenAuthen = new List<EFModels.CHECK_OPEN_AUTH>();
        public List<Models.EFModels.FACE_SYNC> ListFaceSync = new List<EFModels.FACE_SYNC>();

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
    }
}
