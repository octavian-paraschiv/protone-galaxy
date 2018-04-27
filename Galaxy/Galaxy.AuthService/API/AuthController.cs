using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Galaxy.DirectoryService.API
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        [Route("login/{user}/{passwd}")]
        [HttpGet]
        public string Login(string user, string passwd)
        {
            return "";
        }
        
        [Route("logout/{user}")]
        [HttpGet]
        public string Logout(string user)
        {
            return "";
        }

        [Route("logout2/{token}")]
        [HttpGet]
        public string Logout2(string token)
        {
            return "";
        }

        [Route("validate/{token}")]
        [HttpGet]
        public bool ValidateToken(string token)
        {
            int i = int.Parse(token);
            return (i % 2 == 0);

            //return true;
        }
    }

}



