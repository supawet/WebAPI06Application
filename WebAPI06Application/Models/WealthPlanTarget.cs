using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI06Application.Models
{
    public class ExpectedReturn
    {
        public double SD { get; set; }
        public double RET { get; set; }
    }

    public class Proceeds   //  เงินที่ได้
    {
        public int Month { get; set; }
        public double Lowest { get; set; }
        public double Downtrend { get; set; }
        public double NormL { get; set; }
        public double NormH { get; set; }
        public double Uptrend { get; set; }
    }

    public class WealthPlanTarget
    {
        public string AccessToken { get; set; }
        public double Amount_Needed { get; set; }
        public double Investment_Period { get; set; }
        public double Initial_Investment { get; set; }
        public double Investment_Per_Month { get; set; }
        public int Investment_Risk { get; set; }
        //public double Interest { get; set; }
    }

    public class WealthPlanTargetResponse
    {
        public string Target { get; set; }
        public double? Target_Month { get; set; }
        public double? Target_Amount { get; set; }
        public double? Recommended_Choice1 { get; set; }
        public double? Recommended_Choice2 { get; set; }
        public double? Recommended_Choice3 { get; set; }
        public double Amount_Needed { get; set; }
        public double Investment_Period { get; set; }
        public double Initial_Investment { get; set; }
        public double Investment_Per_Month { get; set; }
        public int Investment_Risk { get; set; }
        public List<Proceeds> Plot { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}