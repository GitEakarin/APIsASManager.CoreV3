using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models.EFModels
{
    public class HEART_BREAK
    {
        public DateTime created { get; set; }
        public string device_no { get; set; }
        public string device_status { get; set; }
        public string create_date { get; set; }
        public string nonce { get; set; }
        public string sign { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
    }
}
