﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using WebAPI06Application.Models;

namespace WebAPI06Application.Controllers
{
    //[AuthenticationFilter]

    [RoutePrefix("api/Signup_Verify")]
    public class Signup_VerifyController : ApiController
    {
        // GET: api/Signup_Verify
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Signup_Verify/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Signup_Verify
        public SignupVerifyResponse Post(SignupVerify signupverify)
        {
            SignupPersistance signupPersistance = new SignupPersistance();
            if (signupPersistance == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            //Signup signup = new Signup();

            return signupPersistance.GetSignupVerify(signupverify);
        }

        // PUT: api/Signup_Verify/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Signup_Verify/5
        public void Delete(int id)
        {
        }
    }
}