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
    //[Authorize]

    [RoutePrefix("api/Login")]

    public class LoginController : ApiController
    {
        // GET: api/Login
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello REST API", "I am Authorized" };
        }

        // GET: api/Login/5
        public string Get(int id)
        {
            return "Hello Authorized API with ID = " + id;
        }

        // POST: api/Login
        //public void Post([FromBody]string value)
        //public LoginResponse Post([FromBody]string value)
        public LoginResponse Post(Login login)
        {
            LoginPersistance loginPersistance = new LoginPersistance();
            if (loginPersistance == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            //Signup signup = new Signup();

            return loginPersistance.GetLogin(login);
        }

        // PUT: api/Login/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Login/5
        public void Delete(int id)
        {
        }
    }
}
