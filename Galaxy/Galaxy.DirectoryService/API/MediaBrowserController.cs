using Galaxy.Core.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Galaxy.DirectoryService.API
{
    [RoutePrefix("api/mediaBrowser")]
    public class MediaBrowserController : ApiController
    {
        [Route("name/{token}/{name}")]
        [HttpGet]
        public IEnumerable<string> GetByFileName(string token, string name)
        {
            var client = new RestClient("http://localhost:" + GalaxyServiceDirectory.AUTH_SVC_PORT);

            var request = new RestRequest("api/auth/validate/{token}", Method.GET);
            request.AddQueryParameter("token", token);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            if (content == "false")
                throw new UnauthorizedAccessException("The specified token is not valid.");

            return new string[] { "a", "b" };
        }
    }
}
