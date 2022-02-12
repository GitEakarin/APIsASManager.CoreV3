using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models.EFModels
{
    public class VIEW_TEMPERATURE_RECORD
    {
        public DateTime created { get; set; }
        public string qrcode { get; set; }
        public string device_no { get; set; }
        public string base64 { get; set; }
        public string oper_type { get; set; }
        public string oper_type_desc { get; set; }
        public string temperature { get; set; }
        public string userName { get; set; }
        public string nonce { get; set; }
        public string sign { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
    }
}
