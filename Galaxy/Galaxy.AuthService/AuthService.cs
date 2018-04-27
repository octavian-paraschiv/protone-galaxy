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
    public partial class AuthService : GalaxyService
    {
        public string baseAddress = "http://localhost:" + 
            GalaxyServiceDirectory.AUTH_SVC_PORT.ToString();

        [STAThread]
        public static void Main()
        {
            new GalaxyServiceBootstrapper<AuthService>().Run();
        }

        protected override void OnConfigureService(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);
        }

        protected override void OnServiceStart()
        {
            Trace.WriteLine("AuthService::OnStart: called, baseAddress=" + baseAddress);

            _server = WebApp.Start<AuthService>(url: baseAddress);

            Trace.WriteLine("AuthService::OnStart: done.");
        }

        protected override void OnServiceStop()
        {
            Trace.WriteLine("AuthService::OnStop: called ...");

            if (_server != null)
                _server.Dispose();

            Trace.WriteLine("AuthService::OnStop: done.");
        }
    }
}
