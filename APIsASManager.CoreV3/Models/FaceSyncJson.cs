using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models
{
    public class FaceSyncJson
    {
        public string userId { get; set; }
        public string deptName { get; set; }
        public string deviceId { get; set; }
        public string cardNum { get; set; }
        public string newCardNum { get; set; }
        public string name { get; set; }
        public string base64 { get; set; }
        public int delFlag { get; set; }

    }
}
