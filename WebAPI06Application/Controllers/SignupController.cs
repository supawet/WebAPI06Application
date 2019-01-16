using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using WebAPI06Application.Models;

namespace WebAPI06Application.Controllers
{
    //[AuthenticationFilter]

    [RoutePrefix("api/Signup")]
    public class SignupController : ApiController
    {
        //[Route("")]
        // GET: api/Signup
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //[Route("{navDate}")]
        // GET: api/Signup/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Signup
        //public void Post([FromBody]string value)
        //public SignupResponse Post([FromBody]string value)
        public SignupResponse Post(Signup signup)
        {
            SignupPersistance signupPersistance = new SignupPersistance();
            if (signupPersistance == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            //Signup signup = new Signup();

            return signupPersistance.GetSignup(signup);
        }

        // PUT: api/Signup/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Signup/5
        public void Delete(int id)
        {
        }
    }
}