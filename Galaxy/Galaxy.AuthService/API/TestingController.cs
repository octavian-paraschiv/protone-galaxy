using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Galaxy.DirectoryService.API
{
    [RoutePrefix("api/testing")]
    public class TestingController : ApiController
    {
        [Route("hello")]
        [HttpGet]
        public string Hello()
        {
            return "hello world";
        }

        [Route("gettime")]
        [HttpGet]
        public DateTime GetTime()
        {
            return DateTime.Now;
        }
    }

}



