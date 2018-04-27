using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Galaxy.Core.Services;
using Owin;
using System.Web.Http;

namespace Galaxy.DirectoryService
{
    public partial class DirectoryService : GalaxyService
    {
        public string baseAddress = "http://localhost:" + 
            GalaxyServiceDirectory.DIRECTORY_SVC_PORT.ToString();

        [STAThread]
        public static void Main()
        {
            new GalaxyServiceBootstrapper<DirectoryService>().Run();
        }

        protected override void OnConfigureService(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);
        }

        protected override void OnServiceStart()
        {
            Trace.WriteLine("DirectoryService::OnStart: called, baseAddress=" + baseAddress);

            _server = WebApp.Start<DirectoryService>(url: baseAddress);

            Trace.WriteLine("DirectoryService::OnStart: done.");
        }

        protected override void OnServiceStop()
        {
            Trace.WriteLine("DirectoryService::OnStop: called ...");

            if (_server != null)
                _server.Dispose();

            Trace.WriteLine("DirectoryService::OnStop: done.");
        }
    }
}
