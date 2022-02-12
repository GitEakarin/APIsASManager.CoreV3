using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models.EFModels
{
    public class EXCEPTION
    {
        public DateTime created { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public string exception { get; set; }
        public string exception_inner { get; set; }
    }
}
