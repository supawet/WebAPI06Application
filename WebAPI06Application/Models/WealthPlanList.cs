using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI06Application.Models
{
    public class WealthPlanList
    {
        public string AccessToken { get; set; }

    }
    public class WealthPlanListResponse
    {
        public List<WealthPlanListName> Data { get; set; }

        public string Message { get; set; }
        public string Status { get; set; }
    }

    public class WealthPlanListName
    {
        public string WealthPlanName { get; set; }
    }
}