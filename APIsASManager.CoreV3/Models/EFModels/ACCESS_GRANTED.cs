using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models.EFModels
{
	public class ACCESS_GRANTED
	{
        [Newtonsoft.Json.JsonIgnore]
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        internal  DateTime Created { get; set; }
        public int log_id { get; set; }
        public int log_msg_id { get; set; }
        public string log_direction { get; set; }
        public string log_time { get; set; }
        public string log_msg { get; set; }
        public int log_controller_id { get; set; }
        public string log_door { get; set; }
        public int log_door_id { get; set; }
        public string log_utc_value { get; set; }
        public string log_utc { get; set; }
        public string log_snapshot1 { get; set; }
        public string log_snapshot2 { get; set; }
        public string c_access_group { get; set; }
        public int code_value { get; set; }
        public string c_no { get; set; }
        public int ch_id { get; set; }
        public string ch_name { get; set; }
        public string ch_first_name { get; set; }
        public string ch_last_name { get; set; }
        public string u_def01 { get; set; }
        public string u_def02 { get; set; }
        public string access_date { get; set; }
    }
}
