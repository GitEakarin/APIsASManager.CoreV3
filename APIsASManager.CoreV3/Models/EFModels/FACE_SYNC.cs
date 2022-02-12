using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models.EFModels
{
    public class FACE_SYNC : FaceSyncJson
    {
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
