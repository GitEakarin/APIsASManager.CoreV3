using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models.EFModels
{
    public class CARD_NO_LIST
    {
        [Newtonsoft.Json.JsonIgnore]
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public DateTime Created { get; set; }
        public int c_code { get; set; }
        public string c_no { get; set; }
    }
}
