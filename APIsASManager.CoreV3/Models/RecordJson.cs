using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models
{
    public class RecordJson
    {
        /// <summary>

        /// It's userid, If it's in all-person mode, a random uuid is used

        /// </summary>

        public string qrcode { get; set; }

        /// <summary>

        /// QR code number on the device

        /// </summary>

        public string device_no { get; set; }

        /// <summary>

        /// Capture of current passers-by

        /// </summary>

        public string base64 { get; set; }

        /// <summary>

        /// Open Door Way ('0'、Face；'1'、scan code；'2'、swipe card)

        /// </summary>

        public string oper_type { get; set; }

        /// <summary>

        /// The body temperature of the current passer-by

        /// </summary>

        public string temperature { get; set; }

        /// <summary>

        /// Name of current passer

        /// </summary>

        public string userName { get; set; }

        /// <summary>

        /// Implement your own interface,  regardless of this

        /// </summary>

        public string nonce { get; set; }

        /// <summary>

        /// Implement your own interface,  regardless of this

        /// </summary>

        public string sign { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
    }
}
