using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models.EFModels
{
    public class GET_USER
    {
        [Newtonsoft.Json.JsonIgnore]
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public DateTime Created { get; set; }
        public int ch_id { get; set; }
        public string ch_name { get; set; }
        public string ch_first_name { get; set; }
        public string ch_last_name { get; set; }
        public string ch_middle_name { get; set; }
        public int ch_gender { get; set; }
        public string ch_idcard_no { get; set; }
        public int ch_type { get; set; }
        public string b_address { get; set; }
        public string b_company { get; set; }
        public string b_department { get; set; }
        public string b_division { get; set; }
        public string b_email { get; set; }
        public string b_employee_id { get; set; }
        public string b_fax { get; set; }
        public DateTime b_hire { get; set; }
        public string b_job_title { get; set; }
        public string b_notes { get; set; }
        public string b_office { get; set; }
        public string b_phone { get; set; }
        public string b_phone_ext { get; set; }
        public string u_def01 { get; set; }
        public string u_def02 { get; set; }
    }
}
