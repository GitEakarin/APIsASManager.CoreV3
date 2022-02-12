using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models.EFModels
{
    public class GET_CARD
    {
        [Newtonsoft.Json.JsonIgnore]
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public DateTime Created { get; set; }
        public int ch_id { get; set; }
        public int c_code { get; set; }
        public string c_no { get; set; }
        public string ch_name { get; set; }
        public int c_access_group { get; set; }
        public string c_access_group_name { get; set; }
        public DateTime c_activation { get; set; }
        public int c_activation_enable { get; set; }
        public int c_data_group_id { get; set; }
        public string c_data_group_name { get; set; }
        public string c_data_group_privilege { get; set; }
        public int c_deactivation_enable { get; set; }
        public string c_def01 { get; set; }
        public string c_def02 { get; set; }
        public string c_def03 { get; set; }
        public string c_def04 { get; set; }
        public string c_def05 { get; set; }
        public string c_def06 { get; set; }
        public int c_disable_lock_card { get; set; }
        public int c_privilege { get; set; }
        public int c_status { get; set; }
        public string c_support { get; set; }
        public int c_type { get; set; }
        public string dg_privilege { get; set; }
        public string pin_code { get; set; }
    }
}
