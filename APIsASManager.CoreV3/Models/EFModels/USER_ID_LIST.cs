using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models.EFModels
{
    public partial class USER_ID_LIST
    {
        [Newtonsoft.Json.JsonIgnore]
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public DateTime Created { get; set; }
        public string ch_id { get; set; }
        public string ch_name { get; set; }
    }
}
