using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI06Application.Models
{
    public class WealthPlanTargetList
    {
        public string AccessToken { get; set; }

    }
    public class WealthPlanTargetListResponse
    {
        public List<WealthPlanTargetListName> Data { get; set; }

        public string Message { get; set; }
        public string Status { get; set; }
    }

    public class WealthPlanTargetListName
    {
        public string WealthPlanTargetName { get; set; }
    }
}