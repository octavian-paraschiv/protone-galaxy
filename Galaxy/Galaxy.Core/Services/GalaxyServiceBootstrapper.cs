using Galaxy.Core.Logging;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Galaxy.Core.Services
{
    public class GalaxyServiceBootstrapper<T> where T : GalaxyService
    {
        T _svc = null;

        public GalaxyServiceBootstrapper()
        {
            Logger.Initialize();
            _svc = Activator.CreateInstance(typeof(T)) as T;
        }

        public void Run()
        {
            string svcName = _svc.GetType().Name;

            if (Environment.UserInteractive)
            {
                Trace.WriteLine(string.Format("{0}: running as stand-alone app ...", svcName));
                _svc.RunStandAlone(Environment.GetCommandLineArgs());
            }
            else
            {
                Trace.WriteLine(string.Format("{0}: running as Windows service ...", svcName));
                ServiceBase.Run(_svc);
            }
        }
    }
}
