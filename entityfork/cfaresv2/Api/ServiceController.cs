using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace cfaresv2.Api
{
    public class ServiceController : ApiController
    {
        // GET api/service
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/service/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/service
        public void Post([FromBody]string value)
        {
        }

        // PUT api/service/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/service/5
        public void Delete(int id)
        {
        }
    }
}
