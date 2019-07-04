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

    [RoutePrefix("api/WealthPlan")]

    public class WealthPlanController : ApiController
    {
        // GET: api/WealthPlan
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello REST API", "I am Authorized" };
        }

        // GET: api/WealthPlan/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/WealthPlan
        //public void Post([FromBody]string value)
        //public WealthPlanResponse Post([FromBody]string value)
        public WealthPlanResponse Post(WealthPlan wealthPlan)
        {
            WealthPlanPersistance wealthPlanPersistance = new WealthPlanPersistance();
            if (wealthPlanPersistance == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            //WealthPlan wealthPlan = new WealthPlan();

            return wealthPlanPersistance.GetWealthPlan(wealthPlan);
        }
        

        // PUT: api/WealthPlan/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/WealthPlan/5
        public void Delete(int id)
        {
        }
    }
}