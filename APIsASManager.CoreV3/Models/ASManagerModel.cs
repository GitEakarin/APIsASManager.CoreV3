using APIsASManager.CoreV3.Models.EFModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Models
{
    public class ASManagerModel
    {
        public UserIdListClass UserIdListModel { get; set; }
        public class CommonClass
        {
            [JsonProperty(Order = 1)]
            public string success { get; set; }
            [JsonProperty(Order = 2)]
            public int total { get; set; }
        }
        public class UserIdListClass : CommonClass
        {
            public List<Models.EFModels.USER_ID_LIST> data { get; set; }
        }
        public class CardNoListClass : CommonClass
        {
            public List<Models.EFModels.CARD_NO_LIST> data { get; set; }
        }
        public class AccessGrantedClass //: CommonClass
        {
            [JsonProperty(Order = 1)]
            public string success { get; set; }
            [JsonProperty(Order = 2)]
            public int total { get; set; }
            [JsonProperty(Order = 3)]
            public List<Models.EFModels.ACCESS_GRANTED> data { get; set; }
        }
        public class GetUserClass : CommonClass
        {
            public List<Models.EFModels.GET_USER> data { get; set; }
        }
        public class GetCardClass : CommonClass
        {
            public List<Models.EFModels.GET_CARD> data { get; set; }
        }
    }
}
