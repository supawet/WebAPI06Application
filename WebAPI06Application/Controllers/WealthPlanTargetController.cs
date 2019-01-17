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

    [RoutePrefix("api/WealthPlanTarget")]

    public class WealthPlanTargetController : ApiController
    {
        // GET: api/WealthPlanTarget
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello REST API", "I am Authorized" };
        }

        // GET: api/WealthPlanTarget/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/WealthPlanTarget
        //public void Post([FromBody]string value)
        //public WealthPlanTargetResponse Post([FromBody]string value)
        public WealthPlanTargetResponse Post(WealthPlanTarget wealthPlanTarget)
        {
            WealthPlanTargetPersistance wealthPlanTargetPersistance = new WealthPlanTargetPersistance();
            if (wealthPlanTargetPersistance == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            //WealthPlanTarget wealthPlanTarget = new WealthPlanTarget();

            return wealthPlanTargetPersistance.GetWealthPlanTarget(wealthPlanTarget);
        }


        // PUT: api/WealthPlanTarget/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/WealthPlan/5
        public void Delete(int id)
        {
        }
    }
}