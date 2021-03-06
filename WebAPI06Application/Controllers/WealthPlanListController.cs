﻿using System;
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

    [RoutePrefix("api/WealthPlanList")]

    public class WealthPlanListController : ApiController
    {
        // GET: api/WealthPlanList
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello REST API", "I am Authorized" };
        }

        // GET: api/WealthPlanList/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/WealthPlanList
        //public void Post([FromBody]string value)
        //public WealthPlanListResponse Post([FromBody]string value)
        public WealthPlanListResponse Post(WealthPlanList wealthPlanList)
        {
            WealthPlanListPersistance wealthPlanListPersistance = new WealthPlanListPersistance();
            if (wealthPlanListPersistance == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            //WealthPlan wealthPlan = new WealthPlan();

            return wealthPlanListPersistance.GetWealthPlanList(wealthPlanList);
        }


        // PUT: api/WealthPlanList/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/WealthPlanList/5
        public void Delete(int id)
        {
        }
    }
}