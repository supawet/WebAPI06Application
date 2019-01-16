using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace WebAPI06Application.Models
{
    public class User
    {
        //[Required]
        //[Display(Name = "User name")]
        public String ID_Card { get; set; }         //ID_Card (hash)
        public String UnitHolder { get; set; }      //    UnitHolder (hash)
        //public String Email { get; set; }
        //public String Password { get; set; }    //pin
    }

    public class User_OTP
    {
        public String OTP { get; set; }
        //public String UnitHolder { get; set; }      //    UnitHolder (hash)
        //public String Email { get; set; }
        //public String Password { get; set; }    //pin
    }

    public class User_PIN
    {
        public String PIN { get; set; }
        //public String UnitHolder { get; set; }      //    UnitHolder (hash)
        //public String Email { get; set; }
        //public String Password { get; set; }    //pin
    }
}