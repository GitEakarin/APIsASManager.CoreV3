using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models
{
    public class HeartBrakeJson
    {
        public string device_no { get; set; }
        public string device_status { get; set; }
        public string create_date { get; set; }
        public string nonce { get; set; }
        public string sign { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
    }
}
