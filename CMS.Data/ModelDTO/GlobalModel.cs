using CMS.Data.ModelEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Data.ModelDTO
{
    public class GlobalModel
    {
        public int? totalUnread { get; set; }
        public List<SpUserNotifySearchResult> lstUserNoti { get; set; } = new List<SpUserNotifySearchResult>();
        public string avatar { get; set; } = "noimages.png";
        public ClaimsPrincipal user { get; set; }
        public string userId { get; set; }
        
    }

}
