using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI06Application.Models
{
    public class WealthPlanTarget
    {
        public string AccessToken { get; set; }
        public decimal Amount_Needed { get; set; }
        public decimal Investment_Period { get; set; }
        public decimal Initial_Investment { get; set; }
        public decimal Investment_Per_Month { get; set; }
        public decimal Investment_Risk { get; set; }
    }

    public class WealthPlanTargetResponse
    {
        public decimal Test { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}