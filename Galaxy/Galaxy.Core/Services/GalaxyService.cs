using Owin;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Galaxy.Core.Services
{
    public abstract class GalaxyService : ServiceBase
    {
        protected IDisposable _server = null;

        public void Configuration(IAppBuilder app)
        {
            OnConfigureService(app);
        }

        internal void RunStandAlone(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                OnServiceStart();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        protected override void OnStop()
        {
            try
            {
                OnServiceStop();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        protected abstract void OnServiceStart();
        protected abstract void OnServiceStop();
        protected abstract void OnConfigureService(IAppBuilder app);
    }
}
