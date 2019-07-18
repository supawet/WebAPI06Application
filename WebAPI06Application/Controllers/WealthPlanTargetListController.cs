using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using WebAPI06Application.Models;
using System.Web.Http.Cors;

namespace WebAPI06Application.Controllers
{
    //[AuthenticationFilter]
    //[Authorize]

    //[EnableCors(origins: "http://localhost", headers: "*", methods: "*")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    [RoutePrefix("api/WealthPlanTargetList")]

    public class WealthPlanTargetListController : ApiController
    {
        // GET: api/WealthPlanTargetList
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello REST API", "I am Authorized" };
        }

        // GET: api/WealthPlanTargetList/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/WealthPlanTargetList
        //public void Post([FromBody]string value)
        //public WealthPlanTargetListResponse Post([FromBody]string value)
        public WealthPlanTargetListResponse Post(WealthPlanTargetList wealthPlanTargetList)
        {
            WealthPlanTargetListPersistance wealthPlanTargetListPersistance = new WealthPlanTargetListPersistance();
            if (wealthPlanTargetListPersistance == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            //WealthPlanTarget wealthPlanTarget = new WealthPlanTarget();

            return wealthPlanTargetListPersistance.GetWealthPlanTargetList(wealthPlanTargetList);
        }


        // PUT: api/WealthPlanTargetList/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/WealthPlanTargetList/5
        public void Delete(int id)
        {
        }
    }
}
