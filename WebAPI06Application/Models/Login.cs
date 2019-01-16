using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI06Application.Models
{
    public class Login
    {
        //public string UnitHolder { get; set; }
        public string ID_Card { get; set; }
        public string Mobile_No { get; set; }
        public string PIN { get; set; }
        //public string Face_ID { get; set; }
        //public string Type { get; set; }    //  1 = Std,    2 = B-Chanel

        //public ArrayList ALLOCATION { get; set; }
        //public double? PERCENT { get; set; }
    }
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}